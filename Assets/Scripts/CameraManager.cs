﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    // Manage Camera transition
    public Transform player;

    [Header("[Player Animation]")]
    public float playerAnimationSpeed;
    private Vector3 playerTargetPosition;


    [Header("[Camera Animation]")]
    [SerializeField]
    private Vector3 cameraVelocity = Vector3.zero;
    [SerializeField]
    private float cameraSmoothTime;
    //[HideInInspector]
    public bool cameraTransition;
    private Transform mainCamera;
    private Vector3 cameraTargetPosition;
    public bool nextDirectionRight;

    [SerializeField]
    private GameObject waypointsParent;
    private Transform[] allFrames;
    [SerializeField]
    private Transform currentFrame;
    public Transform[] currentWaypoints; // waypoints of current Frame, each currentWaypoint have a WayPointScript

    public static CameraManager instance = null;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    private void Start() {
        mainCamera = this.transform;
        cameraTransition = false;
        cameraTargetPosition = this.transform.position;
        allFrames = new Transform[waypointsParent.transform.childCount];
        int i = 0;
        foreach (Transform child in waypointsParent.transform)
        {
            allFrames[i] = child;
            i++;
            //child is your child transform
        }
        GetWaypoints();
    }

    private void GetWaypoints(){
        currentWaypoints = currentFrame.GetComponentsInChildren<Transform>();
    }

    // Update is called once per frame
    private void Update () {
        if (!cameraTransition)
        {
            foreach (Transform childWaypoint in currentWaypoints) // Check each waypoints in the frame
            {
                if (Vector2.Distance(player.position, childWaypoint.position) <= 0.2f && !cameraTransition)
                {
                    Waypoint waypoint = childWaypoint.GetComponent<Waypoint>();
                    cameraTransition = true;

                    cameraTargetPosition = waypoint.nextcameraPosition;
                    playerTargetPosition = waypoint.nextplayerPosition;
                    currentFrame = waypoint.nextFrame;
                    GetWaypoints();
                    //ActiveWaypoints(false);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (cameraTransition&&Vector2.Distance(mainCamera.position, cameraTargetPosition) >= 0.001f)
        {
            mainCamera.position = Vector3.SmoothDamp(mainCamera.position, cameraTargetPosition, ref cameraVelocity, cameraSmoothTime);
        }

        if (cameraTransition&&Vector2.Distance(player.position, playerTargetPosition) >= 0.2f)
        {
            if (nextDirectionRight)
                player.position += Vector3.right * Time.deltaTime * playerAnimationSpeed;
            else
                player.position -= Vector3.right * Time.deltaTime * playerAnimationSpeed;
        }

        if(Vector2.Distance(player.position, playerTargetPosition) < 0.2f)
        {
            cameraTransition = false;
            //ActiveWaypoints(true);
        }
    }

    private void ActiveWaypoints(bool value)
    {
        foreach (Transform frame in allFrames) // Check each waypoints in the frame
        {
            frame.gameObject.SetActive(value);
        }
    }
    
}


