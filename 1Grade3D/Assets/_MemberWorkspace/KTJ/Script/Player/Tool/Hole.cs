using System.Collections.Generic;
using UnityEngine;

public sealed class Hole : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform rotatingObject;

    [Header("Rotation")]
    [SerializeField] private float openedXAngle = -40f;
    [SerializeField] private float closedXAngle = 90f;
    [SerializeField, Min(0f)] private float rotationSpeed = 180f;

    private readonly Dictionary<GameObject, int> detectedPlayers = new();
    private Vector3 baseEulerAngles;
    private float targetXAngle;

    private void Awake()
    {
        if (rotatingObject == null)
        {
            Debug.LogError("Hole: Rotating Object가 지정되지 않았습니다.", this);
            enabled = false;
            return;
        }

        baseEulerAngles = rotatingObject.localEulerAngles;
        targetXAngle = closedXAngle;
        SetRotationImmediately(closedXAngle);
    }

    private void Update()
    {
        if (rotatingObject == null)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.Euler(
            targetXAngle,
            baseEulerAngles.y,
            baseEulerAngles.z);

        rotatingObject.localRotation = Quaternion.RotateTowards(
            rotatingObject.localRotation,
            targetRotation,
            rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject player = FindPlayerObject(other);

        if (player == null)
        {
            return;
        }

        detectedPlayers.TryGetValue(player, out int contactCount);
        detectedPlayers[player] = contactCount + 1;
        targetXAngle = openedXAngle;
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject player = FindPlayerObject(other);

        if (player == null ||
            !detectedPlayers.TryGetValue(player, out int contactCount))
        {
            return;
        }

        if (contactCount <= 1)
        {
            detectedPlayers.Remove(player);
        }
        else
        {
            detectedPlayers[player] = contactCount - 1;
        }

        if (detectedPlayers.Count == 0)
        {
            targetXAngle = closedXAngle;
        }
    }

    private void OnDisable()
    {
        detectedPlayers.Clear();
        targetXAngle = closedXAngle;
    }

    private void SetRotationImmediately(float xAngle)
    {
        rotatingObject.localRotation = Quaternion.Euler(
            xAngle,
            baseEulerAngles.y,
            baseEulerAngles.z);
    }

    private static GameObject FindPlayerObject(Collider other)
    {
        Transform current = other.transform;

        while (current != null)
        {
            if (current.CompareTag("Player"))
            {
                return current.gameObject;
            }

            current = current.parent;
        }

        return null;
    }
}
