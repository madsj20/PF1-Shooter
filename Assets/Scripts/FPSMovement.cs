using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

[RequireComponent(typeof(CharacterController))]
public class FPSMovement : NetworkBehaviour
{
    [Header("Camera")]
    public Transform playerRoot;
    public Transform playerCam;

    public float cameraSensitivity;

    float rotX;
    float rotY;

    [Header("Movement")]
    public CharacterController controller;
    public float speed;
    public Transform feet;
    Vector3 velocity;

    [Header("Input")]
    public InputAction move;
    public InputAction mouseX;
    public InputAction mouseY;

    private void OnEnable()
    {
        move.Enable();
        mouseX.Enable();
        mouseY.Enable();
    }

    void OnDisable()
    {
        move.Disable();
        mouseX.Disable();
        mouseY.Disable();
    }

    void Start()
    {
        /*if (isLocalPlayer)
        {
            playerCam.GetComponent<Camera>().enabled = false;
            return;
        }*/
        Cursor.lockState = CursorLockMode.Locked;

        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        /*if (isLocalPlayer)
        {
            return;
        }*/
        Vector2 mouseInput = new Vector2(mouseX.ReadValue<float>() * cameraSensitivity, mouseY.ReadValue<float>() * cameraSensitivity);
        rotX -= mouseInput.y;
        rotX = Mathf.Clamp(rotX, -90, 90);
        rotY += mouseInput.x;

        playerRoot.rotation = Quaternion.Euler(0f, rotY, 0f);
        playerCam.localRotation = Quaternion.Euler(rotX, 0f, 0f);

        //Player movement
        Vector2 moveInput = move.ReadValue<Vector2>();
        Vector3 moveVelocity = playerRoot.forward * moveInput.y + playerRoot.right * moveInput.x;
        controller.Move(moveVelocity * speed * Time.deltaTime);
        controller.Move(velocity * Time.deltaTime);
    }
}
