using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public sealed class RopeConnector : MonoBehaviour
{
    [Header("Rope Endpoints")]
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;

    [Header("Appearance")]
    [SerializeField, Min(0.001f)] private float thickness = 0.05f;
    [SerializeField] private Color color = Color.white;
    [SerializeField] private Material ropeMaterial;

    private LineRenderer lineRenderer;
    private Material generatedMaterial;

    private void Awake()
    {
        ApplyRope();
    }

    private void LateUpdate()
    {
        UpdatePositions();
    }

    private void OnValidate()
    {
        ApplyRope();
    }

    private void OnDestroy()
    {
        if (generatedMaterial == null)
        {
            return;
        }

        if (Application.isPlaying)
        {
            Destroy(generatedMaterial);
        }
        else
        {
            DestroyImmediate(generatedMaterial);
        }
    }

    [ContextMenu("Apply Rope")]
    public void ApplyRope()
    {
        if (!TryGetComponent(out lineRenderer))
        {
            return;
        }

        ApplyMaterial();

        lineRenderer.useWorldSpace = true;
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = thickness;
        lineRenderer.endWidth = thickness;
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;

        if (startPoint == null || endPoint == null)
        {
            lineRenderer.enabled = false;
            return;
        }

        UpdatePositions();
    }

    private void UpdatePositions()
    {
        if (lineRenderer == null)
        {
            return;
        }

        if (startPoint == null || endPoint == null)
        {
            lineRenderer.enabled = false;
            return;
        }

        lineRenderer.SetPosition(0, startPoint.position);
        lineRenderer.SetPosition(1, endPoint.position);
        lineRenderer.enabled = true;
    }

    private void ApplyMaterial()
    {
        if (ropeMaterial != null)
        {
            lineRenderer.sharedMaterial = ropeMaterial;
            return;
        }

        if (generatedMaterial == null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");

            if (shader == null)
            {
                shader = Shader.Find("Sprites/Default");
            }

            if (shader == null)
            {
                Debug.LogError("RopeConnector: 사용할 수 있는 Unlit 셰이더를 찾지 못했습니다.", this);
                return;
            }

            generatedMaterial = new Material(shader)
            {
                name = "Generated Rope Material",
                hideFlags = HideFlags.HideAndDontSave
            };
        }

        lineRenderer.sharedMaterial = generatedMaterial;
    }
}
