using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientedBoundingBox : MonoBehaviour
{
    [SerializeField] private Bounds boundingBox;

    private MeshFilter mesh;

    void Start()
    {
        // Get mesh
        mesh = gameObject.GetComponent<MeshFilter>();

        // Calculate inital box
        CalculateBoundingBox();
    }

    void Update()
    {
        // Recalculate the bounding box when mesh changes
        CalculateBoundingBox();
    }

    void CalculateBoundingBox()
    {
        // Get local vertices
        Vector3[] vertices = mesh.mesh.vertices;

        // Find local min and max
        Vector3 localMin = vertices[0];
        Vector3 localMax = vertices[0];
        foreach (Vector3 v in vertices)
        {
            localMin = Vector3.Min(localMin, v);
            localMax = Vector3.Max(localMax, v);
        }

        // Calculate local center and size
        Vector3 localCenter = (localMin + localMax) / 2f;
        Vector3 localSize = localMax - localMin;

        // Update local bounding box
        boundingBox.center = localCenter;
        boundingBox.size = localSize;
    }

    Vector3[] CalculateLocalCorners()
    {
        Vector3[] corners = new Vector3[8];
        corners[0] = boundingBox.center + new Vector3(-boundingBox.extents.x, -boundingBox.extents.y, -boundingBox.extents.z);
        corners[1] = boundingBox.center + new Vector3(boundingBox.extents.x, -boundingBox.extents.y, -boundingBox.extents.z);
        corners[2] = boundingBox.center + new Vector3(-boundingBox.extents.x, -boundingBox.extents.y, boundingBox.extents.z);
        corners[3] = boundingBox.center + new Vector3(boundingBox.extents.x, -boundingBox.extents.y, boundingBox.extents.z);
        corners[4] = boundingBox.center + new Vector3(-boundingBox.extents.x, boundingBox.extents.y, -boundingBox.extents.z);
        corners[5] = boundingBox.center + new Vector3(boundingBox.extents.x, boundingBox.extents.y, -boundingBox.extents.z);
        corners[6] = boundingBox.center + new Vector3(-boundingBox.extents.x, boundingBox.extents.y, boundingBox.extents.z);
        corners[7] = boundingBox.center + new Vector3(boundingBox.extents.x, boundingBox.extents.y, boundingBox.extents.z);
        return corners;
    }

    // Draws the obb outline (puts an X on top/bottom of the box for clarity)
    void OnDrawGizmos()
    {
        // Transform center to world space
        Vector3 worldCenter = transform.TransformPoint(boundingBox.center);

        // Create transform matrix
        Matrix4x4 transformMatrix = Matrix4x4.TRS(worldCenter,
                                                transform.rotation,
                                                transform.lossyScale);

        // Calculate local corners
        Vector3[] corners = CalculateLocalCorners();

        // Transform corners to world space
        for (int i = 0; i < corners.Length; i++)
        {
            corners[i] = transformMatrix.MultiplyPoint3x4(corners[i]);
        }

        // Draw the box edges
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(corners[0], corners[1]);
        Gizmos.DrawLine(corners[1], corners[3]);
        Gizmos.DrawLine(corners[3], corners[2]);
        Gizmos.DrawLine(corners[2], corners[0]);
        Gizmos.DrawLine(corners[4], corners[5]);
        Gizmos.DrawLine(corners[5], corners[7]);
        Gizmos.DrawLine(corners[7], corners[6]);
        Gizmos.DrawLine(corners[6], corners[4]);
        Gizmos.DrawLine(corners[0], corners[4]);
        Gizmos.DrawLine(corners[1], corners[5]);
        Gizmos.DrawLine(corners[2], corners[6]);
        Gizmos.DrawLine(corners[3], corners[7]);
    }
}