using UnityEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(CharacterController))]
public class FPController : MonoBehaviour
{

    [Header("Movement Parameters")]
    public float MaxSpeed => SprintInput ? SprintSpeed : WalkSpeed;
    public float Acceleration = 15f;

    [SerializeField] float WalkSpeed = 3.5f;
    [SerializeField] float SprintSpeed = 8f;

    [SerializeField] float JumpHeight = 2f;
    public bool Sprinting
    {
        get
        {
            return SprintInput && CurrentSpeed > 0.1f;
        }
    }

    [Header("Looking Parameters")]
    public Vector2 LookSensitivity = new Vector2(0.1f, 0.1f);

    public float PitchLimit = 85f;
    [SerializeField] float currentPitch = 0f;

    public float CurrentPitch
    {
        get => currentPitch;

        set
        {
            currentPitch = Mathf.Clamp(value, -PitchLimit, PitchLimit);
        }
    }

    [Header("Canera Parameters")]
    [SerializeField] float CameraNormalFOV = 60f;
    [SerializeField] float CameraSprintFOV = 80f;
    [SerializeField] float CameraFOVSmoothing = 1f;

    [Header("Crouch & Slide Parameters")]
    public float CrouchHeight = 1f;
    public float StandHeight = 2f;

    public float CrouchSpeed = 2f;
    public float SlideStartForce = 10f;
    public float SlideDecay = 8f;

    private bool IsCrouching = false;
    private bool IsSliding = false;

    private Vector3 slideVelocity;

    private Vector3 originalCenter;

    [Header("Crouch Camera")]
    [SerializeField] private float cameraStandHeight = 1.8f;
    [SerializeField] private float cameraCrouchHeight = 1f;
    [SerializeField] private float cameraLerpSpeed = 10f;


    float TargetCameraFOV
    {
        get
        {
            return Sprinting ? CameraSprintFOV : CameraNormalFOV;
        }
    }

    [Header("Physics Paramters")]
    [SerializeField] float GravityScale = 3f;

    public float VerticalVelocity = 0f;
    public Vector3 CurrentVelocity { get; private set; }
    public float CurrentSpeed { get; private set; }

    public bool IsGrounded => characterController.isGrounded;

    [Header("Input")]
    public Vector2 MoveInput;
    public Vector2 LookInput;

    public bool SprintInput;

    [Header("Components")]
    [SerializeField] CinemachineCamera fpCamera;
    [SerializeField] CharacterController characterController;

    #region Unity Methods

    void OnValidate()
    {
        if (characterController == null)
        {
            characterController = GetComponent<CharacterController>();
        }
    }

    void Start()
    {
        originalCenter = characterController.center;
    }

    void Update()
    {
        MoveUpdate();
        LookUpdate();
        CameraUpdate();
    }

    #endregion

    #region Controller Methods

    public void TryJump()
    {
        if (IsGrounded == false)
        {
            return;
        }
        VerticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Physics.gravity.y * GravityScale);
    }

    void MoveUpdate()
    {
        Vector3 motion;

        // if sliding, ignore normal motion and use slide movement
        if (IsSliding)
        {
            motion = slideVelocity;
            slideVelocity = Vector3.Lerp(slideVelocity, Vector3.zero, SlideDecay * Time.deltaTime);

            // if slowed down enough, stop sliding (should probably change the 0.5f to a variable to change slide distances)
            if (slideVelocity.magnitude < 0.2f){
                IsSliding = false;
            }
        }
        else
        {
            // normal movement
            motion = transform.forward * MoveInput.y + transform.right * MoveInput.x;
            motion.y = 0f;
            motion.Normalize();

            // adjust speed when crouching
            float targetSpeed = IsCrouching ? CrouchSpeed : MaxSpeed;

            if (motion.sqrMagnitude >= 0.01f)
            {
                CurrentVelocity = Vector3.MoveTowards(CurrentVelocity, motion * targetSpeed, Acceleration * Time.deltaTime);
            }
            else
            {
                CurrentVelocity = Vector3.MoveTowards(CurrentVelocity, Vector3.zero, Acceleration * Time.deltaTime);
            }
        }

        if (IsGrounded && VerticalVelocity <= 0.01f)
        {
            VerticalVelocity = -3f;
        }
        else
        {
            // gravity handling since i'm not using rigidbody
            VerticalVelocity += Physics.gravity.y * GravityScale * Time.deltaTime;
        }

        Vector3 fullVelocity = new Vector3(CurrentVelocity.x, VerticalVelocity, CurrentVelocity.z);

        characterController.Move(fullVelocity * Time.deltaTime);

        //update speed
        CurrentSpeed = CurrentVelocity.magnitude;
    }

    void LookUpdate()
    {
        Vector2 input = new Vector2(LookInput.x * LookSensitivity.x, LookInput.y * LookSensitivity.y);

        //look up and down
        CurrentPitch -= input.y;

        fpCamera.transform.localRotation = Quaternion.Euler(CurrentPitch, 0f, 0f);

        //look left and right
        transform.Rotate(Vector3.up * input.x);
    }

    void CameraUpdate()
    {
        float targetFOV = CameraNormalFOV;

        if (Sprinting)
        {
            float speedRatio = CurrentSpeed / SprintSpeed;

            targetFOV = Mathf.Lerp(CameraNormalFOV, CameraSprintFOV, speedRatio);
        }

        fpCamera.Lens.FieldOfView = Mathf.Lerp(fpCamera.Lens.FieldOfView, targetFOV, CameraFOVSmoothing * Time.deltaTime);

        float targetCamHeight = IsCrouching ? cameraCrouchHeight : cameraStandHeight;

        Vector3 localPos = fpCamera.transform.localPosition;
        localPos.y = Mathf.Lerp(localPos.y, targetCamHeight, cameraLerpSpeed * Time.deltaTime);
        fpCamera.transform.localPosition = localPos;
    }

    #region Crouching & Sliding
    public void StartCrouch()
    {
        if (Sprinting && CurrentSpeed > WalkSpeed * 1.2f)
        {
            IsSliding = true;
            slideVelocity = CurrentVelocity + transform.forward * SlideStartForce;
        }

        IsCrouching = true;

        characterController.height = CrouchHeight;
        characterController.center = new Vector3(
            originalCenter.x,
            CrouchHeight / 2f,
            originalCenter.z);
    }

    public void StopCrouch()
    {
        // if (!CanStand())
        // {
        //     return;
        // }

        IsCrouching = false;
        IsSliding = false;

        characterController.height = StandHeight;
        characterController.center = originalCenter;
    }

    bool CanStand()
    {
        return !Physics.Raycast(transform.position, Vector3.up, StandHeight - CrouchHeight + 0.1f);
    }

    #endregion

    #endregion
}
