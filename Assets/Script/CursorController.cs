using UnityEngine;

public class CursorController : MonoBehaviour
{
    private bool _cursorLocked;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _cursorLocked = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            SwitchMouseCursorMode();
        }
    }

    public void SwitchMouseCursorMode()
    {
        if (!_cursorLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            _cursorLocked = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            _cursorLocked = false;
        }
    }
}
