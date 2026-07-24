using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public sealed class FrameTraceEffect : MonoBehaviour
{
    private static readonly int ProgressId = Shader.PropertyToID("_Progress");

    [SerializeField] private Image targetImage;
    [SerializeField, Min(0.01f)] private float duration = 1.2f;
    [SerializeField] private AnimationCurve progressCurve =
        AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField] private bool playOnEnable;

    public bool IsPlaying => playCoroutine != null;

    private Material runtimeMaterial;
    private Coroutine playCoroutine;
    private Action completedCallback;

    private void Awake()
    {
        if (targetImage == null)
        {
            targetImage = GetComponent<Image>();
        }

        if (targetImage.material != null)
        {
            runtimeMaterial = Instantiate(targetImage.material);
            runtimeMaterial.name = $"{targetImage.material.name} (Instance)";
            targetImage.material = runtimeMaterial;
        }

        targetImage.raycastTarget = false;
        ApplyProgress(0f);
    }

    private void OnEnable()
    {
        if (playOnEnable)
        {
            Play();
        }
    }

    private void OnDestroy()
    {
        completedCallback = null;

        if (runtimeMaterial != null)
        {
            Destroy(runtimeMaterial);
        }
    }

    [ContextMenu("Play")]
    public void Play()
    {
        Play(null);
    }

    public void Play(Action onCompleted)
    {
        StopCurrent();
        completedCallback = onCompleted;
        playCoroutine = StartCoroutine(PlayRoutine());
    }

    [ContextMenu("Reset Effect")]
    public void ResetEffect()
    {
        StopCurrent();
        completedCallback = null;
        ApplyProgress(0f);
    }

    public void SetProgress(float progress)
    {
        StopCurrent();
        completedCallback = null;
        ApplyProgress(progress);
    }

    private IEnumerator PlayRoutine()
    {
        float elapsedTime = 0f;
        ApplyProgress(0f);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float normalizedTime = Mathf.Clamp01(elapsedTime / duration);
            ApplyProgress(progressCurve.Evaluate(normalizedTime));
            yield return null;
        }

        ApplyProgress(1f);
        playCoroutine = null;

        Action callback = completedCallback;
        completedCallback = null;
        callback?.Invoke();
    }

    private void ApplyProgress(float progress)
    {
        if (runtimeMaterial != null)
        {
            runtimeMaterial.SetFloat(ProgressId, Mathf.Clamp01(progress));
        }
    }

    private void StopCurrent()
    {
        if (playCoroutine == null)
        {
            return;
        }

        StopCoroutine(playCoroutine);
        playCoroutine = null;
    }
}
