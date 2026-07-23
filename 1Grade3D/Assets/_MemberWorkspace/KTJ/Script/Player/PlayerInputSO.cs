using _MemberWorkspace.JJH._02_Scripts.Map;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "PlayerInput", menuName = "KTJ/Input/Player Input")]
public sealed class PlayerInputSO : ScriptableObject, Controls.IPlayerActions
{
    [SerializeField] private LayerMask itemLayer;

    public Vector2 MoveInput { get; private set; }
    public Vector2 MousePosition => mousePosition;

    private Controls _control;

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

    private void OnEnable()
    {
        if (_control == null)
        {
            _control = new Controls();
            _control.Player.SetCallbacks(this);
        }

        _control.Player.Enable();
    }

    private void OnDisable()
    {
        if (_control != null)
            _control.Player.Disable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
    }

    public void OnMousePos(InputAction.CallbackContext context)
    {
        mousePosition = context.ReadValue<Vector2>();
    }

    public GroundTile GetGroundTile()
    {
        Ray ray = MainCamera.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, MainCamera.farClipPlane, itemLayer))
        {
            GroundTile tile = hit.collider.GetComponentInParent<GroundTile>();
            if (tile.HasItem)
                return tile;
        }

        return null;
    }
}
