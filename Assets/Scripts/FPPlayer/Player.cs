using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(FPController))]
public class Player : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] FPController FPController;
    [SerializeField] GunController GunController;
    [SerializeField] PauseMenu PauseMenu;
    bool isFiring;

    #region Input Handling

    void OnMove(InputValue value)
    {
        FPController.MoveInput = value.Get<Vector2>();
    }

    void OnLook(InputValue value)
    {
        FPController.LookInput = value.Get<Vector2>();
    }

    void OnSprint(InputValue value)
    {
        FPController.SprintInput = value.isPressed;
    }

    void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            FPController.TryJump();
        }
    }

    void OnCrouch(InputValue value)
    {
        Debug.Log("CROUCH INPUT: " + value.isPressed);
        if (value.isPressed)
        {
            FPController.StartCrouch();
        } else
        {
            FPController.StopCrouch();
        }
    }

    void OnAttack(InputValue value)
    {
        isFiring = value.isPressed;
        GunController.SetFiring(isFiring);
    }

    void OnReload(InputValue value)
    {
        if (value.isPressed)
        {
            GunController.Reload();
        }
    }

    void OnPause(InputValue value)
    {
        if (value.isPressed)
        {
            if (PauseMenu.isPaused)
            {
                PauseMenu.Instance.Resume();
            } else
            {
                PauseMenu.Instance.Pause();
            }
        }
    }

    #endregion

    #region Unity Methods

    void OnValidate()
    {
        if (FPController == null) FPController = GetComponent<FPController>();
        if (GunController == null) GunController = GetComponent<GunController>();
    }

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (isFiring)
        {
            GunController.TryShoot();
        }
    }
    #endregion

}
