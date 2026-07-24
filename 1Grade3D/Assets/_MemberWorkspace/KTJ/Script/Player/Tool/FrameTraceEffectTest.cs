using UnityEngine;
using UnityEngine.InputSystem;

public sealed class FrameTraceEffectTest : MonoBehaviour
{
    [SerializeField] private FrameTraceEffect frameTraceEffect;

    private void Awake()
    {
        if (frameTraceEffect == null)
        {
            frameTraceEffect = GetComponent<FrameTraceEffect>();
        }
    }

    private void Update()
    {
        if (frameTraceEffect != null &&
            Keyboard.current != null &&
            Keyboard.current.tKey.wasPressedThisFrame)
        {
            frameTraceEffect.Play();
        }
    }
}
