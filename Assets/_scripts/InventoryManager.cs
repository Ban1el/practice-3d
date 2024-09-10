using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private List<Item> items = new List<Item>();
    private void AddItem(Item item)
    {
        items.Add(item);
    }

    private List<Item> GetItems()
    {
        return items;
    }

    private void OnEnable()
    {
        Actions.InventoryItemList += GetItems;
        Actions.InventoryAddItem += AddItem;
    }

    private void OnDisable()
    {
        Actions.InventoryItemList -= GetItems;
        Actions.InventoryAddItem -= AddItem;
    }
}
