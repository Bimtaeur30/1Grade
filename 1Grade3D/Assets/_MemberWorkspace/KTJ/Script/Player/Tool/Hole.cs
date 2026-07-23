using System.Collections.Generic;
using GameLib.EventChannelSystem;
using UnityEngine;

public sealed class Hole : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform rotatingObject;
    [SerializeField] private FrameTraceEffect frameTraceEffect;
    [SerializeField] private EventChannelSO turnChannel;

    [Header("Rotation")]
    [SerializeField] private float openedXAngle = -40f;
    [SerializeField] private float closedXAngle = 90f;
    [SerializeField, Min(0f)] private float rotationSpeed = 180f;

    private readonly Dictionary<GameObject, int> detectedPlayers = new();
    private Vector3 baseEulerAngles;
    private float targetXAngle;
    private bool hasStartedFrameTrace;

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

        if (detectedPlayers.Count > 0 &&
            !hasStartedFrameTrace &&
            Quaternion.Angle(rotatingObject.localRotation, targetRotation) <= 0.1f)
        {
            StartFrameTrace();
        }
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
            ResetFrameTrace();
        }
    }

    private void OnDisable()
    {
        detectedPlayers.Clear();
        targetXAngle = closedXAngle;
        ResetFrameTrace();
    }

    private void StartFrameTrace()
    {
        if (frameTraceEffect == null)
        {
            Debug.LogError("Hole: Frame Trace Effect가 지정되지 않았습니다.", this);
            return;
        }

        hasStartedFrameTrace = true;
        frameTraceEffect.Play(HandleFrameTraceCompleted);
    }

    private void HandleFrameTraceCompleted()
    {
        if (!hasStartedFrameTrace || detectedPlayers.Count == 0)
        {
            return;
        }

        if (turnChannel == null)
        {
            Debug.LogError("Hole: Turn Channel이 지정되지 않았습니다.", this);
            return;
        }

        turnChannel.RaiseEvent(TurnEvents.TurnEndEvent);
    }

    private void ResetFrameTrace()
    {
        hasStartedFrameTrace = false;

        if (frameTraceEffect != null)
        {
            frameTraceEffect.ResetEffect();
        }
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
