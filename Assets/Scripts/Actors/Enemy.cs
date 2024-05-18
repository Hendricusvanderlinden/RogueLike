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

    // Update wordt eens per frame aangeroepen
    void Update()
    {
        RunAI();
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
        // Als de doelwit null is, stel het doelwit in op de speler (van GameManager)
        if (Target == null)
        {
            Target = GameManager.Get.Player;
        }

        // Converteer de positie van het doelwit naar een gridpositie
        Vector3Int gridPosition = MapManager.Get.FloorMap.WorldToCell(Target.transform.position);

        // Controleer eerst of al aan het vechten is, omdat de FieldOfView-controle meer CPU kost
        if (IsFighting || GetComponent<Actor>().FieldOfView.Contains(gridPosition))
        {
            // Als de vijand niet aan het vechten was, zou hij nu moeten vechten
            IsFighting = true;

            // Roep MoveAlongPath aan met de gridpositie
            MoveAlongPath(gridPosition);
        }
    }
}