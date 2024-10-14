using System;
using UnityEngine;
using UnityEngine.EventSystems;

[Serializable]
public class InputController
{
    public bool isMouseDown;
    public bool canMove;
    
#if UNITY_EDITOR
    private bool IsMouseButtonDown() => Input.GetMouseButtonDown(0);
    private bool IsMouseHeld() => Input.GetMouseButton(0);
    private bool IsMouseButtonUp() => Input.GetMouseButtonUp(0);
    public float IsMouseX() => Input.GetAxis("Mouse X");
#else
    private bool IsMouseButtonDown(Touch touch) => touch.phase == TouchPhase.Began;
    private bool IsMouseHeld(Touch touch) => touch.phase == TouchPhase.Moved;
    private bool IsMouseButtonUp(Touch touch) => touch.phase == TouchPhase.Ended;
    public float IsMouseX(Touch touch) => touch.deltaPosition.x;

    public Touch GetTouch() => Input.GetTouch(0);
#endif

    public void HandleMouseInput()
    {
#if UNITY_EDITOR
        if (IsPointerOverUIElement()) // PC handling UI check
        {
            canMove = false;
            isMouseDown = false;
            return;
        }

        if (IsMouseButtonDown())
        {
            canMove = false;
            isMouseDown = true;
        }

        if (IsMouseHeld() && isMouseDown)
        {
            canMove = true;
        }

        if (IsMouseButtonUp())
        {
            canMove = false;
            isMouseDown = false;
        }
#else
        if (Input.touchCount == 0) return;
        var touch = GetTouch();

        if (IsPointerOverUIElement(touch)) // Mobile UI check
        {
            canMove = false;
            isMouseDown = false;
            return;
        }

        if (IsMouseButtonDown(touch))
        {
            canMove = false;
            isMouseDown = true;
        }

        if (IsMouseHeld(touch) && isMouseDown)
        {
            canMove = true;
        }

        if (IsMouseButtonUp(touch))
        {
            canMove = false;
            isMouseDown = false;
        }
#endif
    }

#if UNITY_EDITOR
    private bool IsPointerOverUIElement()
    {
        return EventSystem.current.IsPointerOverGameObject(); // Check for UI element under mouse in editor
    }
#else
    private bool IsPointerOverUIElement(Touch touch)
    {
        return EventSystem.current.IsPointerOverGameObject(touch.fingerId); // Check for UI element under touch on mobile
    }
#endif

}