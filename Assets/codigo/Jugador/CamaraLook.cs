using UnityEngine;
using UnityEngine.InputSystem;

public class CamaraLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;

    private float xRotation = 0f;

    void Start()
    {
        // Locks cursor to screen center and hides it
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerBody == null && transform.parent != null)
        {
            playerBody = transform.parent;
        }
    }

    void Update()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null) return;

        // Read raw mouse Delta values from Input System
        Vector2 mouseDelta = mouse.delta.ReadValue();

        float mouseX = mouseDelta.x * mouseSensitivity * Time.deltaTime;
        float mouseY = mouseDelta.y * mouseSensitivity * Time.deltaTime;

        // Pitch (Looking Up/Down) - Clamped so screen doesn't flip upside down
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Yaw (Looking Left/Right) - Rotates the Player Body horizontally
        if (playerBody != null)
        {
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }
}