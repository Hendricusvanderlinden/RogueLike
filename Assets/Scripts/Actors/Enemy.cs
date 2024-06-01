using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Actor), typeof(AStar))]
public class Enemy : MonoBehaviour
{
    // Variabelen
    public Actor Target { get; set; }
    public bool IsFighting { get; private set; } = false;
    private AStar algorithm;
    private int confused = 0; // Toegevoegd


    // Start wordt aangeroepen vóór de eerste frame-update
    void Start()
    {
        // Stel het algoritme in op het AStar-component van dit script
        algorithm = GetComponent<AStar>();

        // Voeg het Actor-component toe aan de Enemies-lijst van GameManager
        GameManager.Get.AddEnemy(GetComponent<Actor>());
    }


    // Functie om langs het pad naar de doelpositie te bewegen
    public void MoveAlongPath(Vector3Int targetPosition)
    {
        Vector3Int gridPosition = MapManager.Get.FloorMap.WorldToCell(transform.position);
        Vector2 direction = algorithm.Compute((Vector2Int)gridPosition, (Vector2Int)targetPosition);
        Action.Move(GetComponent<Actor>(), direction);
    }

    // Functie om de AI van de vijand uit te voeren
    public void RunAI()
    {
        // If target is null, set target to player (from gameManager)
        if (Target == null)
        {
            Target = GameManager.Get.Player;
        }

        // Check of de enemy in de war is
        if (confused > 0)
        {
            // Verminder de confused-waarde met 1
            confused--;

            // Toon een bericht dat de enemy in de war is
            UIManager.Get.AddMessage($"The {name} is confused and cannot act", Color.yellow);

            return; // Stop de AI-verwerking
        }

        // convert the position of the target to a gridPosition
        var gridPosition = MapManager.Get.FloorMap.WorldToCell(Target.transform.position);

        // First check if already fighting, because the FieldOfView check costs more cpu
        if (IsFighting || GetComponent<Actor>().FieldOfView.Contains(gridPosition))
        {
            // If the enemy was not fighting, it should be fighting now
            if (!IsFighting)
            {
                IsFighting = true;
            }

            // See how far away the player is
            float targetDistance = Vector3.Distance(transform.position, Target.transform.position);

            // if close ...
            if (targetDistance <= 1.5f)
            {
                // ... hit!
                Action.Hit(GetComponent<Actor>(), Target);
            }
            else
            {
                // call MoveAlongPath with the gridPosition
                MoveAlongPath(gridPosition);
            }
        }
    }
    // Public functie om de enemy in de war te brengen
    public void Confuse()
    {
        confused = 8;
    }
}