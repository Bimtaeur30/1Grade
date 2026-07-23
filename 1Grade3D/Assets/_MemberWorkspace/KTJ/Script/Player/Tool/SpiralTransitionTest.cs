using UnityEngine;
using UnityEngine.InputSystem;

public sealed class SpiralTransitionTest : MonoBehaviour
{
    [SerializeField] private SpiralTransition spiralTransition;

    private void Awake()
    {
        if (spiralTransition == null)
        {
            spiralTransition = GetComponent<SpiralTransition>();
        }
    }

    private void Update()
    {
        if (spiralTransition == null ||
            spiralTransition.IsPlaying ||
            Keyboard.current == null ||
            !Keyboard.current.tKey.wasPressedThisFrame)
        {
            return;
        }

        if (spiralTransition.Progress < 0.5f)
        {
            spiralTransition.PlayClose();
        }
        else
        {
            spiralTransition.PlayOpen();
        }
    }
}
