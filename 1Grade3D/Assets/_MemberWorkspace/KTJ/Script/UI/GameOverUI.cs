using System.Collections;
using GameLib.EventChannelSystem;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public sealed class GameOverUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private EventChannelSO turnChannel;

    [Header("Fade")]
    [SerializeField, Min(0f)] private float fadeDuration = 0.75f;
    [SerializeField] private AnimationCurve fadeCurve =
        AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private Coroutine fadeCoroutine;

    private void Awake()
    {
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        HideImmediately();
    }

    private void OnEnable()
    {
        if (turnChannel == null)
        {
            Debug.LogError("GameOverUI: Turn Channel이 지정되지 않았습니다.", this);
            return;
        }

        turnChannel.AddListener<TurnEndEvent>(HandleTurnEnd);
    }

    private void OnDisable()
    {
        if (turnChannel != null)
        {
            turnChannel.RemoveListener<TurnEndEvent>(HandleTurnEnd);
        }

        StopFade();
    }

    private void HandleTurnEnd(TurnEndEvent turnEndEvent)
    {
        Show();
    }

    public void Show()
    {
        StopFade();

        if (!isActiveAndEnabled)
        {
            return;
        }

        fadeCoroutine = StartCoroutine(FadeInRoutine());
    }

    public void HideImmediately()
    {
        StopFade();
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    private IEnumerator FadeInRoutine()
    {
        float startAlpha = canvasGroup.alpha;

        if (fadeDuration <= 0f)
        {
            CompleteFade();
            yield break;
        }

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float normalizedTime = Mathf.Clamp01(elapsedTime / fadeDuration);
            canvasGroup.alpha = Mathf.Lerp(
                startAlpha,
                1f,
                fadeCurve.Evaluate(normalizedTime));
            yield return null;
        }

        CompleteFade();
    }

    private void CompleteFade()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        fadeCoroutine = null;
    }

    private void StopFade()
    {
        if (fadeCoroutine == null)
        {
            return;
        }

        StopCoroutine(fadeCoroutine);
        fadeCoroutine = null;
    }
}
