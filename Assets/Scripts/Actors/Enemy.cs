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
        // If target is null, set target to player (from GameManager)
        if (Target == null)
        {
            Target = GameManager.Get.Player;
        }

        // Convert the position of the target to a gridPosition
        var gridPosition = MapManager.Get.FloorMap.WorldToCell(Target.transform.position);

        // First check if already fighting, because the FieldOfView check costs more CPU
        if (IsFighting || GetComponent<Actor>().FieldOfView.Contains(gridPosition))
        {
            // If the enemy was not fighting, it should be fighting now
            if (!IsFighting)
            {
                IsFighting = true;
            }

            // Call MoveAlongPath with the gridPosition
            MoveAlongPath(gridPosition);
        }
    }
}