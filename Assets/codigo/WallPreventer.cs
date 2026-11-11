using Unity.XR.CoreUtils;
using UnityEngine;

[RequireComponent(typeof(CharacterController)), RequireComponent(typeof(XROrigin))]
public class WallPreventer : MonoBehaviour
{
    CharacterController m_CharacterController;
    Transform m_LocalHeadTransform;

    void Awake()
    {
        if (!TryGetComponent(out m_CharacterController) || !TryGetComponent(out XROrigin xrOrigin))
        {
            Debug.LogWarning("Missing Components.");
            return;
        }
        m_LocalHeadTransform = xrOrigin.Camera.transform;
    }

    void Update()
    {
        // Ajusta el centro del Character Controller a la posición de la cabeza en X y Z
        m_CharacterController.center = new Vector3(m_LocalHeadTransform.localPosition.x, m_CharacterController.center.y, m_LocalHeadTransform.localPosition.z);
        // Fuerza la actualización de la física del Character Controller
        m_CharacterController.SimpleMove(Vector3.zero);
    }
}