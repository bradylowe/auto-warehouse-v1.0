using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowForklift : MonoBehaviour
{
    // Get user input toggles
    private bool zoomingOut = false;
    private bool zoomingIn = false;
    private bool panningLeft = false;
    private bool panningRight = false;

    // Reference of forklift
    public GameObject forklift;
    // Camera distance
    private float distance;
    private float panAngle;
    // Toggle to sky view
    private bool topView;
    
    // Start is called before the first frame update
    void Start()
    {
        // Initialize
        distance = 10f;
        panAngle = 0f;
        topView = false;
    }

    // Run each frame
    void Update()
    {
        // Zoom out
        if (Input.GetKeyDown("-") && distance < 50f)
            zoomingOut = true;
        // Zoom ins
        if (Input.GetKeyDown("=") && distance > 5f)
            zoomingIn = true;
        // Pan left/right
        if (Input.GetKeyDown("[")) panningLeft = true;
        if (Input.GetKeyDown("]")) panningRight = true;
        if (Input.GetKeyDown("\\")) topView = !topView;
    }

    // LateUpdate is called after Update each frame
    void LateUpdate()
    {
        // Zoom out/in
        if (zoomingOut)
        {
            distance += 5f;
            zoomingOut = false;
        }
        else if (zoomingIn)
        {
            distance -= 5f;
            zoomingIn = false;
        }

        // Pan left/right
        if (panningLeft)
        {
            panAngle += 10f;
            panningLeft = false;
        }
        else if (panningRight) 
        { 
            panAngle -= 10f;
            panningRight = false;
        }

        // Set camera where user wants it
        Vector3 pos = forklift.transform.position;
        if (topView)
        {
            // Set the position of the camera's transform
            pos.y = 30f + distance * 2f;
            transform.position = pos;
            // Set the angle to look down and turn with the lift
            transform.LookAt(forklift.transform);
            // Rotate with forklift from above
            transform.rotation = Quaternion.Euler(90f, forklift.transform.rotation.eulerAngles.y, forklift.transform.rotation.eulerAngles.z);
        }
        else
        {
            // Set the position of the camera's transform
            Vector3 offset = -forklift.transform.forward * distance;
            offset.y = distance / 3f;
            transform.position = forklift.transform.position + offset;
            transform.RotateAround(forklift.transform.position, -Vector3.up, panAngle);
            transform.LookAt(pos);
        }
    }
}
