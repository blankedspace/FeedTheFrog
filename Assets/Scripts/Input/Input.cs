
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Input : MonoBehaviour
{
    private TouchControls _TouchControls;

    public delegate void processInput(Vector2 pos);
    public processInput ProcessTap;

    public delegate void processDrag(Vector2 pos,bool first);
    public processDrag ProcessDrag;


    public delegate void quickDrag(Vector2 startpos, Vector2 pos);
    public quickDrag QuickDrag;

    private void Update()
    {
        if (_TouchControls.Touchs.TouchDrag.IsPressed())
        {
            if (ProcessDrag != null)
            {
                ProcessDrag(_TouchControls.Touchs.TouchPosition.ReadValue<Vector2>(), false);
            }
        }
    }

    private void Awake()
    {
        _TouchControls = new TouchControls();
    }
    private void OnEnable()
    {
        _TouchControls.Enable();
    }
    private void OnDisable()
    {
        _TouchControls.Disable();
    }
    void Start()
    {

        _TouchControls.Touchs.TouchPress.started += ctx => StartTouch(ctx);
        _TouchControls.Touchs.TouchDrag.started += ctx => StartDrag(ctx);
        _TouchControls.Touchs.TouchDrag.canceled += ctx => DragEneded(ctx);
        
    }

    private void DragEneded(InputAction.CallbackContext ctx)
    {
     
        if (QuickDrag != null)
        {
            QuickDrag(_TouchControls.Touchs.DragEnded.ReadValue<Vector2>(), _TouchControls.Touchs.TouchPosition.ReadValue<Vector2>());
        }
    }

    private void StartDrag(InputAction.CallbackContext ctx)
    {
        if (ProcessDrag != null)
        {
            ProcessDrag(_TouchControls.Touchs.TouchPosition.ReadValue<Vector2>(), true);
        }
    }

    private void StartTouch(InputAction.CallbackContext ctx)
    {
       
        if (ProcessTap != null)
        {
            ProcessTap(_TouchControls.Touchs.TouchPosition.ReadValue<Vector2>());
        }
    }
}
