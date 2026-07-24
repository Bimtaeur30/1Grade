using TMPro;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(TMP_Text))]
public sealed class CircularScrollingText : MonoBehaviour
{
    [Header("Circle")]
    [SerializeField, Min(0.01f)] private float radius = 2f;
    [SerializeField] private float startAngle = 90f;
    [SerializeField, Min(0.1f)] private float characterSpacing = 12f;
    [SerializeField] private float oppositeAngle = 180f;

    [Header("Rotation")]
    [Tooltip("양수는 반시계 방향, 음수는 시계 방향입니다.")]
    [SerializeField] private float rotationSpeed = 30f;
    [SerializeField] private bool useUnscaledTime = true;

    private TMP_Text targetText;
    private float rotationOffset;
    [SerializeField, HideInInspector] private string sourceText;
    [SerializeField, HideInInspector] private string generatedText;

    private void Awake()
    {
        CacheText();
    }

    private void OnEnable()
    {
        CacheText();
        UpdateCircularText();
    }

    private void OnDisable()
    {
        if (targetText != null && !string.IsNullOrEmpty(sourceText))
        {
            targetText.text = sourceText;
        }

    }

    private void Update()
    {
        if (Application.isPlaying)
        {
            float deltaTime = useUnscaledTime
                ? Time.unscaledDeltaTime
                : Time.deltaTime;

            rotationOffset = Mathf.Repeat(
                rotationOffset + rotationSpeed * deltaTime,
                360f);
        }

        UpdateCircularText();
    }

    [ContextMenu("Reset Rotation")]
    public void ResetRotation()
    {
        rotationOffset = 0f;
        UpdateCircularText();
    }

    private void CacheText()
    {
        if (targetText == null)
        {
            targetText = GetComponent<TMP_Text>();
        }
    }

    private void UpdateCircularText()
    {
        CacheText();

        if (targetText == null)
        {
            return;
        }

        EnsureDuplicatedText();
        targetText.ForceMeshUpdate();
        TMP_TextInfo textInfo = targetText.textInfo;
        int characterCount = textInfo.characterCount;

        if (characterCount < 2)
        {
            return;
        }

        int charactersPerCopy = characterCount / 2;

        for (int i = 0; i < characterCount; i++)
        {
            TMP_CharacterInfo characterInfo = textInfo.characterInfo[i];

            if (!characterInfo.isVisible)
            {
                continue;
            }

            int materialIndex = characterInfo.materialReferenceIndex;
            int vertexIndex = characterInfo.vertexIndex;
            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

            Vector3 characterCenter =
                (vertices[vertexIndex] + vertices[vertexIndex + 2]) * 0.5f;

            int copyIndex = i < charactersPerCopy ? 0 : 1;
            int characterIndex = i - copyIndex * charactersPerCopy;
            float centeredCharacterIndex =
                characterIndex - (charactersPerCopy - 1) * 0.5f;

            float angle = startAngle + rotationOffset +
                copyIndex * oppositeAngle +
                -characterSpacing * centeredCharacterIndex;
            float angleRadians = angle * Mathf.Deg2Rad;

            Vector3 circlePosition = new Vector3(
                Mathf.Cos(angleRadians) * radius,
                Mathf.Sin(angleRadians) * radius,
                0f);

            // 글자의 위쪽이 항상 원 중심을 향하게 합니다.
            float characterRotation = angle + 90f;

            Quaternion rotation =
                Quaternion.Euler(0f, 0f, characterRotation);

            for (int vertex = 0; vertex < 4; vertex++)
            {
                int currentVertexIndex = vertexIndex + vertex;
                Vector3 localVertex =
                    vertices[currentVertexIndex] - characterCenter;
                localVertex.x = -localVertex.x;
                vertices[currentVertexIndex] =
                    rotation * localVertex + circlePosition;
            }
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            TMP_MeshInfo meshInfo = textInfo.meshInfo[i];

            if (meshInfo.mesh == null)
            {
                continue;
            }

            meshInfo.mesh.vertices = meshInfo.vertices;
            targetText.UpdateGeometry(meshInfo.mesh, i);
        }
    }

    private void EnsureDuplicatedText()
    {
        string currentText = targetText.text;

        if (string.IsNullOrEmpty(sourceText))
        {
            sourceText = CollapseRepeatedHalves(currentText);
        }
        else if (currentText != generatedText &&
                 currentText != sourceText)
        {
            sourceText = currentText;
        }

        generatedText = sourceText + sourceText;

        if (targetText.text != generatedText)
        {
            targetText.text = generatedText;
        }
    }

    [ContextMenu("Repair Duplicated Text")]
    private void RepairDuplicatedText()
    {
        CacheText();

        if (targetText == null)
        {
            return;
        }

        sourceText = CollapseRepeatedHalves(targetText.text);
        generatedText = sourceText + sourceText;
        targetText.text = generatedText;
        UpdateCircularText();
    }

    private static string CollapseRepeatedHalves(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        string collapsedText = text;

        while (collapsedText.Length % 2 == 0)
        {
            int halfLength = collapsedText.Length / 2;
            string firstHalf = collapsedText.Substring(0, halfLength);
            string secondHalf = collapsedText.Substring(halfLength);

            if (firstHalf != secondHalf)
            {
                break;
            }

            collapsedText = firstHalf;
        }

        return collapsedText;
    }

    private void OnValidate()
    {
        radius = Mathf.Max(0.01f, radius);
        characterSpacing = Mathf.Max(0.1f, characterSpacing);

        if (isActiveAndEnabled)
        {
            UpdateCircularText();
        }
    }
}
