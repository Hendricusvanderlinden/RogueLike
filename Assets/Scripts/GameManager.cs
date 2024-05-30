using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;


    private List<Actor> enemies = new List<Actor>(); // Lijst van vijanden

    public Actor Player { get; set; } // Speler


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    

    public static GameManager Get { get => instance; }

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
        items.Remove(item);
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

    // Functie om een vijand toe te voegen aan de lijst
    public void AddEnemy(Actor enemy)
    {
        enemies.Add(enemy);
    }

    public void RemoveEnemy(Actor enemy)
    {
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
        }
    }

    // Functie om de beurt van de vijanden te starten
    public void StartEnemyTurn()
    {
        // Loop door alle enemies en voer de RunAI-functie uit voor elke vijand
        foreach (var enemy in enemies)
        {
            // Controleer of de actor daadwerkelijk een vijand is
            Enemy enemyComponent = enemy.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                enemyComponent.RunAI();
            }
        }
    }

    public Actor GetActorAtLocation(Vector3 location)
    {
        // Controleer of de locatie overeenkomt met de positie van de speler
        if (Player != null && location == Player.transform.position)
        {
            return Player;
        }

        // Loop door alle vijanden en vergelijk hun positie met de opgegeven locatie
        foreach (var enemy in enemies)
        {
            if (enemy != null && location == enemy.transform.position)
            {
                return enemy;
            }
        }

        // Geef null terug als geen actor is gevonden op de opgegeven locatie
        return null;
    }
    // Functie om een vijand toe te voegen aan de lijst
    public GameObject CreateActor(string name, Vector2 position)
    {
        // Instantiate the actor prefab based on the provided name
        GameObject actor = Instantiate(Resources.Load<GameObject>($"Prefabs/{name}"), new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity);

        // Check if the created actor is the player
        if (name == "Player")
        {
            // If it's the player, set the Player variable to its Actor component
            Player = actor.GetComponent<Actor>();
        }
        else
        {
            // If it's not the player, add it to the list of enemies
            AddEnemy(actor.GetComponent<Actor>());
        }

        // Set the name of the actor game object
        actor.name = name;

        // Return the instantiated actor
        return actor;
    }
}
