using UnityEngine;
using UnityEngine.EventSystems;

public class MouseCameraControl : MonoBehaviour
{
    public EventSystem EventSystem;

    public Transform Target;

    private float zoomSpeed = 1.2f;
    private float moveSpeed = 0.015f;
    private float rotateSpeed = 5.0f;

    private GameObject TemporaryCameraParent { get; set; }
    private Camera Camera { get; set; }
    private Vector3 PivotPoint { get; set; }
    private bool IsPanStarted { get; set; }
    private bool IsZoomStarted { get; set; }
    private bool IsPivotStarted { get; set; }

    void Start()
    {
        TemporaryCameraParent = new GameObject();
    }

    void Awake()
    {
        Camera = Camera.main;
    }

    void LateUpdate()
    {
        var isMouseOverUI = EventSystem.IsPointerOverGameObject();

        float wheelie = 0;

        if (!isMouseOverUI)
        {
            if (Input.GetMouseButtonDown(0))
                IsPivotStarted = true;
            if (Input.GetMouseButtonDown(1))
                IsZoomStarted = true;
            if (Input.GetMouseButtonDown(2))
                IsPanStarted = true;

            wheelie = Input.GetAxis("Mouse ScrollWheel");
        }

        if (Input.GetMouseButtonUp(0))
            IsPivotStarted = false;
        if (Input.GetMouseButtonUp(1))
            IsZoomStarted = false;
        if (Input.GetMouseButtonUp(2))
            IsPanStarted = false;

        if (!IsPivotStarted && !IsZoomStarted && !IsPanStarted && wheelie == 0)
            return;

        if (Target != null)
        {
            PivotPoint = Target.transform.position;
        }
        else
        {
            // Пересчитываем точку опоры только при начале pivot/zoom/pan действия (когда кнопка мыши только нажалась)
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
                PivotPoint = Camera.transform.position + (Camera.transform.forward * 1f);
        }

        var x = Input.GetAxis("Mouse X");
        var y = Input.GetAxis("Mouse Y");

        if (wheelie != 0f)
        {
            var currentZoomSpeed = 3f;
            Camera.transform.Translate(Vector3.forward * (wheelie * currentZoomSpeed));
        }

        if (Input.GetKey(KeyCode.RightAlt) || Input.GetKey(KeyCode.LeftAlt))
        {
            var distanceToOrbit = Vector3.Distance(Camera.transform.position, PivotPoint);
            
            if (Input.GetMouseButton(0))
            {
                //LMB - PIVOT

                //var currentRotateSpeed = Mathf.Clamp(rotateSpeed * (distanceToOrbit / 50), 1.0f, rotateSpeed);

                TemporaryCameraParent.transform.position = PivotPoint;
                Camera.transform.parent = TemporaryCameraParent.transform;
                TemporaryCameraParent.transform.Rotate(Vector3.left * (y * rotateSpeed));
                TemporaryCameraParent.transform.Rotate(Vector3.up * (x * rotateSpeed), Space.World);
                Camera.transform.parent = null;
            }
            else if (Input.GetMouseButton(1))
            {
                //RMB - ZOOM

                var currentZoomSpeed = Mathf.Clamp(zoomSpeed * (distanceToOrbit / 50), 0.1f, 2.0f);

                Camera.transform.Translate(Vector3.forward * (x * currentZoomSpeed));
            }
            else if (Input.GetMouseButton(2))
            {
                //MMB - PAN

                var speed = moveSpeed;

                speed = distanceToOrbit * speed;

                var translateX = Vector3.right * (x * speed) * -1;
                var translateY = Vector3.up * (y * speed) * -1;

                Camera.transform.Translate(translateX);
                Camera.transform.Translate(translateY);
            }
        }
    }
}