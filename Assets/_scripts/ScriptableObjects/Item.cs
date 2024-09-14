using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class Item : ScriptableObject
{
    #region Details
    public new string name;
    public string description;
    public Sprite icon;
    private string itemID = System.Guid.NewGuid().ToString();
    #endregion

    #region Inventory Variables
    public bool isStored = false;
    public int itemSlot = -1;
    #endregion

    public string GetItemID()
    {
        return itemID;
    }
}
