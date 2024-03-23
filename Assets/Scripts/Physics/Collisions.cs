using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Collisions
{
    public static bool AreColliding(OBB obb1, OBB obb2)
    {
        // Get corners
        Vector3[] corners1 = obb1.GetCorners();
        Vector3[] corners2 = obb2.GetCorners();

        // Check for overlaps -- OBB 1
        for (int i = 0; i < 3; i++)
        {
            if (!IsAxisOverlapped(corners1, corners2, obb1.GetAxis(i))) return false;
        }

        // Check for overlaps - - OBB 2
        for (int i = 0; i < 3; i++)
        {
            if (!IsAxisOverlapped(corners1, corners2, obb2.GetAxis(i))) return false;
        }

        // Check the perpendicular axes
        for (int i = 0; i < 3; i++) // OBB1 axes
        {
            for (int j = 0; j < 3; j++) // OBB2 axes
            {
                Vector3 axis = Vector3.Cross(obb1.GetAxis(i), obb2.GetAxis(j));
                if (!IsAxisOverlapped(corners1, corners2, axis)) return false;
            }
        }

        return true; // Collision
    }

    // Looking at corner points on axis for overlap
    private static bool IsAxisOverlapped(Vector3[] corners1, Vector3[] corners2, Vector3 axis)
    {
        // Do all corner points -- do float max/min for comparisons
        float min1 = float.MaxValue;
        float max1 = float.MinValue;
        float min2 = float.MaxValue;
        float max2 = float.MinValue;

        foreach (Vector3 corner in corners1)
        {
            float projection = Vector3.Dot(corner, axis);
            min1 = Mathf.Min(min1, projection);
            max1 = Mathf.Max(max1, projection);
        }

        foreach (Vector3 corner in corners2)
        {
            float projection = Vector3.Dot(corner, axis);
            min2 = Mathf.Min(min2, projection);
            max2 = Mathf.Max(max2, projection);
        }

        // Overlap
        float distance = Mathf.Abs((min1 + max1) / 2f - (min2 + max2) / 2f);
        float totalSize = Mathf.Max(max1 - min1, max2 - min2);

        return distance <= totalSize;
    }
}
