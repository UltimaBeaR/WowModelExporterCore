using UnityEngine;
using UnityEngine.EventSystems;

public class MouseCameraControl : MonoBehaviour
{
    public EventSystem EventSystem;

    private float zoomSpeed = 1.2f;
    private float moveSpeed = 0.01f;
    private float rotateSpeed = 8.0f;

    private GameObject orbitVector;

    // Use this for initialization
    void Start()
    {
        // Create a capsule (which will be the lookAt target and global orbit vector)
        orbitVector = new GameObject();
        orbitVector.transform.position = Vector3.zero;
    }

    void LateUpdate()
    {
        var isMouseOverUI = EventSystem.IsPointerOverGameObject();

        if (isMouseOverUI)
            return;

        var x = Input.GetAxis("Mouse X");
        var y = Input.GetAxis("Mouse Y");

        var wheelie = Input.GetAxis("Mouse ScrollWheel");

        if (wheelie < 0) // back
        {
            var currentZoomSpeed = 100f;
            transform.Translate(Vector3.forward * (wheelie * currentZoomSpeed));
        }
        if (wheelie > 0) // back
        {
            var currentZoomSpeed = 100f;
            transform.Translate(Vector3.forward * (wheelie * currentZoomSpeed));
        }

        //Input.GetAxis("Mouse ScrollWheel") < 0) // back
        if (Input.GetKey(KeyCode.RightAlt) || Input.GetKey(KeyCode.LeftAlt))
        {
            // Distance between camera and orbitVector. We'll need this in a few places
            var distanceToOrbit = Vector3.Distance(transform.position, orbitVector.transform.position);
            
            if (Input.GetMouseButton(1))
            {
                //RMB - ZOOM

                // Refine the rotateSpeed based on distance to orbitVector
                var currentZoomSpeed = Mathf.Clamp(zoomSpeed * (distanceToOrbit / 50), 0.1f, 2.0f);

                // Move the camera in/out
                transform.Translate(Vector3.forward * (x * currentZoomSpeed));

                // If about to collide with the orbitVector, repulse the orbitVector slightly to keep it in front of us
                if (Vector3.Distance(transform.position, orbitVector.transform.position) < 3)
                {
                    orbitVector.transform.Translate(Vector3.forward, transform);
                }
            }
            else if (Input.GetMouseButton(0))
            {
                //LMB - PIVOT

                // Refine the rotateSpeed based on distance to orbitVector
                var currentRotateSpeed = Mathf.Clamp(rotateSpeed * (distanceToOrbit / 50), 1.0f, rotateSpeed);

                // Temporarily parent the camera to orbitVector and rotate orbitVector as desired
                transform.parent = orbitVector.transform;
                orbitVector.transform.Rotate(Vector3.left * (y * currentRotateSpeed));
                orbitVector.transform.Rotate(Vector3.up * (x * currentRotateSpeed), Space.World);
                transform.parent = null;
            }
            else if (Input.GetMouseButton(2))
            {
                //MMB - PAN

                var speed = moveSpeed;

                speed = distanceToOrbit * speed;

                // Calculate move speed
                var translateX = Vector3.right * (x * speed) * -1;
                var translateY = Vector3.up * (y * speed) * -1;

                // Move the camera
                transform.Translate(translateX);
                transform.Translate(translateY);

                // Move the orbitVector with the same values, along the camera's axes. In effect causing it to behave as if temporarily parented.
                orbitVector.transform.Translate(translateX, transform);
                orbitVector.transform.Translate(translateY, transform);
            }
        }
    }
}