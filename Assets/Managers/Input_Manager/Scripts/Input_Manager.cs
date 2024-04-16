using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public class Input_Manager : MonoBehaviour
{
    private Player_Input_Actions playerInputs;
    public static Input_Manager _INPUT_MANAGER;

    private Vector2 leftAxisValue = Vector2.zero;

    private Vector2 mouseAxisValue = Vector2.zero;

    private float jumpButtonPressed = 0f;

    private float shootButtonPressed = 0f;

    private void Awake()
    {
        if (_INPUT_MANAGER != null && _INPUT_MANAGER != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            playerInputs = new Player_Input_Actions();
            playerInputs.Character.Enable();
            playerInputs.Character.Move.performed += LeftAxisUpdate;
            playerInputs.Character.Jump.performed += JumpButton;
            playerInputs.Character.Shoot.performed += ShootButton;

            _INPUT_MANAGER = this;
            DontDestroyOnLoad(this);
        }

    }
    private void Update()
    {
        jumpButtonPressed += Time.deltaTime;

        shootButtonPressed += Time.deltaTime;

        InputSystem.Update();
    }

    private void LeftAxisUpdate(InputAction.CallbackContext context)
    {
        leftAxisValue = context.ReadValue<Vector2>();
    }

    private void JumpButton(InputAction.CallbackContext context)
    {
        jumpButtonPressed = 0f;
    }

    private void ShootButton(InputAction.CallbackContext context)
    {
        shootButtonPressed = 0f;
    }

    public Vector2 GetLeftAxisUpdate()
    {
        return this.leftAxisValue;
    }

    public Vector2 GetMouseAxisUpdate()
    {
        return this.mouseAxisValue;
    }

    public bool GetJumpButton()
    {
        return this.jumpButtonPressed == 0f;
    }

    public bool GetShootButton()
    {
        return this.shootButtonPressed == 0f;
    }
}
