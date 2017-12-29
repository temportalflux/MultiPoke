/*
Names and ID: Christopher Brennan: 1028443, Dustin Yost: 0984932, Jacob Ruth: 0890406
Course Info: EGP-405-01 
Project Name: Project 3: Synchronized Networking
Due: 11/22/17
Certificate of Authenticity (standard practice): “We certify that this work is entirely our own.  
The assessor of this project may reproduce this project and provide copies to other academic staff, 
and/or communicate a copy of this project to a plagiarism-checking service, which may retain a copy of the project on its database.”
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPixelCorrector : MonoBehaviour
{

    public Vector2 targetViewportSizeInPixels = new Vector2(480.0f, 320.0f);
    public float pixelsPerUnit = 32.0f;
    private Camera _cam;
    private int _currentScreenWidth = 0;
    private int _currentScreenHeight = 0;
    //private float _pixelLockPPU = 32.0f;
    private Vector2 _winSize;

    void Start()
    {
        _cam = Camera.main;
        _cam.orthographic = true;

        resizeCamToTargetSize();
    }

    private void resizeCamToTargetSize()
    {
        if (_currentScreenWidth != Screen.width || _currentScreenHeight != Screen.height)
        {
            Vector2 percentage = new Vector2(Screen.width / targetViewportSizeInPixels.x, Screen.height / targetViewportSizeInPixels.y);
            float targetSize = 0;
            targetSize = percentage.x > percentage.y ? percentage.y : percentage.x;

            int targetSizeFloor = Mathf.FloorToInt(targetSize);

            if (targetSizeFloor < 1)
                targetSizeFloor = 1;

            float camSize = ((Screen.height / 2) / targetSizeFloor / pixelsPerUnit);
            _cam.orthographicSize = camSize;
            //_pixelLockPPU = targetSizeFloor * pixelsPerUnit;
        }
        _winSize = new Vector2(Screen.width, Screen.height);
    }

    void Update()
    {
        if (_winSize.x != Screen.width || _winSize.y != Screen.height)
            resizeCamToTargetSize();
    }
}
