using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class SpriteScanEffect : MonoBehaviour
{
    private static readonly int ScanProgressId = Shader.PropertyToID("_ScanProgress");
    private static readonly int StartPixelCountId = Shader.PropertyToID("_StartPixelCount");
    private static readonly int SpriteUvRectId = Shader.PropertyToID("_SpriteUVRect");
    private static readonly int SpritePixelSizeId = Shader.PropertyToID("_SpritePixelSize");

    [SerializeField] private List<SpriteRenderer> targetRenderers = new();
    [SerializeField, Min(1f)] private float startPixelCount = 5f;
    [SerializeField, Min(0.01f)] private float restoreDuration = 1.5f;
    [SerializeField] private AnimationCurve restoreCurve =
        AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField] private bool playOnEnable;

    private MaterialPropertyBlock propertyBlock;
    private Coroutine scanCoroutine;
    private Action scanCompleted;

    private void Awake()
    {
        propertyBlock = new MaterialPropertyBlock();
    }

    private void OnEnable()
    {
        if (playOnEnable)
        {
            PlayScan();
        }
        else
        {
            SetProgress(1f);
        }
    }

    private void OnDisable()
    {
        if (scanCoroutine != null)
        {
            StopCoroutine(scanCoroutine);
            scanCoroutine = null;
        }

        scanCompleted = null;
    }

    [ContextMenu("Play Scan")]
    public void PlayScan()
    {
        PlayScan(null);
    }

    public void PlayScan(Action onCompleted)
    {
        if (scanCoroutine != null)
        {
            StopCoroutine(scanCoroutine);
        }

        scanCompleted = onCompleted;
        scanCoroutine = StartCoroutine(ScanRoutine());
    }

    [ContextMenu("Show Original")]
    public void ShowOriginal()
    {
        if (scanCoroutine != null)
        {
            StopCoroutine(scanCoroutine);
            scanCoroutine = null;
        }

        scanCompleted = null;
        SetProgress(1f);
    }

    private IEnumerator ScanRoutine()
    {
        float elapsedTime = 0f;
        SetProgress(0f);

        while (elapsedTime < restoreDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float normalizedTime = Mathf.Clamp01(elapsedTime / restoreDuration);
            SetProgress(restoreCurve.Evaluate(normalizedTime));
            yield return null;
        }

        SetProgress(1f);
        scanCoroutine = null;

        Action completed = scanCompleted;
        scanCompleted = null;
        completed?.Invoke();
    }

    private void SetProgress(float progress)
    {
        foreach (SpriteRenderer spriteRenderer in targetRenderers)
        {
            if (spriteRenderer == null || spriteRenderer.sprite == null)
            {
                continue;
            }

            Sprite sprite = spriteRenderer.sprite;
            Texture texture = sprite.texture;
            Rect textureRect = GetTextureRect(sprite);

            Vector4 uvRect = new(
                textureRect.xMin / texture.width,
                textureRect.yMin / texture.height,
                textureRect.width / texture.width,
                textureRect.height / texture.height);

            spriteRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetFloat(ScanProgressId, Mathf.Clamp01(progress));
            propertyBlock.SetFloat(StartPixelCountId, startPixelCount);
            propertyBlock.SetVector(SpriteUvRectId, uvRect);
            propertyBlock.SetVector(
                SpritePixelSizeId,
                new Vector4(textureRect.width, textureRect.height, 0f, 0f));
            spriteRenderer.SetPropertyBlock(propertyBlock);
        }
    }

    private static Rect GetTextureRect(Sprite sprite)
    {
        try
        {
            return sprite.textureRect;
        }
        catch (UnityException)
        {
            return sprite.rect;
        }
    }

    private void Reset()
    {
        targetRenderers.Clear();
        targetRenderers.AddRange(GetComponentsInChildren<SpriteRenderer>(true));
    }
}
