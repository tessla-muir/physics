using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CollisionHandling : MonoBehaviour
{
    List<OBB_Object> allObjects = new List<OBB_Object>();

    void Start()
    {
        // Find collidables
        allObjects = GameObject.FindObjectsOfType<OBB_Object>().ToList();
    }

    void LateUpdate()
    {
        foreach (OBB_Object obb in allObjects)
        {
            foreach (OBB_Object otherOBB in allObjects)
            {
                // Make sure they're not the same
                if (obb.gameObject.name != otherOBB.gameObject.name)
                {
                    // See if they're colliding
                    if (Collisions.AreColliding(obb.obb, otherOBB.obb))
                    {
                        Collisions.HandleCollision(obb, otherOBB);
                    }
                }
            }
        }
    }
}
