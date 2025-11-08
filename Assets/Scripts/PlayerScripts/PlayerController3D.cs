using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
        RUN = 1 << 1,
        DEAD = 1 << 2,
        MOVING = WALK | RUN
    }

    [SerializeField, FoldoutGroup("State Settings")] private bool isFirstPerson = false;
    [SerializeField, FoldoutGroup("State Settings")] private PlayerState currentState;
    [FoldoutGroup("State Settings")] public PlayerState CurrentState => currentState;
    #endregion

    #region Movement
    [FoldoutGroup("Movement Settings"), SerializeField] private float moveSpeed = 2.0f;
    [FoldoutGroup("Movement Settings"), SerializeField] private float sprintSpeed = 5.335f;
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
    [FoldoutGroup("Jump & Gravity"), SerializeField] private float jumpHeight = 1.2f;
    [FoldoutGroup("Jump & Gravity"), SerializeField] private float gravity = -15.0f;
    [FoldoutGroup("Jump & Gravity"), SerializeField] private float jumpTimeout = 0.5f;
    [FoldoutGroup("Jump & Gravity"), SerializeField] private float fallTimeout = 0.15f;

    [FoldoutGroup("Jump & Gravity"), SerializeField, ReadOnly] private float jumpTimeoutDelta;
    [FoldoutGroup("Jump & Gravity"), SerializeField, ReadOnly] private float fallTimeoutDelta;
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

    [Space]
    [FoldoutGroup("Animation"), SerializeField, ReadOnly] private int animParameterIDSpeed;
    [FoldoutGroup("Animation"), SerializeField, ReadOnly] private int animParameterIDJump;
    [FoldoutGroup("Animation"), SerializeField, ReadOnly] private int animParameterIDGrounded;
    [FoldoutGroup("Animation"), SerializeField, ReadOnly] private int animParameterIDFreeFall;
    [FoldoutGroup("Animation"), SerializeField, ReadOnly] private int animParameterIDMotionSpeed;
    [FoldoutGroup("Animation"), SerializeField, ReadOnly] private Animator animator;
    [FoldoutGroup("Animation"), SerializeField, ReadOnly] private bool hasAnimator;


    [Header("Throwing Spear")]
    [FoldoutGroup("Throw"), SerializeField] GameObject spearPrefab;
    [FoldoutGroup("Throw"), SerializeField] Transform throwPoint;
    [Header("Throwing Spear")]
    [FoldoutGroup("Throw"), SerializeField] public float minThrowForce = 10f;
    [FoldoutGroup("Throw"), SerializeField] public float maxThrowForce = 50f;
    [FoldoutGroup("Throw"), SerializeField] public float chargeSpeed = 1.5f;

    [FoldoutGroup("Throw"), SerializeField, ReadOnly] private bool isCharging;
    [FoldoutGroup("Throw"), SerializeField, ReadOnly] private float chargeStartTime;
    [FoldoutGroup("Throw"), SerializeField, ReadOnly] private float currentThrowForce;

    
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
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
    }

    private void Update()
    {
        hasAnimator = TryGetComponent(out animator);

        // JumpAndGravity();
        GroundedCheck();
        Move();
        HandleSpearCharge();
    }

    private void LateUpdate()
    {
        CameraRotation();
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
        float targetSpeed = input.sprint ? sprintSpeed : moveSpeed;
        if (input.move == Vector2.zero) targetSpeed = 0.0f;

        float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;
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

        // ✅ Move relative to camera direction, but don’t rotate player
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

    // #region Jump and Gravity
    // private void JumpAndGravity()
    // {
    //     if (grounded)
    //     {
    //         fallTimeoutDelta = fallTimeout;

    //         if (hasAnimator)
    //         {
    //             animator.SetBool(animParameterIDJump, false);
    //             animator.SetBool(animParameterIDFreeFall, false);
    //         }

    //         if (verticalVelocity < 0.0f)
    //         {
    //             verticalVelocity = -2f;
    //         }

    //         if (input.jump && jumpTimeoutDelta <= 0.0f)
    //         {
    //             verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

    //             if (hasAnimator)
    //             {
    //                 animator.SetBool(animParameterIDJump, true);
    //             }
    //         }

    //         if (jumpTimeoutDelta >= 0.0f)
    //         {
    //             jumpTimeoutDelta -= Time.deltaTime;
    //         }
    //     }
    //     else
    //     {
    //         jumpTimeoutDelta = jumpTimeout;

    //         if (fallTimeoutDelta >= 0.0f)
    //         {
    //             fallTimeoutDelta -= Time.deltaTime;
    //         }
    //         else
    //         {
    //             // update animator if using character
    //             if (hasAnimator)
    //             {
    //                 animator.SetBool(animParameterIDFreeFall, true);
    //             }
    //         }

    //         input.jump = false;
    //     }

    //     if (verticalVelocity < terminalVelocity)
    //     {
    //         verticalVelocity += gravity * Time.deltaTime;
    //     }
    // }
    // #endregion

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
        if (lockCameraPosition) return;

        // ✅ Handle camera input (mouse/stick)
        if (input.look.sqrMagnitude >= THRESHOLD)
        {
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            cinemachineTargetYaw += input.look.x * rotationSpeed * deltaTimeMultiplier;
            cinemachineTargetPitch += input.look.y * rotationSpeed * deltaTimeMultiplier;
        }

        // ✅ Clamp pitch to prevent flipping
        cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, bottomClamp, topClamp);

        // ✅ Apply rotation to the camera target (used by Cinemachine)
        cinemachineCameraTarget.transform.rotation = Quaternion.Euler(
            cinemachineTargetPitch + cameraAngleOverride,
            cinemachineTargetYaw,
            0.0f
        );

        // ✅ Smoothly rotate player to face camera direction (yaw only)
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
        if (isCharging) return;
        isCharging = true;
        chargeStartTime = Time.time;
    }

    public void ReleaseThrow()
    {
        if (!isCharging) return;
        isCharging = false;

        float chargeDuration = Time.time - chargeStartTime;
        currentThrowForce = Mathf.Lerp(minThrowForce, maxThrowForce, chargeDuration / chargeSpeed);
        currentThrowForce = Mathf.Clamp(currentThrowForce, minThrowForce, maxThrowForce);

        ThrowSpear();
    }

    private void HandleSpearCharge()
    {
        if (isCharging)
        {
            float chargeDuration = Time.time - chargeStartTime;
            currentThrowForce = Mathf.Lerp(minThrowForce, maxThrowForce, chargeDuration / chargeSpeed);
        }
    }

    private void ThrowSpear()
    {
        if (spearPrefab == null || throwPoint == null)
        {
            Debug.LogWarning("Missing spearPrefab or throwPoint reference!");
            return;
        }

        // How far ahead the crosshair point is (you can tweak this)
        float crosshairDistance = 15f;

        // Get the world position of the crosshair (center of the screen)
        Vector3 crosshairWorldPos = mainCamera.transform.position + mainCamera.transform.forward * crosshairDistance;

        // Calculate direction from throw point to that position
        Vector3 throwDirection = (crosshairWorldPos - throwPoint.position).normalized;

        // Rotate the throw point to aim at the crosshair (vertical + horizontal)
        throwPoint.rotation = Quaternion.LookRotation(throwDirection, Vector3.up);

        // Spawn spear at throw point
        GameObject spearInstance = Instantiate(spearPrefab, throwPoint.position, throwPoint.rotation);

        // Get or add Rigidbody
        Rigidbody rb = spearInstance.GetComponent<Rigidbody>();
        if (rb == null)
            rb = spearInstance.AddComponent<Rigidbody>();

        rb.useGravity = true;
        rb.isKinematic = false;

        // Apply throw force
        rb.linearVelocity = throwDirection * currentThrowForce;

        // Optional: make spear face its velocity while flying
        Spear spearRotator = spearInstance.GetComponent<Spear>();
        if (spearRotator == null)
            spearRotator = spearInstance.AddComponent<Spear>();
    }

    #endregion

}

