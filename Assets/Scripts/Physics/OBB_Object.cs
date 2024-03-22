using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OBB_Object : MonoBehaviour
{
    public OBB obb;

    // This method is called in the editor whenever the script's properties are modified in the Inspector
    void OnValidate()
    {
        CalculateOBB();
    }

    void Start()
    {
        obb = CalculateOBB();
    }

    OBB CalculateOBB()
    {
        Vector3 center = transform.position;
        Quaternion rotation = transform.rotation;
        Vector3 size = Vector3.Scale(transform.localScale, ((BoxCollider)gameObject.GetComponent<Collider>()).size);

        return new OBB(center, rotation, size);
    }

    public void TranslateOBB(Vector3 translation)
    {
        obb.center += translation;
        gameObject.transform.position = obb.center;
    }

    public void RotateOBB(Quaternion rotationDelta)
    {
        obb.rotation *= rotationDelta;
        gameObject.transform.rotation = obb.rotation;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        // Calculate the corner points of the OBB
        Vector3[] corners = obb.GetCorners();

        // Draw lines connecting the corner points to form the outline of the OBB
        for (int i = 0; i < 4; i++)
        {
            Gizmos.DrawLine(corners[i], corners[(i + 1) % 4]);
            Gizmos.DrawLine(corners[i + 4], corners[((i + 1) % 4) + 4]);
            Gizmos.DrawLine(corners[i], corners[i + 4]);
        }
    }
}
