using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumable : MonoBehaviour
{
    public enum ItemType
    {
        HealthPotion,
        Fireball,
        ScrollOfConfusion
    }
    public int HealingAmount;
    public int Damage;
    [SerializeField]
    private ItemType type;

    public ItemType Type => type;


    private void Start()
    {
        GameManager.Get.AddItem(this);
    }

}