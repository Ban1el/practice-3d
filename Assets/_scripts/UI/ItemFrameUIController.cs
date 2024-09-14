using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemFrameUIController : MonoBehaviour
{
    public int itemSlotNumber = 0;
    public GameObject ChildObj;
    public Image itemImage;
    private Item slotItem;
    private void Awake()
    {
        ChildObj = transform.GetChild(0).gameObject;
        itemImage = ChildObj.GetComponent<Image>();
    }

    private void UpdateItem(int slotNo, Item item) 
    {
        if (itemSlotNumber == slotNo)
        {
            if (slotItem == null)
            {
                SetSlot(item);
            }else if (item.GetItemID() != slotItem.GetItemID())
            {
                SetSlot(item);
            }
        }
    }

    private void SetSlot(Item item)
    {
        ChildObj.SetActive(true);
        slotItem = item;
        itemImage.sprite = item.icon;
    }

    private void OnEnable()
    {
        Actions.UpdateItem += UpdateItem;
    }

    private void OnDisable()
    {
        Actions.UpdateItem -= UpdateItem;
    }
}
