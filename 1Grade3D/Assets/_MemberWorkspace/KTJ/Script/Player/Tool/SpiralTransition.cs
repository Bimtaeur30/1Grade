using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public sealed class SpiralTransition : MonoBehaviour
{
    private static readonly int ProgressId = Shader.PropertyToID("_Progress");

    [SerializeField] private Image transitionImage;
    [SerializeField, Min(0.01f)] private float duration = 1f;
    [SerializeField] private AnimationCurve animationCurve =
        AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    public bool IsPlaying => transitionCoroutine != null;
    public float Progress { get; private set; }

    private Material runtimeMaterial;
    private Coroutine transitionCoroutine;

    private void Awake()
    {
        if (transitionImage == null)
        {
            transitionImage = GetComponent<Image>();
        }

        if (transitionImage.material != null)
        {
            runtimeMaterial = Instantiate(transitionImage.material);
            runtimeMaterial.name = $"{transitionImage.material.name} (Instance)";
            transitionImage.material = runtimeMaterial;
        }

        transitionImage.raycastTarget = false;
        SetProgress(0f);
    }

    private void OnDestroy()
    {
        if (runtimeMaterial != null)
        {
            Destroy(runtimeMaterial);
        }
    }

    [ContextMenu("Play Close")]
    public void PlayClose()
    {
        Play(Progress, 1f);
    }

    [ContextMenu("Play Open")]
    public void PlayOpen()
    {
        Play(Progress, 0f);
    }

    public void SetClosed()
    {
        StopCurrentTransition();
        SetProgress(1f);
    }

    public void SetOpened()
    {
        StopCurrentTransition();
        SetProgress(0f);
    }

    private void Play(float startProgress, float endProgress)
    {
        StopCurrentTransition();
        transitionCoroutine = StartCoroutine(
            TransitionRoutine(startProgress, endProgress));
    }

    private IEnumerator TransitionRoutine(float startProgress, float endProgress)
    {
        float elapsedTime = 0f;
        float distance = Mathf.Abs(endProgress - startProgress);
        float adjustedDuration = Mathf.Max(0.01f, duration * distance);

        while (elapsedTime < adjustedDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float normalizedTime = Mathf.Clamp01(elapsedTime / adjustedDuration);
            float curvedTime = animationCurve.Evaluate(normalizedTime);
            SetProgress(Mathf.Lerp(startProgress, endProgress, curvedTime));
            yield return null;
        }

        SetProgress(endProgress);
        transitionCoroutine = null;
    }

    private void SetProgress(float progress)
    {
        Progress = Mathf.Clamp01(progress);

        if (runtimeMaterial != null)
        {
            runtimeMaterial.SetFloat(ProgressId, Progress);
        }
    }

    private void StopCurrentTransition()
    {
        if (transitionCoroutine == null)
        {
            return;
        }

        StopCoroutine(transitionCoroutine);
        transitionCoroutine = null;
    }
}
