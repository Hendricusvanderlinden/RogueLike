using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static UnityEditor.Progress;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public FloorInfo floorInfo;

    private List<Actor> enemies = new List<Actor>(); // List of enemies
    private List<Ladder> ladders = new List<Ladder>(); // List of ladders
    private List<Tombstone> tombstones = new List<Tombstone>(); // List of tombstones

    public Actor Player { get; set; } // Player

    [System.Serializable]
    public class PlayerData
    {
        public int MaxHitPoints;
        public int HitPoints;
        public int Defense;
        public int Power;
        public int Level;
        public int XP;
        public int XpToNextLevel;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public static GameManager Get { get => instance; }

    private Player player;

    public void SetPlayer(Player player)
    {
        this.player = player;
        LoadPlayerData();
    }

    public void SavePlayerData()
    {
        if (player == null) return;

        PlayerData data = new PlayerData
        {
            MaxHitPoints = player.MaxHitPoints,
            HitPoints = player.HitPoints,
            Defense = player.Defense,
            Power = player.Power,
            Level = player.Level,
            XP = player.XP,
            XpToNextLevel = player.XpToNextLevel
        };

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/playerdata.json", json);
    }

    public void LoadPlayerData()
    {
        string path = Application.persistentDataPath + "/playerdata.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);

            player.MaxHitPoints = data.MaxHitPoints;
            player.HitPoints = data.HitPoints;
            player.Defense = data.Defense;
            player.Power = data.Power;
            player.Level = data.Level;
            player.XP = data.XP;
            player.XpToNextLevel = data.XpToNextLevel;
        }
    }

    public void DeletePlayerData()
    {
        string path = Application.persistentDataPath + "/playerdata.json";
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    private void OnApplicationQuit()
    {
        SavePlayerData();
    }

    public GameObject CreateGameObject(string name, Vector2 position)
    {
        GameObject actor = Instantiate(Resources.Load<GameObject>($"Prefabs/{name}"), new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity);
        actor.name = name;
        return actor;
    }

    private List<Consumable> items = new List<Consumable>();

    public void AddItem(Consumable item)
    {
        items.Add(item);
    }

    public void RemoveItem(Consumable item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
        }
    }

    public Consumable GetItemAtLocation(Vector3 location)
    {
        foreach (Consumable item in items)
        {
            if (item.transform.position == location)
            {
                return item;
            }
        }
        return null;
    }

    public void AddEnemy(Actor enemy)
    {
        enemies.Add(enemy);

        // Update enemies left text
        UpdateEnemiesLeftText();
    }

    public void RemoveEnemy(Actor enemy)
    {
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);

            // Update enemies left text
            UpdateEnemiesLeftText();
        }
    }

    private void UpdateEnemiesLeftText()
    {
        int enemiesLeft = enemies.Count;
        UIManager.Get.floorInfo.SetEnemiesLeftText($"{enemiesLeft} enemies left");
    }

    public void StartEnemyTurn()
    {
        foreach (var enemy in enemies)
        {
            Enemy enemyComponent = enemy.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                enemyComponent.RunAI();
            }
        }
    }

    public Actor GetActorAtLocation(Vector3 location)
    {
        if (Player != null && location == Player.transform.position)
        {
            return Player;
        }

        foreach (var enemy in enemies)
        {
            if (enemy != null && location == enemy.transform.position)
            {
                return enemy;
            }
        }
        return null;
    }

    public GameObject CreateActor(string name, Vector2 position)
    {
        GameObject actor = Instantiate(Resources.Load<GameObject>($"Prefabs/{name}"), new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity);

        if (name == "Player")
        {
            Player = actor.GetComponent<Actor>();
        }
        else
        {
            AddEnemy(actor.GetComponent<Actor>());
        }

        actor.name = name;
        return actor;
    }

    public List<Actor> GetNearbyEnemies(Vector3 location)
    {
        List<Actor> nearbyEnemies = new List<Actor>();

        foreach (var enemy in enemies)
        {
            float distance = Vector3.Distance(location, enemy.transform.position);
            if (distance < 5f)
            {
                nearbyEnemies.Add(enemy);
            }
        }

        return nearbyEnemies;
    }

    // Add the following methods for handling ladders
    public void AddLadder(Ladder ladder)
    {
        ladders.Add(ladder);
    }

    public Ladder GetLadderAtLocation(Vector3 location)
    {
        foreach (Ladder ladder in ladders)
        {
            if (Vector3.Distance(ladder.transform.position, location) < 0.1f)
            {
                return ladder;
            }
        }
        return null;
    }

    // Add function to add tombstone
    public void AddTombstone(Tombstone stone)
    {
        tombstones.Add(stone);
    }

    // Function to clear the current floor
    public void ClearFloor()
    {
        foreach (var enemy in enemies)
        {
            Destroy(enemy.gameObject);
        }
        foreach (var item in items)
        {
            Destroy(item.gameObject);
        }
        foreach (var ladder in ladders)
        {
            Destroy(ladder.gameObject);
        }
        foreach (var stone in tombstones)
        {
            Destroy(stone.gameObject);
        }

        enemies.Clear();
        items.Clear();
        ladders.Clear();
        tombstones.Clear();
    }
}

