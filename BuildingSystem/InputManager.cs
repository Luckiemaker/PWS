using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{

    [SerializeField]
    private Camera sceneCamera;
    [SerializeField]
    private LayerMask placementLayermask;

    public event Action OnLeftClick, OnESC, OnR;

    private Vector3 lastPosition;


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))//left mousebutton
        {
            OnLeftClick?.Invoke();//? only executes if not null
        }
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            OnESC?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            OnR?.Invoke();
        }
    }

    public bool IsPointedOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();//return true or false if pointed over ui
    }
    public Vector3 GetMousePosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = sceneCamera.nearClipPlane;//we cannot select objects which are not rendered by camera
        Ray ray = sceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, placementLayermask))
        {
            lastPosition = hit.point;
        }
        return lastPosition;
    }
}
