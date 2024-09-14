using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldItem : MonoBehaviour, IPickable
{
    [SerializeField] private Item item;
    public void Pick()
    {
        Actions.InventoryAddItem?.Invoke(item);
        Destroy(this.gameObject);
    }
}
