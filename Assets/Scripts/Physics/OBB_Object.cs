using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OBB_Object : MonoBehaviour
{
    public OBB obb;

    // Momentum stuff
    private float mass = 1.1f;
    private Vector3 momentum;
    private Vector3 angularMomentum;

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
        momentum += translation / mass;
    }

    public void RotateOBB(Vector3 angularVelocity)
    {
        angularMomentum += angularVelocity / mass;
    }

    void LateUpdate()
    {
        // Postion update through momentum and time
        obb.center += momentum * Time.fixedDeltaTime;
        gameObject.transform.position = obb.center;

        // Rotation update through angular momentum and time
        obb.rotation *= Quaternion.Euler(angularMomentum * Time.fixedDeltaTime);
        gameObject.transform.rotation = obb.rotation;

        momentum *= 0.99f;
        angularMomentum *= 0.99f;
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
