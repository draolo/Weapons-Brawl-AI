using UnityEngine;
using System.Collections;
using System;

public class CameraController : MonoBehaviour
{
    private Cinemachine.CinemachineVirtualCamera cam;
    private CinemachineCameraOffset offsetManager;
    public PlayerMovement playerMovementManager;
    //public GameObject player;                     //Public variable to store a reference to the player game object
    
    private Vector3 offsetToReach;                  //the distance beetween what we are looking and the transform
    private bool targetReach = true;                //have we reach our target offset?  
    //private Func<float> GetCameraZoom;        
    private bool isLooking = false;                 //are we looking around(flag)
    public float lookAroundSpeed = 100f;            //the speed of the camera when we look around
    private float zoom;                             //the target zoom to reach
    public float beginZoom = 5f;                    //the initial camera zooom
    public float cameraZoomSpeed = 5f;              //the speed of the transaciton when the zoom chagne N.B. this will not increase the zoom amount
    public float zoomChange = 20f;                  //the zoom change amount 
    public float scrollBoost = 5f;                  //zooming with mouse scroll must be faster
    public float maxZoomIn = 5f;                    //the minimum bound for our zoom
    public float maxZoomOut = 100f;                 //the maximum bound for our zoom
    public float cameraComebackSpeed = 15f;         //the speed of the camera when come back form a look around action
    public bool costantComebackSpeed=true;          //the comeback speed is costant or propotionated to the distance
    public float acceptableDinstanceToLag = 0.2f;   //as far as we are getting closer to the target but never able to reach the target when comeback speed is not constant we need to set a distance to say it's ok stop to coming closer
    public bool automaticCameraComeback = false; 
    void Start()
    {
        cam = transform.GetComponent<Cinemachine.CinemachineVirtualCamera>();
        offsetManager = transform.GetComponent<CinemachineCameraOffset>();
        zoom = beginZoom;
        cam.m_Lens.OrthographicSize = beginZoom;

    }



    void Update()
    {

    }
    // LateUpdate is called after Update each frame
    void LateUpdate()
    {

        MovementHandler();
        ZoomHandler();

    }

    private void ZoomHandler()
    {

        KeyZoom();
        MouseZoom(Input.GetAxis("Zoom In"));
        MouseZoom(Input.GetAxis("Zoom Out")*-1);
        zoom = Mathf.Clamp(zoom, maxZoomIn, maxZoomOut);
        float cameraZoomDifference = zoom - cam.m_Lens.OrthographicSize;
        cam.m_Lens.OrthographicSize += cameraZoomDifference * cameraZoomSpeed * Time.deltaTime;

    }

    void KeyZoom()
    {
        if (Input.GetKey(KeyCode.KeypadPlus))
        {
            zoom -= zoomChange * Time.deltaTime;
            //Debug.Log(zoom);
        }
        if (Input.GetKey(KeyCode.KeypadMinus))
        {
            zoom += zoomChange * Time.deltaTime;
            //Debug.Log(zoom);

        }

    }

    void MouseZoom(float delta)
    {
        if (delta < 0)
        {
            zoom -= zoomChange * Time.deltaTime * scrollBoost;
            //Debug.Log(zoom);
        }
        if (delta > 0)
        {
            zoom += zoomChange * Time.deltaTime * scrollBoost;
            //Debug.Log(zoom);
        }

    }
    
    public void OnGUI()
    {

        if (Event.current.type == EventType.ScrollWheel)
        {
            MouseZoom(Event.current.delta.y);
        }
    }

    private void MovementHandler()
    {

        if (Input.GetButtonDown("Look Around"))
        {   //right mouse button is down so we are looking around

            isLooking = true;
        }

        if (Input.GetButtonUp("Look Around"))
        {   //right mouse button has comed up so we heve stopped to look around
            if (automaticCameraComeback)
            {
                CameraComeback();
            }
            isLooking = false;
        }

        if (playerMovementManager!= null && playerMovementManager.isActiveAndEnabled && playerMovementManager.isMoving)
        {   //we are moving, better reset the camera offset
            offsetManager.m_Offset = new Vector3(0, 0, 0);
        }

        if (isLooking)
        {   //we are looking around let's look around
            LookAroundBehaviour();
        }
        else if (!targetReach)
        {   //the camera isn't where it should be, lets get it back smoothly
            ComebackBehaviour();   
        }

        if (Input.GetButtonDown("Reset Camera Position"))
        {
            CameraComeback();
        }

    }

    private void CameraComeback()
    {
        offsetToReach = new Vector3(0, 0, 0);
        targetReach = false;
    }

    private void LookAroundBehaviour()
    {
        if (Input.GetAxis("Mouse X") != 0)
        {
            offsetManager.m_Offset -= new Vector3(Input.GetAxisRaw("Mouse X") * Time.deltaTime * lookAroundSpeed,
                                        Input.GetAxisRaw("Mouse Y") * Time.deltaTime * lookAroundSpeed, 0.0f);
            
        }
       
    }

    private void ComebackBehaviour()
    {

        Vector3 actualOffset = offsetManager.m_Offset;
        Vector3 movingDir = (offsetToReach - actualOffset).normalized;
        Vector3 newOffset;
        float distance = Vector3.Distance(offsetToReach, actualOffset);

        if (distance > acceptableDinstanceToLag)
        {
            if (costantComebackSpeed)
            {
                newOffset = actualOffset + movingDir * cameraComebackSpeed * Time.deltaTime;
            }
            else
            {
                newOffset = actualOffset + movingDir * distance * cameraComebackSpeed * Time.deltaTime;
            }

            float distanceAfterMoving = Vector3.Distance(newOffset, offsetToReach);
            if (distanceAfterMoving > distance)
            {
                newOffset = offsetToReach;
                targetReach = true;
            }
            offsetManager.m_Offset = newOffset;
        }
        else
        {
            offsetManager.m_Offset = offsetToReach;
            targetReach = true;
        }
    }
}

