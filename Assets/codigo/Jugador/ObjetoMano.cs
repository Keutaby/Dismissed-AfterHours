using UnityEngine;

public class ObjetoMano : MonoBehaviour
{
    void Update()
    {
        // Check if manoJugador has any child objects (like the cell phone)
        if (transform.childCount > 0)
        {
            foreach (Transform child in transform)
            {
                // Force local position and rotation
                child.localPosition = Vector3.zero;
                child.localRotation = Quaternion.identity;

                // Force scale back to 1
                if (child.localScale == Vector3.zero)
                {
                    child.localScale = Vector3.one;
                }

                // Ensure MeshRenderers inside the phone model are turned ON
                MeshRenderer[] renderers = child.GetComponentsInChildren<MeshRenderer>();
                foreach (MeshRenderer mr in renderers)
                {
                    mr.enabled = true;
                }
            }
        }
    }
}