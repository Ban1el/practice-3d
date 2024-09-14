using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Actions
{
    public static Action<UserInterface> OnEnableUI;
    public static Action<UserInterface> OnDisableUI;
    public static Action<bool> SetPlayerControl;

    public static Func<List<Item>> InventoryItemList;
    public static Action<Item> InventoryAddItem;
    public static Action<int> CheckUpdateInventory;
}
