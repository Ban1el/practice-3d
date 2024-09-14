using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class ItemFrameUIManager : MonoBehaviour
{
    private List<ItemFrameUIController> itemFrameUIControllers = new List<ItemFrameUIController>();

    private void Awake()
    {
        ItemFrameUIController[] controllers = GetComponentsInChildren<ItemFrameUIController>(true);
        itemFrameUIControllers.AddRange(controllers);

        SetSlotNo();
    }

    private void UpdateInventory(int index)
    {
        if (Actions.InventoryItemList() != null)
        {
            if (Actions.InventoryItemList().Count > index)
            {
                itemFrameUIControllers[index].UpdateItem(index, Actions.InventoryItemList()[index]);
            }
        }
    }

    private void SetSlotNo()
    {
        int slot = 0;
        foreach (ItemFrameUIController item in itemFrameUIControllers)
        {
            item.itemSlotNumber = slot;
            slot++;
        }
    }

    private void OnEnable()
    {
        Actions.CheckUpdateInventory += UpdateInventory;
    }

    private void OnDisable()
    {
        Actions.CheckUpdateInventory -= UpdateInventory;
    }
}
