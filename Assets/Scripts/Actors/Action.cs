using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action : MonoBehaviour
{
    static public void MoveOrHit(Actor actor, Vector2 direction)
    {
        Actor target = GameManager.Get.GetActorAtLocation(actor.transform.position + (Vector3)direction);
        if (target == null)
        {
            Move(actor, direction);
        }
        else
        {
            Hit(actor, target);
        }
        EndTurn(actor);
    }

    static public void Move(Actor actor, Vector2 direction)
    {
        actor.Move(direction);
        actor.UpdateFieldOfView();
    }

    static private void EndTurn(Actor actor)
    {
        if (actor.GetComponent<Player>() != null)
        {
            GameManager.Get.StartEnemyTurn();
        }
    }

    static public void Hit(Actor actor, Actor target)
    {
        int damage = actor.Power - target.Defense;
        string description = $"{actor.name} attacks {target.name}";
        Color color = actor.GetComponent<Player>() ? Color.white : Color.red;

        if (damage > 0)
        {
            UIManager.Get.AddMessage($"{description} for {damage} hit points.", color);
            target.DoDamage(damage, actor); // Pass the attacker
        }
        else
        {
            UIManager.Get.AddMessage($"{description} but does no damage.", color);
        }
    }
}

