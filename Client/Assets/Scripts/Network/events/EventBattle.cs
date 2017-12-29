using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventBattle : EventNetwork
{

    [BitSerialize(1)]
    public uint idSender;

    [BitSerialize(2)]
    public uint idReceiver;

    public EventBattle(byte id) : base(id)
    {
    }

}

