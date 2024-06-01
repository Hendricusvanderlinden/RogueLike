using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<Consumable> items = new List<Consumable>();
    public int MaxItems = 10;

    public List<Consumable> Items { get; internal set; }

    public bool AddItem(Consumable item)
    {
        if (items.Count < MaxItems)
        {
            items.Add(item);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void DropItem(Consumable item)
    {
        items.Remove(item);
    }
}