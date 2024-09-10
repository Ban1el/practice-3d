using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraMovement : MonoBehaviour
{
    [SerializeField] private float xSens;
    [SerializeField] private float ySens;
    [SerializeField] private Transform _orientation;
    [SerializeField] private bool canMove = true;

    private float xRotation;
    private float yRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (canMove)
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * xSens;
            float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * ySens;

            yRotation += mouseX;
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
            _orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        }
    }

    private void SetControl(bool _canMove)
    {
        canMove = _canMove;
    }

    private void OnEnable()
    {
        Actions.SetPlayerControl += SetControl;
    }

    private void OnDisable()
    {
         Actions.SetPlayerControl -= SetControl;
    }
}
