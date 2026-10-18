using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour {

    [Header("References")]
    [SerializeField] private PlayerStats _stats;
    [SerializeField] private PlayerCamera _camera;

    [Header("Ground Check")]
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float _groundCheckRadius = 0.25f;
    [SerializeField] private LayerMask _groundLayer;

    public PlayerStats Stats => _stats;
    public PlayerCamera CameraRef => _camera;
    public CharacterController CharacterController { get; private set; }
    public PlayerAnimator Animator { get; private set; }

    private Vector2 _moveInput;
    private bool _jumpPressedThisFrame;
    private bool _sprintHeld;
    private bool _dodgePressedThisFrame;

    public Vector2 MoveInput => _moveInput;
    public bool HasMoveInput => _moveInput.sqrMagnitude > 0.01f;

    public bool WantsToJump => _jumpPressedThisFrame && _coyoteTimer > 0f;
    public bool WantsToDodge => _dodgePressedThisFrame && Time.time - _lastDodgeTime > _stats.DodgeCooldown && _stats.HasStamina(_stats.DodgeStaminaCost);

    public bool WantsToSprint => _sprintHeld && HasMoveInput && _stats.HasStamina(1f);

    public Vector3 MoveDirection { get; set; }
    public float VerticalVelocity { get; private set; }
    public Vector3 DodgeDirection { get; private set; }
    public bool IsGrounded { get; private set; }

    private float _coyoteTimer;
    private float _jumpBufferTimer;
    private bool _jumpReleased;

    private float _lastDodgeTime = -999f;

    private PlayerStateMachine _stateMachine;
    private PlayerStateFactory _factory;

    private void Awake() {
        CharacterController = GetComponent<CharacterController>();
        Animator = GetComponent<PlayerAnimator>();

        _inputSetup();

        _stateMachine = new PlayerStateMachine();
        _factory = new PlayerStateFactory(this, _stateMachine);
        _stateMachine.Initialize(_factory.Idle);
    }

    private PlayerControls _input;
    private void _inputSetup() {
        _input = new PlayerControls();

        _input.Player.Move.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
        _input.Player.Move.canceled += ctx => _moveInput = Vector2.zero;

        _input.Player.Jump.performed += ctx => {
            _jumpPressedThisFrame = true;
            _jumpBufferTimer = _stats.JumpBufferTime;
        };
        _input.Player.Jump.canceled += ctx => _jumpReleased = true; ;

        _input.Player.Sprint.performed += ctx => _sprintHeld = true;
        _input.Player.Sprint.canceled += ctx => _sprintHeld = false;

        _input.Player.Dodge.performed += ctx => _dodgePressedThisFrame = true;
    }


    private void OnEnable() => _input.Player.Enable();
    private void OnDisable() => _input.Player.Disable();

    private void Update() {
        UpdateGrounded();
        UpdateCoyoteTimer();
        UpdateJumpBuffer();
        HandleGravity();

        _stateMachine.Update();

        _stats.RegenStamina(Time.deltaTime);
        UpdateAnimator();

        _jumpPressedThisFrame = false;
        _dodgePressedThisFrame = false;
        _jumpReleased = false;
    }

    public void ApplyMovement() {
        Vector3 inputDir = GetCameraRelativeMoveDirection();

        float targetSpeed = WantsToSprint ? _stats.RunSpeed : _stats.WalkSpeed;
        if (WantsToSprint) {
            _stats.ConsumeStamina(_stats.StaminaDrainPerSec * Time.deltaTime);
        }

        float currentSpeed = MoveDirection.magnitude;
        float desiredSpeed = inputDir.magnitude * targetSpeed;
        float newSpeed = Mathf.MoveTowards(currentSpeed, desiredSpeed, _stats.Acceleration * Time.deltaTime);

        if (inputDir.sqrMagnitude > 0.01f) {
            MoveDirection = inputDir.normalized * newSpeed;
            Quaternion targetRot = Quaternion.LookRotation(inputDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, _stats.RotationSpeed * Time.deltaTime);
        } else {
            MoveDirection = Vector3.MoveTowards(MoveDirection, Vector3.zero, _stats.Acceleration * Time.deltaTime);
        }

        CharacterController.Move(MoveDirection * Time.deltaTime);
    }

    public void MoveCharacter(Vector3 delta) => CharacterController.Move(delta);

    public Vector3 GetCameraRelativeMoveDirection() {
        Vector3 f = _camera.PlanarForward;
        Vector3 r = _camera.PlanarRight;
        return f * _moveInput.y + r * _moveInput.x;
    }

    public void SetVerticalVelocity(float v) => VerticalVelocity = v;
    public void ResetCoyoteTimer() => _coyoteTimer = 0f;
    public void MarkDodgeTime() => _lastDodgeTime = Time.time;
    public void CosumeStamina(float amount) => _stats.ConsumeStamina(amount);
    public void SetDodgeDirection(Vector3 dir) => DodgeDirection = dir;

    private void UpdateGrounded() {
        IsGrounded = Physics.CheckSphere(_groundCheck.position, _groundCheckRadius, _groundLayer, QueryTriggerInteraction.Ignore);
    }

    private void UpdateCoyoteTimer() {
        if (IsGrounded)
            _coyoteTimer = _stats.CoyoteTime;
        else
            _coyoteTimer -= Time.deltaTime;
    }

    private void UpdateJumpBuffer() {
        if (_jumpBufferTimer > 0f)
            _jumpBufferTimer -= Time.deltaTime;
    }

    private void HandleGravity() {
        if (IsGrounded && VerticalVelocity < 0f)
            VerticalVelocity = -2f;

        float multiplier = 1f;
        if (VerticalVelocity < 0f)
            multiplier = _stats.FallGravityMultiplier;
        else if (_jumpReleased)
            multiplier = _stats.LowJumpMultiplier;

        VerticalVelocity += _stats.Gravity * multiplier * Time.deltaTime;
        CharacterController.Move(Vector3.up * VerticalVelocity * Time.deltaTime);
    }

    private void UpdateAnimator() {
        //float normalized = MoveDirection.magnitude / _stats.RunSpeed;
        float targetSpeed = WantsToSprint ? Stats.RunSpeed : Stats.WalkSpeed;
        float targetNormalized = HasMoveInput ? targetSpeed / Stats.RunSpeed : 0f;

        Animator.SetSpeed(targetNormalized);
        Animator.SetGrounded(IsGrounded);
        Animator.SetVerticalVelocity(VerticalVelocity);
    }

    private void OnDrawGizmosSelected() {
        if (_groundCheck == null) return;
        Gizmos.color = IsGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(_groundCheck.position, _groundCheckRadius);
    }
}
