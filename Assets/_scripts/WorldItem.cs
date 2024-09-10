using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldItem : MonoBehaviour, IPickable
{
    [SerializeField] private Item item;

    private void Start()
    {
        item.name = "Room key";
        item.description = "Key to open a door.";
    }

    public void Pick()
    {
        Actions.InventoryAddItem?.Invoke(item);
        Destroy(this.gameObject);
    }
}
