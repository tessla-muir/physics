using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    GameObject player;
    OBB_Object playerOBB;
    float movementSpeed = 0.008f;
    float rotationSpeed = 0.15f;

    void Start()
    {
        player = gameObject;
        playerOBB = gameObject.GetComponent<OBB_Object>();
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate Left
        if (Input.GetKey(KeyCode.A))
        {
            playerOBB.RotateOBB(new Vector3(0, -rotationSpeed, 0));
        }
        // Rotate Right
        else if (Input.GetKey(KeyCode.D))
        {
            playerOBB.RotateOBB(new Vector3(0, rotationSpeed, 0));
        }

        // Calculate movement direction based on current rotation
        Vector3 movementDirection = Vector3.zero;

        // Move forward
        if (Input.GetKey(KeyCode.W))
        {
            movementDirection += Vector3.forward;
        }
        // Move backward
        if (Input.GetKey(KeyCode.S))
        {
            movementDirection -= Vector3.forward;
        }

        // Normalize movement direction if needed
        if (movementDirection != Vector3.zero)
        {
            movementDirection.Normalize();
            playerOBB.TranslateOBB(playerOBB.obb.rotation * movementDirection * movementSpeed);
        }
    }
}
