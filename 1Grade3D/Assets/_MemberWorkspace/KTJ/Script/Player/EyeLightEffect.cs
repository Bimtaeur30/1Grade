using GameLib.EventChannelSystem;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EyeLightEffect : MonoBehaviour
{
    [SerializeField] private EventChannelSO PlayerChannel;
    [SerializeField] private GameObject LightObject;
    [SerializeField] private Volume GlobalVolume;

    private Vignette vignette;
    private FilmGrain filmGrain;

    private void Awake()
    {
        if (GlobalVolume == null)
        {
            GlobalVolume = FindFirstObjectByType<Volume>();
        }

        CacheVolumeOverrides();
        ApplyScannerVolume(false);
        PlayerChannel.AddListener<ScannerEvent>(HandleScannerEvent);
    }
    private void OnDestroy()
    {
        PlayerChannel.RemoveListener<ScannerEvent>(HandleScannerEvent);
    }
    private void HandleScannerEvent(ScannerEvent @event)
    {
        LightObject.SetActive(@event.IsStart);
        ApplyScannerVolume(@event.IsStart);
    }

    private void CacheVolumeOverrides()
    {
        if (GlobalVolume == null || GlobalVolume.profile == null)
        {
            Debug.LogWarning("EyeLightEffect: Global Volume을 찾지 못했습니다.", this);
            return;
        }

        GlobalVolume.profile.TryGet(out vignette);
        GlobalVolume.profile.TryGet(out filmGrain);
    }

    private void ApplyScannerVolume(bool isScanning)
    {
        if (vignette != null)
        {
            vignette.active = true;
            vignette.color.overrideState = true;
            vignette.intensity.overrideState = true;
            vignette.color.value = isScanning
                ? new Color(0.41037738f, 1f, 0.4473691f, 1f)
                : Color.black;
            vignette.intensity.value = isScanning ? 0.599f : 0.383f;
        }

        if (filmGrain != null)
        {
            filmGrain.active = isScanning;
            filmGrain.type.overrideState = true;
            filmGrain.intensity.overrideState = true;
            filmGrain.response.overrideState = true;
            filmGrain.type.value = FilmGrainLookup.Large02;
            filmGrain.intensity.value = 0.889f;
            filmGrain.response.value = 0.8f;
        }
    }
}
