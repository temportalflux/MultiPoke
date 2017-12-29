using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
     * A base event to (de)serialize a (float,float) location
     */
public class EventWithLocation : EventWithPlayerID
{

    [BitSerialize(3)]
    public float posX;

    [BitSerialize(4)]
    public float posY;

    public EventWithLocation(byte id) : base(id)
    {
    }

}
