using UnityEngine;
using UnityEngine.InputSystem;

public class TestUIPress : MonoBehaviour
{
    public InputActionReference accion;

    void OnEnable()
    {
        accion.action.Enable();
        accion.action.performed += OnPress;
    }

    void OnDisable() => accion.action.performed -= OnPress;

    void OnPress(InputAction.CallbackContext c) => Debug.Log("UI PRESS detectado");
}