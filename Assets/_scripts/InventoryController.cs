using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    private List<Transform> gridElements = new List<Transform>();

    private void Awake()
    {
        Transform[] elements = GetComponentsInChildren<Transform>(true); 
        foreach (Transform element in elements)
        {
            gridElements.Add(element); 
        }
    }

    private void UpdateInventory()
    {
        for (int i = 0; i < Actions.InventoryItemList().Count; i++)
        {
            Debug.Log(Actions.InventoryItemList()[i].name);
            //Image iconFrame = gridElements[i].GetChild(0).transform.GetComponent<Image>();
            //iconFrame.enabled = true;
            //iconFrame.sprite = Actions.InventoryItemList()[i].icon;
        }
    }

    private void OnEnable()
    {
        UpdateInventory();
    }
}
