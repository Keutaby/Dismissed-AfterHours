using UnityEngine;

public class ObjetoMano : MonoBehaviour
{
    void Update()
    {
        if (transform.childCount > 0)
        {
            foreach (Transform child in transform)
            {
                child.localPosition = Vector3.zero;
                child.localRotation = Quaternion.identity;

                if (child.localScale == Vector3.zero)
                {
                    child.localScale = Vector3.one;
                }

                MeshRenderer[] renderers = child.GetComponentsInChildren<MeshRenderer>();
                foreach (MeshRenderer mr in renderers)
                {
                    mr.enabled = true;
                }
            }
        }
    }
}