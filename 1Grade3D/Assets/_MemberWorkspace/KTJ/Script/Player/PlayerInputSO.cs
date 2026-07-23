using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "PlayerInput", menuName = "KTJ/Input/Player Input")]
public sealed class PlayerInputSO : ScriptableObject
{
    public Vector2 MoveInput { get; private set; }

    private Controls controls;
    private int enableCount;

    public void EnableInput()
    {
        enableCount++;

        if (enableCount > 1)
        {
            return;
        }

        controls ??= new Controls();
        controls.Player.Move.performed += OnMove;
        controls.Player.Move.canceled += OnMove;
        controls.Player.Enable();
    }

    public void DisableInput()
    {
        enableCount = Mathf.Max(0, enableCount - 1);

        if (enableCount > 0 || controls == null)
        {
            return;
        }

        controls.Player.Move.performed -= OnMove;
        controls.Player.Move.canceled -= OnMove;
        controls.Player.Disable();
        MoveInput = Vector2.zero;
    }

    private void OnDisable()
    {
        enableCount = 0;

        if (controls == null)
        {
            return;
        }

        controls.Player.Move.performed -= OnMove;
        controls.Player.Move.canceled -= OnMove;
        if (Application.isPlaying)
        {
            controls.Dispose();
        }
        else if (controls.asset != null)
        {
            DestroyImmediate(controls.asset);
        }

        controls = null;
        MoveInput = Vector2.zero;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
    }
}
