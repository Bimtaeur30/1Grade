using _MemberWorkspace.JJH._02_Scripts.Map;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "PlayerInput", menuName = "KTJ/Input/Player Input")]
public sealed class PlayerInputSO : ScriptableObject
{
    [SerializeField] private LayerMask itemLayer;

    public Vector2 MoveInput { get; private set; }

    private Controls controls;
    private int enableCount;

    private Vector2 mousePosition;
    private Camera mainCamera;

    public Camera MainCamera
    {
        get
        {
            if (mainCamera == null)
                mainCamera = Camera.main;

            return mainCamera;
        }
    }

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
        controls.Player.MousePos.performed += OnMousePos;
        controls.Player.MousePos.canceled += OnMousePos;
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
        controls.Player.MousePos.performed -= OnMousePos;
        controls.Player.MousePos.canceled -= OnMousePos;
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

    private void OnMousePos(InputAction.CallbackContext context)
    {
        mousePosition = context.ReadValue<Vector2>();
    }

    public GroundTile GetGroundTile()
    {
        Ray ray = MainCamera.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, MainCamera.farClipPlane, itemLayer))
        {
            GroundTile tile = hit.collider.GetComponent<GroundTile>();
            return tile;
        }

        return null;
    }
}
