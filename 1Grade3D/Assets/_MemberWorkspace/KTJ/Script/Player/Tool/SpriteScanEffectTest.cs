using UnityEngine;
using UnityEngine.InputSystem;

public sealed class SpriteScanEffectTest : MonoBehaviour
{
    [SerializeField] private SpriteScanEffect spriteScanEffect;

    private void Awake()
    {
        if (spriteScanEffect == null)
        {
            spriteScanEffect = GetComponent<SpriteScanEffect>();
        }
    }

    private void Update()
    {
        if (spriteScanEffect != null &&
            Keyboard.current != null &&
            Keyboard.current.tKey.wasPressedThisFrame)
        {
            spriteScanEffect.PlayScan();
        }
    }
}
