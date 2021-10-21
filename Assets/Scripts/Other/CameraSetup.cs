using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class CameraSetup : MonoBehaviour{
    //public GameObject mainCamera;
    public GameObject virtualCamera;

    // Use this for initialization

    void Start()
    {
        Cinemachine.CinemachineVirtualCamera virtualController = virtualCamera.GetComponent<Cinemachine.CinemachineVirtualCamera>();
        virtualController.m_Follow = this.transform;
    }



}
