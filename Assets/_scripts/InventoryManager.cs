using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private List<Item> items = new List<Item>();

    private void UpdateInventory()
    {
        if (items.Count > 0)
        {
            int slotNo = 0;
            foreach (Item item in items)
            {
                Actions.UpdateItem?.Invoke(slotNo, item);
                slotNo++;
            }
        }
    }

    private void AddItem(Item item)
    {
        items.Add(item);
    }

    private void OnEnable()
    {
        Actions.InventoryAddItem += AddItem;
        Actions.UpdateInventory += UpdateInventory;
    }

    private void OnDisable()
    {
        Actions.InventoryAddItem -= AddItem;
        Actions.UpdateInventory -= UpdateInventory;
    }
}
