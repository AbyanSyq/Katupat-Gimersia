using System;
using System.Collections.Generic;
using Redcode.Pools;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Globalization;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController3D : MonoBehaviour
{
    #region State
    [Flags]
    public enum PlayerState
    {
        IDLE = 0,
        WALK = 1 << 0,
        DEAD = 1 << 1,
        MOVING = WALK
    }

    [SerializeField, FoldoutGroup("State Settings")] private bool isFirstPerson = false;
    [SerializeField, FoldoutGroup("State Settings")] private PlayerState currentState;
    [FoldoutGroup("State Settings")] public PlayerState CurrentState => currentState;
    #endregion

    #region Movement
    [FoldoutGroup("Movement Settings"), SerializeField] private float moveSpeed = 8.0f;
    [FoldoutGroup("Movement Settings"), SerializeField] private float moveSpeedWhenCharge = 2.0f;
    [FoldoutGroup("Movement Settings"), SerializeField, Range(0.0f, 0.3f)] private float rotationSmoothTime = 0.12f;
    [FoldoutGroup("Movement Settings"), SerializeField] private float speedChangeRate = 10.0f;

    [Space]
    [FoldoutGroup("Movement Settings"), SerializeField, ReadOnly] private float speed;
    [FoldoutGroup("Movement Settings"), SerializeField, ReadOnly] private float animationBlend;
    [FoldoutGroup("Movement Settings"), SerializeField, ReadOnly] private float targetRotation = 0.0f;
    [FoldoutGroup("Movement Settings"), SerializeField, ReadOnly] private float rotationVelocity;
    [FoldoutGroup("Movement Settings"), SerializeField, ReadOnly] private float verticalVelocity;
    [FoldoutGroup("Movement Settings"), SerializeField, ReadOnly] private float terminalVelocity = 53.0f;
    #endregion

    #region Jump & Gravity
    [Header("Jump & Gravity")]
    [FoldoutGroup("Jump & Gravity"), SerializeField] private float jumpHeight = 1.2f;
    [FoldoutGroup("Jump & Gravity"), SerializeField] private float gravity = -15.0f;
    [FoldoutGroup("Jump & Gravity"), SerializeField] private float jumpTimeout = 0.5f;
    [FoldoutGroup("Jump & Gravity"), SerializeField] private float fallTimeout = 0.15f;

    [FoldoutGroup("Jump & Gravity"), SerializeField, ReadOnly] private float jumpTimeoutDelta;
    [FoldoutGroup("Jump & Gravity"), SerializeField, ReadOnly] private float fallTimeoutDelta;

    [Header("Air Control")]
    // Stored horizontal velocity captured at the moment of jump (used to preserve momentum)
    [FoldoutGroup("Jump & Gravity"), SerializeField] private bool disableAirControl = true;
    [FoldoutGroup("Jump & Gravity"), SerializeField, Range(0f, 1f)] private float airControlFactor = 0.5f; // 0 = no steering, 1 = full steering
    [FoldoutGroup("Jump & Gravity"), SerializeField, Range(0.1f, 20f)] private float airControlLerpSpeed = 8f; // responsiveness when blending stored velocity towards input
    private Vector3 storedAirVelocity = Vector3.zero;

    [Header("Knockback")]
    [FoldoutGroup("Jump & Gravity"), SerializeField] private float knockbackForce = 15f; // horizontal force magnitude
    [FoldoutGroup("Jump & Gravity"), SerializeField] private float knockbackHeight = 10f; // vertical velocity magnitude
    private bool isKnocked = false;
    bool originalAirControl;
    // landing/knockback tracking
    private bool prevGrounded = true;
    private bool knockbackInProgress = false;
    #endregion

    #region Ground Check
    [FoldoutGroup("Ground Check"), SerializeField] public bool grounded = true;
    [FoldoutGroup("Ground Check"), SerializeField] private float groundedOffset = -0.14f;
    [FoldoutGroup("Ground Check"), SerializeField] private float groundedRadius = 0.28f;
    [FoldoutGroup("Ground Check"), SerializeField] private LayerMask groundLayers;
    #endregion

    #region Camera
    [FoldoutGroup("Cinemachine Camera"), SerializeField] private GameObject cinemachineCameraTarget;
    [FoldoutGroup("Cinemachine Camera"), SerializeField] private float rotationSpeed = 1.0f;
    [FoldoutGroup("Cinemachine Camera"), SerializeField] private float topClamp = 70.0f;
    [FoldoutGroup("Cinemachine Camera"), SerializeField] private float bottomClamp = -30.0f;
    [FoldoutGroup("Cinemachine Camera"), SerializeField] private float cameraAngleOverride = 0.0f;
    [FoldoutGroup("Cinemachine Camera"), SerializeField] private bool lockCameraPosition = false;

    [FoldoutGroup("Cinemachine Camera"), ReadOnly] private float cinemachineTargetYaw;
    [FoldoutGroup("Cinemachine Camera"), ReadOnly] private float cinemachineTargetPitch;
    [FoldoutGroup("Cinemachine Camera"), ReadOnly] private GameObject mainCamera;
    [FoldoutGroup("Cinemachine Camera"), Range(0f, 120f), ReadOnly] private float originalCameraFOV;
    [FoldoutGroup("Cinemachine Camera"), SerializeField] private CinemachineCamera cinemachineCameraComponent;
    [FoldoutGroup("Cinemachine Camera"), SerializeField] private CinemachineImpulseSource cinemachineImpulseSource;
    [FoldoutGroup("Camera Shake"), SerializeField] private float maxShakeIntensity = 1.5f;
    [FoldoutGroup("Camera Shake"), SerializeField] private float shakeIncreaseSpeed = 0.8f;
    [FoldoutGroup("Camera Shake"), SerializeField, ReadOnly] private Coroutine chargeShakeCoroutine;

    #endregion

    #region Input System
    [FoldoutGroup("Input System"), SerializeField, ReadOnly] private CharacterController controller;
    [FoldoutGroup("Input System"), SerializeField, ReadOnly] private PlayerInputHandler input;
    [FoldoutGroup("Input System"), SerializeField, ReadOnly] private PlayerInput playerInput;
    [FoldoutGroup("Input System"), SerializeField, ReadOnly] private const float THRESHOLD = 0.01f;

    private bool IsCurrentDeviceMouse
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            return playerInput.currentControlScheme == "KeyboardMouse";
#else
            return false;
#endif
        }
    }
    #endregion

    #region Animation
    [Serializable]
    public enum AnimationEventTriggerType
    {
        ONLAND,
        ONFOODSTEP,
    }

    [Space]
    [FoldoutGroup("Animation"), SerializeField] private string animParameterSpeedName;
    [FoldoutGroup("Animation"), SerializeField] private string animParameterJumpName;
    [FoldoutGroup("Animation"), SerializeField] private string animParameterGroundedName;
    [FoldoutGroup("Animation"), SerializeField] private string animParameterFreeFallName;
    [FoldoutGroup("Animation"), SerializeField] private string animParameterMotionSpeedName;
    [FoldoutGroup("Animation"), SerializeField] private string animParameterThrowForceName;

    [Space]
    [FoldoutGroup("Animation"), SerializeField, ReadOnly] private int animParameterIDSpeed;
    [FoldoutGroup("Animation"), SerializeField, ReadOnly] private int animParameterIDJump;
    [FoldoutGroup("Animation"), SerializeField, ReadOnly] private int animParameterIDGrounded;
    [FoldoutGroup("Animation"), SerializeField, ReadOnly] private int animParameterIDFreeFall;
    [FoldoutGroup("Animation"), SerializeField, ReadOnly] private int animParameterIDMotionSpeed;
    [FoldoutGroup("Animation"), SerializeField, ReadOnly] private int animParameterIDThrowForce;
    [FoldoutGroup("Animation"), SerializeField, ReadOnly] private Animator animator;
    [FoldoutGroup("Animation"), SerializeField, ReadOnly] private bool hasAnimator;

    [Header("Throwing Spear")]
    [FoldoutGroup("Throw"), SerializeField] GameObject spearPrefab;
    [FoldoutGroup("Throw"), SerializeField] Transform throwPoint;
    [Header("Throwing Spear")]
    [FoldoutGroup("Throw"), SerializeField] public float minThrowForce;
    [FoldoutGroup("Throw"), SerializeField] public float maxThrowForce;
    [FoldoutGroup("Throw"), SerializeField] public float chargeSpeed;
    [FoldoutGroup("Throw"), SerializeField] private float fovResetSpeed;
    [FoldoutGroup("Throw"), SerializeField] float throwCooldown;
    [FoldoutGroup("Throw"), SerializeField] float chargeLowestDuration;

    [FoldoutGroup("Throw"), SerializeField, ReadOnly] private bool isCharging;
    [FoldoutGroup("Throw"), SerializeField, ReadOnly] bool isReloading;
    [FoldoutGroup("Throw"), SerializeField, ReadOnly] private float chargeStartTime;
    [FoldoutGroup("Throw"), SerializeField, ReadOnly] private float currentThrowForce;
    [FoldoutGroup("Throw"), SerializeField, ReadOnly] private float currentThrowForceNormalized;
    [FoldoutGroup("Throw"), SerializeField, ReadOnly] float initialThrowCooldown;

    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

        if (cinemachineCameraComponent == null)
        {
            cinemachineCameraComponent = GameObject.Find("PlayerFollowCamera3D").GetComponent<CinemachineCamera>();
        }

        originalCameraFOV = cinemachineCameraComponent.Lens.FieldOfView;

        input = GetComponent<PlayerInputHandler>();
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        hasAnimator = TryGetComponent(out animator);
    }

    private void Start()
    {
        cinemachineTargetYaw = cinemachineCameraTarget.transform.rotation.eulerAngles.y;
        AssignAnimationID();

        jumpTimeoutDelta = jumpTimeout;
        fallTimeoutDelta = fallTimeout;
        initialThrowCooldown = throwCooldown;
        throwCooldown = 0f;
        originalAirControl = disableAirControl;
        prevGrounded = grounded;
    }

    private void Update()
    {
        // preserve previous grounded state for landing detection
        prevGrounded = grounded;

        hasAnimator = TryGetComponent(out animator);

        JumpAndGravity();
        GroundedCheck();
        Move();
        HandleSpearCharge();
        HandleSpearVisibility();
        HandleSpearCooldown();

        // If knockback was active, detect landing to end it (player was previously not grounded and now is)
        if (knockbackInProgress && grounded && !prevGrounded)
        {
            knockbackInProgress = false;
            isKnocked = false;
            disableAirControl = originalAirControl;
            storedAirVelocity = Vector3.zero;
            Debug.Log("Knockback ended on landing");
        }
    }

    private void LateUpdate()
    {
        CameraRotation();

        float targetFOV = isCharging ? originalCameraFOV * 1.2f : originalCameraFOV;
        float fovDifference = Mathf.Abs(targetFOV - originalCameraFOV);
        float fovSpeed = isCharging ? fovDifference / chargeSpeed : fovResetSpeed;
        cinemachineCameraComponent.Lens.FieldOfView = Mathf.MoveTowards(
            cinemachineCameraComponent.Lens.FieldOfView,
            targetFOV,
            fovSpeed * Time.deltaTime
        );
    }
    #endregion

    #region Player State
    public void ChangeState(PlayerState newState)
    {
        if (currentState == newState) return;
        currentState = newState;
    }
    #endregion

    #region Movement
    private void Move()
    {
        // If we're in the air handle stored velocity + optional steering
        if (!grounded)
        {
            if (disableAirControl)
            {
                // No air control: keep original stored horizontal velocity
                controller.Move(storedAirVelocity * Time.deltaTime +
                                new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);

                // Update animator to reflect no horizontal control (optional)
                if (hasAnimator)
                {
                    animator.SetFloat(animParameterIDSpeed, 0f);
                    animator.SetFloat(animParameterIDMotionSpeed, 0f);
                }

                return;
            }

            // Air control enabled: steer the stored horizontal velocity towards input direction
            float camYawLocal = mainCamera.transform.eulerAngles.y;
            Vector3 inputDirLocal = new Vector3(input.move.x, 0.0f, input.move.y).normalized;
            Vector3 desiredHorizLocal = Quaternion.Euler(0.0f, camYawLocal, 0.0f) * inputDirLocal * moveSpeed * input.move.magnitude;

            // Blend storedAirVelocity towards desired horizontal velocity based on airControlFactor and responsiveness
            float blend = Mathf.Clamp01(airControlFactor) * airControlLerpSpeed * Time.deltaTime;
            storedAirVelocity = Vector3.Lerp(storedAirVelocity, desiredHorizLocal, blend);

            controller.Move(storedAirVelocity * Time.deltaTime +
                            new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);

            if (hasAnimator)
            {
                animator.SetFloat(animParameterIDSpeed, storedAirVelocity.magnitude);
                animator.SetFloat(animParameterIDMotionSpeed, input.move.magnitude);
            }

            return;
        }

        float targetSpeed = isCharging ? moveSpeedWhenCharge : moveSpeed;
        if (input.move == Vector2.zero) targetSpeed = 0.0f;
        float groundedMultiplier = grounded ? 1f : 0.01f;

        float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude * groundedMultiplier;
        float speedOffset = 0.1f;
        float inputMagnitude = input.move.magnitude;

        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * speedChangeRate);
            speed = Mathf.Round(speed * 1000f) / 1000f;
        }
        else
        {
            speed = targetSpeed;
        }

        animationBlend = Mathf.Lerp(animationBlend, targetSpeed, Time.deltaTime * speedChangeRate);
        if (animationBlend < 0.01f) animationBlend = 0f;

        float cameraYaw = mainCamera.transform.eulerAngles.y;
        Vector3 inputDirection = new Vector3(input.move.x, 0.0f, input.move.y).normalized;
        Vector3 moveDirection = Quaternion.Euler(0.0f, cameraYaw, 0.0f) * inputDirection;

        controller.Move(moveDirection.normalized * (speed * Time.deltaTime) +
                        new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);

        if (hasAnimator)
        {
            animator.SetFloat(animParameterIDSpeed, animationBlend);
            animator.SetFloat(animParameterIDMotionSpeed, inputMagnitude);
        }
    }
    #endregion

    #region Grounded Check
    private void GroundedCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z);
        grounded = Physics.CheckSphere(spherePosition, groundedRadius, groundLayers, QueryTriggerInteraction.Ignore);

        if (hasAnimator)
        {
            animator.SetBool(animParameterIDGrounded, grounded);
        }
    }
    #endregion

    #region Camera
    private void CameraRotation()
    {
        if (GameManager.Instance.isGamePaused) return;
        if (lockCameraPosition) return;

        if (input.look.sqrMagnitude >= THRESHOLD)
        {
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            cinemachineTargetYaw += input.look.x * rotationSpeed * deltaTimeMultiplier;
            cinemachineTargetPitch += input.look.y * rotationSpeed * deltaTimeMultiplier;
        }

        cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, bottomClamp, topClamp);

        cinemachineCameraTarget.transform.rotation = Quaternion.Euler(
            cinemachineTargetPitch + cameraAngleOverride,
            cinemachineTargetYaw,
            0.0f
        );

        float smoothYaw = Mathf.SmoothDampAngle(
            transform.eulerAngles.y,
            cinemachineTargetYaw,
            ref rotationVelocity,
            rotationSmoothTime
        );

        transform.rotation = Quaternion.Euler(0.0f, smoothYaw, 0.0f);
    }

    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f) angle += 360f;
        if (angle > 360f) angle -= 360f;
        return Mathf.Clamp(angle, min, max);
    }
    #endregion

    #region Teleport
    public void Teleport(Vector3 position)
    {
        transform.position = position;
    }

    public void Teleport(Transform value)
    {
        controller.enabled = false;
        transform.position = value.position;
        transform.rotation = value.rotation;
        controller.enabled = true;
    }
    #endregion

    #region Animation Utility
    private void AssignAnimationID()
    {
        animParameterIDSpeed = Animator.StringToHash(animParameterSpeedName);
        animParameterIDJump = Animator.StringToHash(animParameterJumpName);
        animParameterIDGrounded = Animator.StringToHash(animParameterGroundedName);
        animParameterIDFreeFall = Animator.StringToHash(animParameterFreeFallName);
        animParameterIDMotionSpeed = Animator.StringToHash(animParameterMotionSpeedName);
        animParameterIDThrowForce = Animator.StringToHash(animParameterThrowForceName);
    }

    public void OnAnimationEventTrigger(AnimationEventTriggerType type)
    {
        Debug.Log("Animation Event Triggered: " + type.ToString());
    }

    public void PlayAnimation(string animationID)
    {
        if (hasAnimator)
        {
            animator.Play(animationID);
        }
    }
    #endregion

    #region Draw Gizmos
    private void OnDrawGizmosSelected()
    {
        Color color = grounded ? new Color(0, 1, 0, 0.35f) : new Color(1, 0, 0, 0.35f);
        Gizmos.color = color;
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z), groundedRadius);
    }
    #endregion

    #region Attack (Throw Spear)
    public void StartCharging()
    {
        if (isCharging || throwCooldown > 0f || isKnocked) return;
        isCharging = true;
        chargeStartTime = Time.time;

        animator.SetBool("IsChargingSpear", true);

        if (chargeShakeCoroutine != null)
            StopCoroutine(chargeShakeCoroutine);
        chargeShakeCoroutine = StartCoroutine(CameraShakeDuringCharge());
    }

    public void CancelThrow()
    {
        if (!isCharging || throwCooldown > 0f) return;

        isCharging = false;

        if (chargeShakeCoroutine != null)
        {
            StopCoroutine(chargeShakeCoroutine);
            chargeShakeCoroutine = null;
        }

        animator.SetBool("IsChargingSpear", false);
        animator.ResetTrigger("ThrowSpear");

        currentThrowForce = 0f;
        currentThrowForceNormalized = 0f;
        animator.SetFloat(animParameterIDThrowForce, 0f);
        Events.OnPlayerChargeForceChanged.Publish(0f);

        cinemachineCameraComponent.Lens.FieldOfView = originalCameraFOV;

        // Debug.Log("Throw canceled.");
    }


    public void ReleaseThrow()
    {
        if (!isCharging || throwCooldown > 0f) return;
        isCharging = false;
        isReloading = true;
        throwCooldown = initialThrowCooldown;

        if (chargeShakeCoroutine != null)
        {
            StopCoroutine(chargeShakeCoroutine);
            chargeShakeCoroutine = null;
        }

        float chargeDuration = Time.time - chargeStartTime;
        if (chargeDuration < chargeLowestDuration)
        {
            StartCoroutine(DelayedReleaseThrow(chargeLowestDuration - chargeDuration));
        }
        else
        {
            DoReleaseThrow(chargeDuration);
        }
    }

    private IEnumerator DelayedReleaseThrow(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        float chargeDuration = Time.time - chargeStartTime;
        DoReleaseThrow(Mathf.Max(chargeDuration, chargeLowestDuration));
    }

    private void DoReleaseThrow(float chargeDuration)
    {
        currentThrowForce = Mathf.Lerp(minThrowForce, maxThrowForce, chargeDuration / chargeSpeed);
        currentThrowForce = Mathf.Clamp(currentThrowForce, minThrowForce, maxThrowForce);

        throwCooldown = initialThrowCooldown;
        isReloading = true;

        animator.SetTrigger("ThrowSpear");
        animator.SetBool("IsChargingSpear", false);
        animator.SetBool("IsSpearVisible", false);

        ThrowSpear();
    }

    private void HandleSpearCharge()
    {
        if (isCharging)
        {
            float chargeDuration = Time.time - chargeStartTime;
            currentThrowForce = Mathf.Lerp(minThrowForce, maxThrowForce, chargeDuration / chargeSpeed);
        }
        else
        {
            currentThrowForce = 0f;
        }

        currentThrowForceNormalized = Mathf.InverseLerp(minThrowForce, maxThrowForce, currentThrowForce);
        Events.OnPlayerChargeForceChanged.Publish(currentThrowForceNormalized);
        animator.SetFloat(animParameterIDThrowForce, currentThrowForceNormalized);
    }

    private void ThrowSpear()
    {
        if (PoolManager.Instance.GetPool<Spear>("Spear") == null || throwPoint == null)
        {
            Debug.LogWarning("Missing Spear pool or throwPoint reference!");
            return;
        }

        float crosshairDistance = 15f;
        Vector3 crosshairWorldPos = mainCamera.transform.position + mainCamera.transform.forward * crosshairDistance;
        Vector3 throwDirection = (crosshairWorldPos - throwPoint.position).normalized;

        throwPoint.rotation = Quaternion.LookRotation(throwDirection, Vector3.up);

        var spearInstance = PoolManager.Instance.GetFromPool<Spear>(0);
        if (spearInstance == null)
        {
            Debug.LogWarning("No spear available in the pool!");
            return;
        }

        spearInstance.transform.position = throwPoint.position;
        spearInstance.transform.rotation = throwPoint.rotation;

        Rigidbody rb = spearInstance.GetComponent<Rigidbody>();
        if (rb == null)
            rb = spearInstance.AddComponent<Rigidbody>();

        rb.useGravity = true;
        rb.isKinematic = false;

        rb.linearVelocity = throwDirection * currentThrowForce;

        Spear spearRotator = spearInstance.GetComponent<Spear>();
        if (spearRotator == null)
            spearRotator = spearInstance.AddComponent<Spear>();
    }

    private IEnumerator CameraShakeDuringCharge()
    {
        float intensity = 0f;

        while (isCharging)
        {
            // Hitung waktu charging relatif
            float chargeDuration = Time.time - chargeStartTime;
            float t = Mathf.Clamp01(chargeDuration / chargeSpeed);

            // Naikkan intensitas getar seiring waktu
            intensity = Mathf.Lerp(0f, maxShakeIntensity, Mathf.SmoothStep(0f, 1f, t));

            // Kirim impulse kecil ke kamera (arah acak)
            Vector3 randomDir = UnityEngine.Random.insideUnitSphere.normalized * 0.2f;
            cinemachineImpulseSource.GenerateImpulse(randomDir * intensity);

            // Kirim getaran tiap beberapa frame agar tidak terlalu sering
            yield return new WaitForSeconds(0.1f);
        }

        // Setelah charging selesai → hentikan shake
        yield break;
    }

    void HandleSpearVisibility()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("SpearReload") || animator.GetCurrentAnimatorStateInfo(0).IsName("SpearIdle"))
            animator.SetBool("IsSpearVisible", true);

        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("SpearThrow"))
            animator.SetBool("IsSpearVisible", false);
    }

    void HandleSpearCooldown()
    {
        throwCooldown -= Time.deltaTime;
        if (throwCooldown < 0f) isReloading = false;
    }

    #endregion

    #region Knockback
    /// <summary>
    /// Apply knockback to the player from a damage position.
    /// Launches player away horizontally and upward with no air control.
    /// </summary>
    public void ApplyKnockback(Vector3 damagePosition)
    {
        isKnocked = true;
        knockbackInProgress = true;

        // Calculate direction away from damage source
        Vector3 knockbackDir = (transform.position - damagePosition).normalized;
        knockbackDir.y = 0f; // Flatten to horizontal

        // if no horizontal direction, push backward
        if (knockbackDir == Vector3.zero)
        {
            knockbackDir = -transform.forward;
        }
        knockbackDir = knockbackDir.normalized;

        // Store horizontal knockback velocity
        storedAirVelocity = knockbackDir * knockbackForce;

        // Apply vertical knockback
        verticalVelocity = knockbackHeight;

        // Temp disable air control during knockback
        disableAirControl = true;

        if (hasAnimator)
        {
            animator.SetBool(animParameterIDJump, true);
        }

        Debug.Log($"Player knocked back from {damagePosition}. isKnocked = true");

        // Apply an immediate small movement so the player is moved off the ground this frame
        // and the airborne logic takes effect on the next Update cycle.
        controller.Move(storedAirVelocity * Time.deltaTime + new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);
    }

    #endregion

    #region Jump and Gravity

    private void JumpAndGravity()
    {
        if (grounded)
        {
            fallTimeoutDelta = fallTimeout;

            if (hasAnimator)
            {
                animator.SetBool(animParameterIDJump, false);
                animator.SetBool(animParameterIDFreeFall, false);
            }

            // For normal landings (not knockback), clear stored horizontal velocity so movement input resumes normally.
            if (!knockbackInProgress)
            {
                storedAirVelocity = Vector3.zero;
            }

            if (verticalVelocity < 0.0f && !knockbackInProgress)
            {
                verticalVelocity = -2f;
            }

            if (input.jump && jumpTimeoutDelta <= 0.0f)
            {
                // Capture current horizontal velocity so the jump inherits movement momentum
                storedAirVelocity = new Vector3(controller.velocity.x, 0f, controller.velocity.z);

                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

                if (hasAnimator)
                {
                    animator.SetBool(animParameterIDJump, true);
                }
            }

            if (jumpTimeoutDelta >= 0.0f)
            {
                jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            jumpTimeoutDelta = jumpTimeout;

            if (fallTimeoutDelta >= 0.0f)
            {
                fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                // update animator if using character
                if (hasAnimator)
                {
                    animator.SetBool(animParameterIDFreeFall, true);
                }
            }

            input.jump = false;
        }

        if (verticalVelocity < terminalVelocity)
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
    }
    #endregion
}
