using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Collisions
{
    // Determines if two obbs are colliding
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

    // Handles collision between two OBBs
    public static void HandleCollision(OBB_Object o1, OBB_Object o2)
    {
        Vector3 distanceVector = CalculateDistanceVector(o1.obb, o2.obb);

        // Move them apart -- position correction
        o1.TranslateOBB(distanceVector / 2f);
        o2.TranslateOBB(-distanceVector / 2f);

        Vector3 normal = (o1.obb.center - o2.obb.center).normalized;

        // Translational Impulse
        Vector3 relativeVelocity = (o1.momentum / o1.mass) - (o2.momentum / o2.mass);
        float relativeOnNormal = Vector3.Dot(relativeVelocity, normal);
        float impulse = (-(1 + 0.4f) * relativeOnNormal) / (o1.mass + o2.mass);

        o1.ApplyImpulse(-normal * impulse);
        o2.ApplyImpulse(normal * impulse);

        // Rotational Impulse
        Vector3 relativeAngularVelocity = (o1.angularMomentum / o1.mass) - (o2.angularMomentum / o2.mass);
        Vector3 angularImpulse = -(1 + 0.4f) * relativeAngularVelocity;
        o1.RotateOBB(-angularImpulse);
        o2.RotateOBB(angularImpulse);
        
        // Torque
        // Vector3 torque = Vector3.Cross(relativeAngularVelocity, normal);
        // o1.ApplyAngularImpulse(-torque);
        // o2.ApplyAngularImpulse(torque);
    }

    public static void HandleWallCollision(OBB_Object obbObject1, OBB_Object obbObject2)
    {
        // Determine which is the wall
        OBB_Object obbObject;
        OBB_Object wall;

        if (obbObject1.gameObject.tag == "Wall")
        {
            obbObject = obbObject2;
            wall = obbObject1;
        }
        else
        {
            obbObject = obbObject1;
            wall = obbObject2;
        }

        // Find the closest point on the wall to the object
        Vector3 closestPointOnWall = ClosestPointOnOBB(wall.obb, obbObject.transform.position);

        // Calculate the normal from the contact point on the wall to the object
        Vector3 wallNormal = CalculateNormalFromContactPoint(obbObject, wall);

        // Move the OBB away from the wall -- position correction
        obbObject.TranslateOBB(wallNormal / 2f);

        // Translational Impulse
        Vector3 relativeVelocity = obbObject.momentum; // Assuming the object has a velocity property
        float relativeOnNormal = Vector3.Dot(relativeVelocity, wallNormal);
        float impulse = (-(1 + 0.4f) * relativeOnNormal) / obbObject.mass;

        obbObject.ApplyImpulse(-wallNormal * impulse);

        // Rotational Impulse - assuming the wall does not impart angular velocity
        Vector3 relativeAngularVelocity = obbObject.angularMomentum;
        Vector3 angularImpulse = -(1 + 0.4f) * relativeAngularVelocity;
        obbObject.RotateOBB(-angularImpulse);
    }


    private static Vector3 CalculateDistanceVector(OBB obb1, OBB obb2)
    {
        Vector3 direction = obb1.center - obb2.center;

        float min1 = float.MaxValue;
        float max1 = float.MinValue;
        float min2 = float.MaxValue;
        float max2 = float.MinValue;

        Vector3[] corners1 = obb1.GetCorners();
        Vector3[] corners2 = obb2.GetCorners();

        foreach (Vector3 corner in corners1)
        {
            float projection = Vector3.Dot(corner - obb1.center, direction);
            min1 = Mathf.Min(min1, projection);
            max1 = Mathf.Max(max1, projection);
        }

        foreach (Vector3 corner in corners2)
        {
            float projection = Vector3.Dot(corner - obb2.center, direction);
            min2 = Mathf.Min(min2, projection);
            max2 = Mathf.Max(max2, projection);
        }

        float overlap = Mathf.Min(max1, max2) - Mathf.Max(min1, min2);
        overlap = Mathf.Max(overlap, 0f);

        return direction.normalized * overlap;
    }

    private static Vector3 CalculateNormalFromContactPoint(OBB_Object obbObject, OBB_Object wall)
    {
        // Find the closest point on the wall's surface to the object
        Vector3 closestPointOnWall = ClosestPointOnOBB(wall.obb, obbObject.transform.position);

        // Calculate the direction vector from the closest point on the wall to the object's center
        Vector3 directionToCenter = obbObject.transform.position - closestPointOnWall;

        // Calculate the normal vector based on the direction from the contact point to the object
        Vector3 normal = directionToCenter.normalized;

        // Rotate the normal according to the wall's rotation
        normal = wall.transform.rotation * normal;

        // Ensure that the normal points outward from the wall's surface
        if (Vector3.Dot(normal, Vector3.up) < 0)
        {
            normal *= -1;
        }

        return normal;
    }

    private static Vector3 ClosestPointOnOBB(OBB obb, Vector3 point)
    {
        // Transform the given point into the local coordinate space of the OBB
        Vector3 localPoint = Quaternion.Inverse(obb.rotation) * (point - obb.center);

        // Clamp the transformed point to the extents of the OBB along each axis
        Vector3 closestLocalPoint = Vector3.zero;
        for (int i = 0; i < 3; i++)
        {
            float extent = Vector3.Dot(localPoint, obb.GetAxis(i));
            extent = Mathf.Clamp(extent, -obb.size[i] / 2f, obb.size[i] / 2f);
            closestLocalPoint += obb.GetAxis(i) * extent;
        }

        // Transform the closest point back into the world coordinate space
        return obb.rotation * closestLocalPoint + obb.center;
    }
}
