using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action : MonoBehaviour
{
    // Voer het einde van de beurt uit
    static private void EndTurn(Actor actor)
    {
        // Controleer of de actor een speler is
        if (actor.GetComponent<Player>() != null)
        {
            // Als het een speler is, roep dan de StartEnemyTurn-functie van GameManager aan
            GameManager.Get.StartEnemyTurn();
        }
    }

    // Beweeg de actor in de opgegeven richting
    static public void Move(Actor actor, Vector2 direction)
    {
        // Controleer of er iemand op de doelpositie staat
        Actor target = GameManager.Get.GetActorAtLocation(actor.transform.position + (Vector3)direction);

        // Als er niemand is, bewegen we
        if (target == null)
        {
            actor.Move(direction);
            actor.UpdateFieldOfView();
        }

        // Beëindig de beurt als dit de speler is
        EndTurn(actor);
    }
}