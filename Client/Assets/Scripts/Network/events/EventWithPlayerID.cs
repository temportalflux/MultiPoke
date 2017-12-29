using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventWithPlayerID : EventNetwork
{

    [BitSerialize(1)]
    public uint clientID;

    [BitSerialize(2)]
    public uint playerID;

    public EventWithPlayerID(byte id) : base(id)
    {
    }

}
