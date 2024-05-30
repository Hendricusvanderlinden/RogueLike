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

    [SerializeField]
    private ItemType type;
    private void Start()
    {
        GameManager.Get.AddItem(this);
    }
    public ItemType Type => type;
}