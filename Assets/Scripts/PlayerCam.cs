using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCam : MonoBehaviour
{
    public float sensX;
    public float sensY;
    public Rigidbody playerRb;
    public Transform orientation;
    public PlayerMovement playerMovement;
    public float smallLeanTilt = 8f;
    public float bigLeanTilt = 13f;
    public float tiltSpeed = 5f;
    public float bigTiltThreshold = 33f;
    public float currentTilt = 8f;
    private float speed;
    private float xRotation;
    private float yRotation;
    private float currentTiltZ;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        speed = playerRb.linearVelocity.magnitude;
        // Get mouse input
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        float mouseX = mouseDelta.x * sensX * 0.01f;
        float mouseY = mouseDelta.y * sensY * 0.01f;
        
        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        print(playerMovement.MoveSpeed);

        if (playerMovement.MoveSpeed > bigTiltThreshold)
        {
            currentTilt = bigLeanTilt;
        }
        else
        {
            currentTilt = smallLeanTilt;
        }
        float targetTiltZ = -playerMovement.CurrentLeanInput * currentTilt;
        currentTiltZ = Mathf.Lerp(currentTiltZ, targetTiltZ, Time.deltaTime * tiltSpeed);
        // Rotate cam and orientation
        transform.rotation = Quaternion.Euler(xRotation, yRotation, currentTiltZ);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        Camera.main.fieldOfView = Mathf.Lerp(60, 60+speed, 100);


    }
}

