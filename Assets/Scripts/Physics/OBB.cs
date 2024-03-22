using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct OBB
{
    public Vector3 center;
    public Quaternion rotation;
    public Vector3 size;

    public OBB(Vector3 center, Quaternion rotation, Vector3 size)
    {
        this.center = center;
        this.rotation = rotation;
        this.size = size;
    }

    public Vector3[] GetCorners()
    {
        // Calculate the corner points of the OBB
        Vector3[] corners = new Vector3[8];
        Vector3 extents = size / 2f;

        Quaternion normalizedRotation = rotation.normalized;
        Matrix4x4 localToWorldMatrix = Matrix4x4.TRS(center, normalizedRotation, Vector3.one);

        corners[0] = localToWorldMatrix.MultiplyPoint3x4(new Vector3(-extents.x, -extents.y, -extents.z));
        corners[1] = localToWorldMatrix.MultiplyPoint3x4(new Vector3(extents.x, -extents.y, -extents.z));
        corners[2] = localToWorldMatrix.MultiplyPoint3x4(new Vector3(extents.x, -extents.y, extents.z));
        corners[3] = localToWorldMatrix.MultiplyPoint3x4(new Vector3(-extents.x, -extents.y, extents.z));

        corners[4] = localToWorldMatrix.MultiplyPoint3x4(new Vector3(-extents.x, extents.y, -extents.z));
        corners[5] = localToWorldMatrix.MultiplyPoint3x4(new Vector3(extents.x, extents.y, -extents.z));
        corners[6] = localToWorldMatrix.MultiplyPoint3x4(new Vector3(extents.x, extents.y, extents.z));
        corners[7] = localToWorldMatrix.MultiplyPoint3x4(new Vector3(-extents.x, extents.y, extents.z));

        return corners;
    }
}