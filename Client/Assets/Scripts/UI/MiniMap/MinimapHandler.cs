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

public class MinimapHandler : MonoBehaviour
{
    public Camera MinimapCamera;
    private Transform _localPlayer;
    private Coroutine _lookForPlayerCoroutine;

    void Start ()
    {
        if (GameManager.INSTANCE == null)
            return;

        // Find A local player on start
        foreach (KeyValuePair<uint, GameState.Player> pair in GameManager.INSTANCE.state.localPlayers)
        {
            GameState.Player p = pair.Value;
            if (p.isLocal)
            {
                _localPlayer = p.objectReference.transform;
                break;
            }
        }

        // if no player is found, keep looking for one overtime in coroutine
        _lookForPlayerCoroutine = StartCoroutine(LookForLocalPlayer(0.5f));
    }

    private IEnumerator LookForLocalPlayer(float timeBetweenChecks)
    {
        while (_localPlayer == null)
        {
            yield return new WaitForSeconds(timeBetweenChecks);

            foreach (KeyValuePair<uint, GameState.Player> pair in GameManager.INSTANCE.state.localPlayers)
            {
                GameState.Player p = pair.Value;
                if (p.isLocal)
                {
                    _localPlayer = p.objectReference.transform;
                    break;
                }
            }
        }

        _lookForPlayerCoroutine = null;
    }

    void Update ()
    {
        if (_localPlayer != null)
        {
            MinimapCamera.transform.position = _localPlayer.transform.position + Vector3.back * 10;
        }
        else
        {
            if (GameManager.INSTANCE == null)
                return;

            if (_lookForPlayerCoroutine == null)
                _lookForPlayerCoroutine = StartCoroutine(LookForLocalPlayer(0.5f));
        }
	}
}
