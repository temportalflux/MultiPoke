using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
* Some player's character needs to be updated
 * Client->Server: Request the user update a certain players physics
*/
public class EventRequestMovement : EventWithLocation
{
    
    [BitSerialize(5)]
    public float velX;

    [BitSerialize(6)]
    public float velY;

    public EventRequestMovement() : base((byte)ChampNetPlugin.MessageIDs.ID_PLAYER_REQUEST_MOVEMENT)
    {
    }

    public EventRequestMovement(uint clientID, uint playerID, float posX, float posY, float velX, float velY) : this()
    {
        this.clientID = clientID;
        this.playerID = playerID;
        this.posX = posX;
        this.posY = posY;
        this.velX = velX;
        this.velY = velY;
    }

}
