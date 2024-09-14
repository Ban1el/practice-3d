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

    private List<Item> GetItemList()
    {
        return items;
    }

    private void OnEnable()
    {
        Actions.InventoryAddItem += AddItem;
        Actions.InventoryItemList += GetItemList;
    }

    private void OnDisable()
    {
        Actions.InventoryAddItem -= AddItem;
        Actions.InventoryItemList -= GetItemList;
    }
}