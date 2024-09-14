using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFrameUIManager : MonoBehaviour
{
    private List<ItemFrameUIController> itemFrameUIControllers = new List<ItemFrameUIController>();

    private void Awake()
    {
        ItemFrameUIController[] controllers = GetComponentsInChildren<ItemFrameUIController>(true);
        itemFrameUIControllers.AddRange(controllers);

        SetSlotNo();
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
        Actions.UpdateInventory?.Invoke();
    }
}
