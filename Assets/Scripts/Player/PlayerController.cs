using System.Collections;
using UnityEngine;
using Guizzan.Input.GIM;
using Guizzan.Input.GIM.Player;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviour, IGuizzanInputManager<PlayerInputs>
{
    private GuizzanInputManager GIM;
    private Animator _animator;
    public static PlayerController Instance;

    [Header(">>>>>>>>>>>>> Bools <<<<<<<<<<<<<")]
    public bool RootMotion = true;
    public bool enableMove = true;
    public bool enableLook = true;
    public bool enablePhysics = true;
    public bool enableIK = false;
    public bool enableLookIK = true;
    public bool enableFootIK = true;

    [Header(">>>>>>>>>>>>> Transforms <<<<<<<<<<<<<")]
    public Transform HeadRotator;
    public Transform headTarget;
    public Transform leftHandTarget;
    public Transform rightHandTarget;
    public GameObject AimCamera;
    public GameObject Camera;
    public GameObject Crosshair;

    [HideInInspector]
    public float Speed;
    public LayerMask PlayerMask;

    [Header(">>>>>>>>>>>>> Movement Parameters <<<<<<<<<<<<<")]
    public float MovementLerp = 5;
    public float Gravity = -9.81f;
    public float friction = 1;
    public float airFriction = 1;
    public float JumpHeight;
    public float JumpDelay;
    public float AirControl;
    public float JumpDamp;
    public float StepDown;
    public float PushPower = 2.0F;
    public float CrouchHeight = 0.8F;
    public float CrouchSpeed = 3F;
    [Range(0f, 1f)]
    public float CrouchWalkingSpeed;
    public float FallCrouchHeight = 0.4f;
    public float FallCrouchTime = 0.3f;
    public float MinFallDistance = 0.3f;
    public float MinFallTime = 0.3f;
    public float SlopeForce;
    private MovingPlatformHandler _platform;

    [Header(">>>>>>>>>>>>> Look Parameters <<<<<<<<<<<<<")]
    public float Smoothing = 2;
    public float UpLookLimit;
    public float DownLookLimit;
    public Vector2 _currentMouseLook;
    public Vector2 _appliedMouseDelta;

    [Header(">>>>>>>>>>>>> Runtime Parameters <<<<<<<<<<<<<")]
    public Vector3 _velocity;
    public bool _applyGravity;
    public bool _isJumping;
    public bool _isCrouching;
    public bool _isRunning;
    public bool _isWalking;
    public bool _isFalling;
    public bool _isLanded;
    public bool _isAiming;
    private bool _wasFalling;
    private float startFallTime;
    private float _crouchHeight;
    private float _initialHeight;
    private Vector2 _AnimationLerp;
    private Vector3 _rootMotion;
    private float _stepDown;
    private CharacterController _controller;
    private IEnumerator JumpRoutine;
    private IEnumerator FallCrouchRoutine;
    private ControllerColliderHit Hit;
    private bool godMode;
    private Vector3 PlatformLastVel;
    private PlayerSoundManager _playerSoundManager;
    public RaycastHit SurfaceHit;

    void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        Instance = this;
    }
    private void Start()
    {
        _platform = gameObject.AddComponent<MovingPlatformHandler>();
        GIM = GuizzanInputManager.Instance;
        GIM._playerController = this;
        GIM.InputMode = InputModes.Player;
        _controller = GetComponent<CharacterController>();
        _initialHeight = _controller.height;
        _crouchHeight = CrouchHeight;
        _animator = GetComponent<Animator>();
        _playerSoundManager = GetComponent<PlayerSoundManager>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (enableLookIK)
        {
            _animator.SetLookAtWeight(1);
            _animator.SetLookAtPosition(headTarget.position);
        }
        else
        {
            _animator.SetLookAtWeight(0);
        }

        if (enableIK)
        {
            _animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            _animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget.position);
            _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
        }
        else
        {
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
            _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
        }
    }

    private void Update()
    {

        if (!enablePhysics) return;
        _isWalking = false;
        Physics.Raycast(transform.position + Vector3.up, Vector3.down, out SurfaceHit, PlayerMask);
        if (enableMove)
        {
            if (Vector2.Distance(Vector2.zero, GIM.MovementVector) >= 0.1f && !_isRunning && !IsFalling) _isWalking = true;
            _AnimationLerp = Vector2.MoveTowards(_AnimationLerp, GIM.MovementVector, Time.deltaTime * MovementLerp);
            if (_isCrouching) _AnimationLerp = Vector2.ClampMagnitude(_AnimationLerp, CrouchWalkingSpeed);
            _controller.height = Mathf.Lerp(_controller.height, _isCrouching ? _crouchHeight : _initialHeight, Time.deltaTime * CrouchSpeed);
            _animator.SetFloat("PosX", _AnimationLerp.x);
            _animator.SetFloat("PosY", _AnimationLerp.y);
            _animator.SetBool("Running", _isRunning);
            _animator.SetBool("Jumping", _isJumping);
            _animator.SetBool("Falling", IsFalling);
            _animator.SetBool("Walking", _isWalking);
            var weight = _animator.GetLayerWeight(_animator.GetLayerIndex("WalkingAdditive"));
            _animator.SetLayerWeight(_animator.GetLayerIndex("WalkingAdditive"), Mathf.Lerp(weight, _isWalking ? 0.5f : 0, Time.deltaTime * 3));
        }
        _isLanded = false;
        if (!IsFalling && _wasFalling)
        {
            _playerSoundManager.SoundEvent("Landing");
            _wasFalling = false;
            _isLanded = true;
        }
        _wasFalling = IsFalling;
        if (enableLook)
        {
            Vector2 smoothMouseDelta = Vector2.Scale(GIM.LookVectorDelta / 20, Vector2.one * Smoothing);
            smoothMouseDelta = new Vector2(smoothMouseDelta.x * GIM.SensitivityX(), smoothMouseDelta.y * GIM.SensitivityY());
            _appliedMouseDelta = Vector2.Lerp(_appliedMouseDelta, smoothMouseDelta, 1 / Smoothing);
            _currentMouseLook += _appliedMouseDelta;
            _currentMouseLook.y = Mathf.Clamp(_currentMouseLook.y, -DownLookLimit, UpLookLimit);
        }
        HeadRotator.transform.localRotation = Quaternion.AngleAxis(-_currentMouseLook.y, Vector3.right);
        transform.localRotation = Quaternion.AngleAxis(_currentMouseLook.x, Vector3.up);

        if (enableFootIK)
        {
            bool enableIK = true;
            if (_isFalling || _isWalking || _isRunning || _isJumping || _isLanded)
            {
                enableIK = false;
            }
            if (_isWalking && _isCrouching)
            {
                enableIK = true;
            }
            GetComponent<FootIK>().EnableIK = enableIK;
        }

        if (!RootMotion && !godMode)
            _rootMotion += ((transform.forward * _AnimationLerp.y) + (transform.right * _AnimationLerp.x)) / Time.deltaTime * 0.00025f * Speed;
    }
    private void FixedUpdate() // Physics calculations
    {
        if (!enablePhysics) return;

        if (godMode)
        {
            Vector3 motion = transform.right * GIM.MovementVector.x + transform.forward * GIM.MovementVector.y;
            motion.y = 0;
            if (_isJumping) motion.y = 1;
            if (_isCrouching) motion.y = -1;
            IsFalling = transform.position.y - SurfaceHit.point.y > MinFallDistance;
            ApplyGravity = IsFalling;
            _controller.Move(motion * Speed);
            return;
        }

        _currentMouseLook.x += _platform.AngleDelta.y; // platform rotation

        Vector3 displacement = (_platform.Velocity + PlatformLastVel) * Time.fixedDeltaTime;
        if (ApplyGravity)
        {
            bool Slope = false;
            if (Hit != null)
            {
                float hitAngle = Vector3.Angle(Vector3.up, Hit.normal);
                if (hitAngle >= _controller.slopeLimit && hitAngle <= 80)
                {
                    _velocity += Vector3.ClampMagnitude(SlopeForce * Time.fixedDeltaTime * Vector3.Project(Vector3.up * Gravity, Hit.normal), SlopeForce * 3);
                    Slope = true;
                }
                Hit = null;
            }
            _velocity = Vector3.Lerp(_velocity, new Vector3(0, _velocity.y, 0), Time.fixedDeltaTime * airFriction);
            _velocity.y -= Gravity * Time.fixedDeltaTime;
            displacement += _velocity * Time.fixedDeltaTime;
            displacement += CalculateAirControl();
            _controller.Move(displacement);
            _rootMotion = Vector3.zero;
            if (!Slope)
            {
                ApplyGravity = !_controller.isGrounded;

                IsFalling = transform.position.y - SurfaceHit.point.y > MinFallDistance;
                IsFalling = Time.timeSinceLevelLoad - startFallTime > MinFallTime;
            }
            if (!ApplyGravity)
            {
                JumpRoutine = JumpDelayRoutine();
                StartCoroutine(JumpRoutine);
                IsFalling = false;
                _isJumping = false;
                PlatformLastVel = Vector3.zero;
            }
        }
        else
        {
            _velocity = Vector3.Lerp(_velocity, Vector3.zero, Time.fixedDeltaTime * friction);
            _velocity -= Vector3.up * _stepDown;
            displacement += _velocity * Time.fixedDeltaTime;
            displacement += _rootMotion;
            _controller.Move(displacement);
            _rootMotion = Vector3.zero;
            if (!_controller.isGrounded)
            {
                ApplyGravity = true;
                _velocity += _animator.velocity * JumpDamp;
                _velocity.y = 0;
            }
            else _stepDown = StepDown;
        }
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!enablePhysics) return;
        float hitAngle = Vector3.Angle(Vector3.up, hit.normal);
        Hit = hit;
        if (hitAngle >= _controller.slopeLimit && hitAngle <= 80) ApplyGravity = true;

        if (hit.collider.CompareTag("Platform")) _platform.SetPlatform(hit.collider.transform);
        else _platform.ResetPlatform();

        Rigidbody body = hit.collider.attachedRigidbody;
        if (body == null || body.isKinematic) return;
        if (hit.moveDirection.y < -0.3f) return;
        Vector3 pushDir = new(hit.moveDirection.x, 0, hit.moveDirection.z);
        body.velocity = pushDir * PushPower;
    }

    private void OnAnimatorMove()
    {
        if (!RootMotion) return;
        _rootMotion += _animator.deltaPosition;
    }

    private bool ApplyGravity
    {
        get { return _applyGravity; }
        set
        {
            if (value == _applyGravity)
                return;
            _applyGravity = value;
            if (_applyGravity)
            {
                if (_platform.platform != null)
                {
                    PlatformLastVel = _platform.Velocity;
                    _platform.ResetPlatform();
                }
                startFallTime = Time.timeSinceLevelLoad;
            }
        }
    }

    private bool IsFalling
    {
        get { return _isFalling; }
        set
        {
            if (value == _isFalling)
                return;
            _isFalling = value;
            if (FallCrouchRoutine == null && !value)
            {
                FallCrouchRoutine = FallCrouch();
                StartCoroutine(FallCrouchRoutine);
            }
        }
    }
    public void SetInput(PlayerInputs Input, InputValue value)
    {
        switch (Input)
        {
            case PlayerInputs.Run:
                switch (value)
                {
                    case InputValue.Up:
                        _isRunning = false;
                        break;
                    case InputValue.Down:
                        _isRunning = true;
                        _isCrouching = false;
                        break;
                }
                break;
            case PlayerInputs.Jump:
                switch (value)
                {
                    case InputValue.Down:
                        if (godMode)
                        {
                            _isJumping = true;
                            break;
                        }
                        Jump();
                        break;
                    case InputValue.Up:
                        if (godMode) _isJumping = false;
                        break;
                }
                break;
            case PlayerInputs.Crouch:
                switch (value)
                {
                    case InputValue.Toggle:
                        _isCrouching = !_isCrouching;
                        break;
                    case InputValue.Up:
                        _isCrouching = false;
                        break;
                    case InputValue.Down:
                        _isCrouching = true;
                        break;
                }
                if (_isCrouching) _isRunning = false;
                break;
            case PlayerInputs.CamMode:
                switch (value)
                {
                    case InputValue.Up:
                        Camera.SetActive(true);
                        AimCamera.SetActive(false);
                        Crosshair.SetActive(false);
                        _isAiming = false;
                        break;
                    case InputValue.Down:
                        Camera.SetActive(false);
                        AimCamera.SetActive(true);
                        Crosshair.SetActive(true);
                        _isAiming = true;
                        break;
                }
                break;

        }

    }

    IEnumerator FallCrouch()
    {
        _isCrouching = true;
        _crouchHeight = FallCrouchHeight;
        yield return new WaitForSeconds(FallCrouchTime);
        _isCrouching = false;
        _crouchHeight = CrouchHeight;
        FallCrouchRoutine = null;
    }

    private void Jump()
    {
        if (!ApplyGravity && JumpRoutine == null && enablePhysics && enableMove)
        {
            ApplyGravity = true;
            _isJumping = true;
            _velocity = _animator.velocity * JumpDamp;
            _velocity.y = Mathf.Sqrt(2 * Gravity + JumpHeight);
        }
    }
    IEnumerator JumpDelayRoutine()
    {
        yield return new WaitForSeconds(JumpDelay);
        JumpRoutine = null;
    }

    Vector3 CalculateAirControl()
    {
        return ((transform.forward * _AnimationLerp.y) + (transform.right * _AnimationLerp.x)) * (AirControl / 100);
    }

    public void AddForce(Vector3 force)
    {
        if (!ApplyGravity)
        {
            ApplyGravity = true;
            _velocity.y = 0;
        }
        _velocity += Vector3.up * _stepDown;
        _velocity += force;
        _stepDown = 0;
    }

    public void AddRelativeForce(Vector3 force)
    {
        if (!ApplyGravity)
        {
            ApplyGravity = true;
            _velocity.y = 0;
        }
        _velocity += Vector3.up * _stepDown;
        _velocity += transform.right * force.x + transform.up * force.y + transform.forward * force.z;
        _stepDown = 0;
    }
    public void ToggleGodMode()
    {
        godMode = !godMode;
        RootMotion = !godMode;
        _isJumping = false;
        _isCrouching = false;
        Speed = 0.5f;
        _velocity = Vector3.zero;
        Debug.Log("God Mode " + (godMode ? "On" : "Off"));
    }
}


