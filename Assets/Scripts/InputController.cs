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
#endif

    public void HandleMouseInput()
    {
#if UNITY_EDITOR
        EditorInput();
#else
       AndroidInput();
#endif
    }
    
#if UNITY_EDITOR
    private void EditorInput()
    {
        // Handle mouse input for the editor
        if (IsPointerOverUIElement())
        {
            canMove = false;
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
    }

#else

    public void AndroidInput()
    {
        // Handle touch input for mobile devices
        if (Input.touchCount == 0) return;

        var touch = Input.GetTouch(0);

        if (IsPointerOverUIElement(touch))
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
    }
#endif

#if UNITY_EDITOR
// This method will check if the mouse is over a UI element in the editor
    private bool IsPointerOverUIElement()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
#else
    // This method checks if a touch is over a UI element on mobile devices
    private bool IsPointerOverUIElement(Touch touch)
    {
        return EventSystem.current.IsPointerOverGameObject(touch.fingerId);
    }
#endif
    
}