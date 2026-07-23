using System;
using UnityEngine;

public sealed class FollowTarget : MonoBehaviour
{
    [Flags]
    private enum FixedAxis
    {
        None = 0,
        X = 1 << 0,
        Y = 1 << 1,
        Z = 1 << 2
    }

    [SerializeField] private Transform target;
    [SerializeField] private FixedAxis fixedAxes;

    private Vector3 fixedPosition;

    private void OnEnable()
    {
        fixedPosition = transform.position;
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        Vector3 targetPosition = target.position;

        transform.position = new Vector3(
            IsFixed(FixedAxis.X) ? fixedPosition.x : targetPosition.x,
            IsFixed(FixedAxis.Y) ? fixedPosition.y : targetPosition.y,
            IsFixed(FixedAxis.Z) ? fixedPosition.z : targetPosition.z);
    }

    [ContextMenu("Set Current Position As Fixed Position")]
    private void SetCurrentPositionAsFixedPosition()
    {
        fixedPosition = transform.position;
    }

    private bool IsFixed(FixedAxis axis)
    {
        return (fixedAxes & axis) != 0;
    }
}
