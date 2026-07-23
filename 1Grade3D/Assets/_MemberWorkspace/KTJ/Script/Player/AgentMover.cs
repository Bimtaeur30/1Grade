using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public sealed class AgentMover : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private PlayerInputSO playerInput;

    [Header("Movement")]
    [SerializeField, Min(0f)] private float moveSpeed = 5f;

    [Header("Direction Visuals")]
    [SerializeField] private SpriteRenderer bodySpriteRenderer;
    [SerializeField] private List<Transform> directionRotationTargets = new();

    [Header("Effects")]
    [SerializeField] private ParticleSystem moveParticle;

    public bool HasMoveInput => MoveInput.sqrMagnitude > 0.0001f;
    public Vector2 MoveInput => playerInput == null ? Vector2.zero : playerInput.MoveInput;

    private Rigidbody rigidbodyComponent;
    private Quaternion[] initialRotations = Array.Empty<Quaternion>();
    private bool movementEnabled = true;
    private bool isFacingRight;

    private void Awake()
    {
        rigidbodyComponent = GetComponent<Rigidbody>();
        CacheInitialRotations();
        ApplyFacing(false, true);
    }

    private void OnEnable()
    {
        playerInput?.EnableInput();
    }

    private void OnDisable()
    {
        playerInput?.DisableInput();
        StopMoveParticle();
    }

    private void Update()
    {
        Vector2 input = MoveInput;

        if (movementEnabled && Mathf.Abs(input.x) > 0.0001f)
        {
            ApplyFacing(input.x > 0f);
        }

        UpdateMoveParticle(movementEnabled && HasMoveInput);
    }

    private void FixedUpdate()
    {
        if (!movementEnabled || playerInput == null)
        {
            return;
        }

        Vector2 input = Vector2.ClampMagnitude(playerInput.MoveInput, 1f);
        Vector3 movement = new(input.x, 0f, input.y);
        rigidbodyComponent.MovePosition(
            rigidbodyComponent.position + movement * (moveSpeed * Time.fixedDeltaTime));
    }

    public void SetMovementEnabled(bool enabled)
    {
        movementEnabled = enabled;

        if (!enabled)
        {
            StopHorizontalMovement();
            StopMoveParticle();
        }
    }

    public void SetPlayerInput(PlayerInputSO input)
    {
        playerInput = input;
    }

    private void StopHorizontalMovement()
    {
        Vector3 velocity = rigidbodyComponent.linearVelocity;
        rigidbodyComponent.linearVelocity = new Vector3(0f, velocity.y, 0f);
    }

    private void CacheInitialRotations()
    {
        initialRotations = new Quaternion[directionRotationTargets.Count];

        for (int i = 0; i < directionRotationTargets.Count; i++)
        {
            Transform target = directionRotationTargets[i];
            initialRotations[i] = target == null ? Quaternion.identity : target.localRotation;
        }
    }

    private void ApplyFacing(bool faceRight, bool force = false)
    {
        if (!force && isFacingRight == faceRight)
        {
            return;
        }

        isFacingRight = faceRight;

        if (bodySpriteRenderer != null)
        {
            bodySpriteRenderer.flipX = faceRight;
        }

        for (int i = 0; i < directionRotationTargets.Count; i++)
        {
            Transform target = directionRotationTargets[i];

            if (target == null)
            {
                continue;
            }

            Quaternion initialRotation =
                i < initialRotations.Length ? initialRotations[i] : target.localRotation;
            target.localRotation = faceRight
                ? initialRotation * Quaternion.Euler(0f, 180f, 0f)
                : initialRotation;
        }
    }

    private void UpdateMoveParticle(bool shouldPlay)
    {
        if (moveParticle == null)
        {
            return;
        }

        if (shouldPlay)
        {
            if (!moveParticle.isPlaying)
            {
                moveParticle.Play();
            }
        }
        else
        {
            StopMoveParticle();
        }
    }

    private void StopMoveParticle()
    {
        if (moveParticle != null && moveParticle.isPlaying)
        {
            moveParticle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }
}
