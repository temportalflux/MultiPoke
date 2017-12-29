using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventClientLeft : EventNetwork
{

    [BitSerialize(1)]
    public uint clientID;

    public EventClientLeft() : base((byte)ChampNetPlugin.MessageIDs.ID_CLIENT_LEFT)
    {
    }

    public EventClientLeft(uint clientID) : this()
    {
        this.clientID = clientID;
    }

}
