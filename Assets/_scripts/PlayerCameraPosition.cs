using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraPosition : MonoBehaviour
{
    [SerializeField] private Transform _cameraPosition;

    void Update()
    {
        transform.position = _cameraPosition.position;
    }
}
