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

    public int MaxHitPoints { get { return maxHitPoints; } }
    public int HitPoints { get { return hitPoints; } }
    public int Defense { get { return defense; } }
    public int Power { get { return power; } }

    private UIManager uiManager;

    private void Start()
    {
        algorithm = new AdamMilVisibility();
        UpdateFieldOfView();
        if (GetComponent<Player>())
        {
            UIManager.Instance.UpdateHealth(hitPoints, maxHitPoints);
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

    public void DoDamage(int hp)
    {
        hitPoints -= hp;
        hitPoints = Mathf.Max(hitPoints, 0);

        if (GetComponent<Player>())
        {
            uiManager.UpdateHealth(hitPoints, maxHitPoints);
        }

        if (hitPoints <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (GetComponent<Player>())
        {
            UIManager.Instance.AddMessage("You died!", Color.red);
        }
        else
        {
            UIManager.Instance.AddMessage($"{name} is dead!", Color.green);
        }

        GameObject remains = GameManager.Get.CreateActor("Gravestone", transform.position);
        remains.name = $"Remains of {name}";

        if (GetComponent<Enemy>())
        {
            GameManager.Get.RemoveEnemy(this);
        }

        Destroy(gameObject);
    }
}
