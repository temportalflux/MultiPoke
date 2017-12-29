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
using UnityEngine.SceneManagement;

public class CameraBlit : MonoBehaviour {

    private static CameraBlit _instance;

    public static void setMaterial(Material material)
    {
        CameraBlit._instance.materialBlit = material;
    }

    private Material materialBlit;

    private void Awake()
    {
        CameraBlit._instance = this;
    }

    private void Start()
    {
        this.materialBlit = null;
    }
    
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (this.materialBlit != null)
        {
            Graphics.Blit(source, destination, this.materialBlit);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }

}
