using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    private AdamMilVisibility algorithm;
    public List<Vector3Int> FieldOfView = new List<Vector3Int>();
    public int FieldOfViewRange = 8;

    [Header("Powers")]
    [SerializeField] private int maxHitPoints;
    [SerializeField] private int hitPoints;
    [SerializeField] private int defense;
    [SerializeField] private int power;

    // New variables for level and XP
    [SerializeField] private int level = 1;
    [SerializeField] private int xp = 0;
    [SerializeField] private int xpToNextLevel = 100;

    public int MaxHitPoints { get { return maxHitPoints; } }
    public int HitPoints { get { return hitPoints; } }
    public int Defense { get { return defense; } }
    public int Power { get { return power; } }
    public int Level { get { return level; } }
    public int Xp { get { return xp; } }
    public int XpToNextLevel { get { return xpToNextLevel; } }

    private UIManager uiManager;

    private void Start()
    {
        algorithm = new AdamMilVisibility();
        UpdateFieldOfView();
        if (GetComponent<Player>())
        {
            UIManager.Get.UpdateHealth(hitPoints, maxHitPoints);
            UIManager.Get.UpdateLevel(level);
            UIManager.Get.UpdateXP(xp);
        }
    }

    public void Move(Vector3 direction)
    {
        if (MapManager.Get.IsWalkable(transform.position + direction))
        {
            transform.position += direction;
        }
    }

    public void UpdateFieldOfView()
    {
        var pos = MapManager.Get.FloorMap.WorldToCell(transform.position);

        FieldOfView.Clear();
        algorithm.Compute(pos, FieldOfViewRange, FieldOfView);

        if (GetComponent<Player>())
        {
            MapManager.Get.UpdateFogMap(FieldOfView);
        }
    }

    public void DoDamage(int hp, Actor attacker)
    {
        hitPoints -= hp;
        hitPoints = Mathf.Max(hitPoints, 0);

        if (GetComponent<Player>())
        {
            uiManager.UpdateHealth(hitPoints, maxHitPoints);
        }

        if (hitPoints <= 0)
        {
            Die(attacker);
        }
    }

    private void Die(Actor attacker)
    {
        if (GetComponent<Player>())
        {
            UIManager.Get.AddMessage("You died!", Color.red); // Red
        }
        else
        {
            UIManager.Get.AddMessage($"{name} is dead!", new Color(1f, 0.64f, 0f)); // Light Orange
            if (attacker != null && attacker.GetComponent<Player>())
            {
                attacker.AddXp(xp);
            }
        }

        GameManager.Get.CreateGameObject("Dead", transform.position).name = $"Remains of {name}";
        GameManager.Get.RemoveEnemy(this);
        Destroy(gameObject);
    }

    public void Heal(int hp)
    {
        int effectiveHealing = Mathf.Min(MaxHitPoints - HitPoints, hp);
        hitPoints += effectiveHealing;
        hitPoints = Mathf.Min(hitPoints, MaxHitPoints);

        if (GetComponent<Player>())
        {
            UIManager.Get.UpdateHealth(hitPoints, MaxHitPoints);
        }

        string message = $"You were healed for {effectiveHealing} hit points.";
        UIManager.Get.AddMessage(message, Color.green);
    }

    public void AddXp(int xp)
    {
        this.xp += xp;
        UIManager.Get.UpdateXP(this.xp);
        while (this.xp >= xpToNextLevel)
        {
            level++;
            this.xp -= xpToNextLevel;
            xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.5f); // Increase required XP exponentially

            maxHitPoints += 10;
            defense += 2;
            power += 2;

            hitPoints = maxHitPoints; // Heal to full on level up

            UIManager.Get.AddMessage("You leveled up!", Color.yellow);
            UIManager.Get.UpdateLevel(level);
            UIManager.Get.UpdateHealth(hitPoints, maxHitPoints);
        }
    }
}

