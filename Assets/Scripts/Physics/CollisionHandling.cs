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
        for (int i = 0; i < allObjects.Count; i++)
        {
            for (int j = i + 1; j < allObjects.Count; j++)
            {
                // Check if they're colliding
                if (Collisions.AreColliding(allObjects[i].obb, allObjects[j].obb))
                {
                    // Normal collision
                    if (allObjects[j].gameObject.tag != "Wall" && allObjects[i].gameObject.tag != "Wall")
                    {
                        Collisions.HandleCollision(allObjects[i], allObjects[j]);
                    }
                    else if (allObjects[j].gameObject.tag == "Wall" ^ allObjects[i].gameObject.tag == "Wall")
                    {
                        Collisions.HandleWallCollision(allObjects[i], allObjects[j]);
                    } 
                }
            }
        }
    }
}
