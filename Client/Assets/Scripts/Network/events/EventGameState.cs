using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Event: A gamestate update
/// </summary>
/// <remarks>
/// Author: Dustin Yost
/// </remarks>
public class EventGameState : EventNetwork
{

    public EventGameState(ChampNetPlugin.MessageIDs message) : base((byte)message)
    {
    }

    public EventGameState() : this(ChampNetPlugin.MessageIDs.ID_UPDATE_GAMESTATE)
    {
    }

    /// <summary>
    /// Deserializes data from a byte array into this event's data
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="lastIndex">The last index.</param>
    /// <remarks>
    /// Author: Dustin Yost
    /// </remarks>
    override public void Deserialize(byte[] data, ref int lastIndex)
    {
        base.Deserialize(data, ref lastIndex);
        GameManager.INSTANCE.state.deltaTime = this.deltaTime;
        GameManager.INSTANCE.state = (GameState)BitSerializeAttribute.Deserialize(GameManager.INSTANCE.state, data, lastIndex);
        //GameManager.INSTANCE.state.Deserialize(data, ref lastIndex);
    }

    /// <summary>
    /// Processes this event to affect the actual environment
    /// </summary>
    /// <remarks>
    /// Author: Dustin Yost
    /// </remarks>
    override public void Execute()
    {
        
    }

}
