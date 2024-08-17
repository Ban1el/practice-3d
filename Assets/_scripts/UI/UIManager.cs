using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UserInterface
{
    ItemPickIcon,
    CrossHair
}

public class UIManager : MonoBehaviour
{
    private List<UI> UserInteraces = new List<UI>();

    private void Awake()
    {
        UI[] controllers = GetComponentsInChildren<UI>(true);
        UserInteraces.AddRange(controllers);
    }

    private void Start()
    {
        DisableUIAll();
    }

    private void TurnOnUI(UserInterface ui)
    {
        foreach (UI controller in UserInteraces)
        {
            if (controller.userInterface == ui)
            {
                controller.gameObject.SetActive(true);
            }
        }
    }

    private void TurnOffUI(UserInterface ui)
    {
        foreach (UI controller in UserInteraces)
        {
            if (controller.userInterface == ui)
            {
                controller.gameObject.SetActive(false);
            }
        }
    }

    private void DisableUIAll()
    {
        foreach (UI controller in UserInteraces)
        {
            controller.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        Actions.OnEnableUI += TurnOnUI;
        Actions.OnDisableUI += TurnOffUI;
    }

    private void OnDisable()
    {
        Actions.OnEnableUI -= TurnOnUI;
        Actions.OnDisableUI -= TurnOffUI;
    }
}
