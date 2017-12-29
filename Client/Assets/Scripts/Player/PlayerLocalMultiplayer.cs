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

public class PlayerLocalMultiplayer : PlayerLocal
{

    protected override void Update()
    {
        base.Update();
        this.updateAxis(null, KeyCode.O, MappedAxis.Vertical, AxisDirection.Positive);
        this.updateAxis(null, KeyCode.K, MappedAxis.Horizontal, AxisDirection.Negative);
        this.updateAxis(null, KeyCode.L, MappedAxis.Vertical, AxisDirection.Negative);
        this.updateAxis(null, KeyCode.Semicolon, MappedAxis.Horizontal, AxisDirection.Positive);
    }

    private InputResponse.UpdateEvent getUpdateFor(KeyCode key)
    {
        if (Input.GetKeyDown(key))
        {
            return InputResponse.UpdateEvent.DOWN;
        }
        else if (Input.GetKeyUp(key))
        {
            return InputResponse.UpdateEvent.UP;
        }
        else if (Input.GetKey(key))
        {
            return InputResponse.UpdateEvent.TICK;
        }
        else
        {
            return InputResponse.UpdateEvent.NONE;
        }
    }

    private void updateAxis(InputDevice device, KeyCode key, MappedAxis axis, AxisDirection dir)
    {
        InputResponse.UpdateEvent evt = this.getUpdateFor(key);
        if (evt != InputResponse.UpdateEvent.NONE)
        {
            //Debug.Log(evt);
            this._pic.onAxis(device, evt, axis, dir);
            this._pic.onAxis(device, evt, axis, (float)dir);
            if (!GameManager.INSTANCE.state.isLocalGame)
            {
                this.requestMove();
            }
        }
    }

}
