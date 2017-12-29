using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

/// <summary>
/// A base class for network events
/// https://msdn.microsoft.com/en-us/library/system.runtime.interopservices.unmanagedtype(v=vs.110).aspx
/// </summary>
/// <remarks>
/// Author: Dustin Yost
/// </remarks>
public class EventNetwork : ISerializing
{

    /// <summary>
    /// Creates an event from a packet identifier
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>Some EventNetwork object</returns>
    /// <remarks>
    /// Author: Dustin Yost
    /// </remarks>
    public static EventNetwork createEvent(int id)
    {
        //Debug.Log("Got event " + id);
        switch (id)
        {
            case (char)ChampNetPlugin.MessageIDs.ID_CONNECTION_LOST:
                return new EventDisconnected();
            case (char)ChampNetPlugin.MessageIDs.ID_CLIENT_CONNECTION_ACCEPTED:
                return new EventConnected();
            case (char)ChampNetPlugin.MessageIDs.ID_CLIENT_CONNECTION_REJECTED:
                return new EventConnectionRejected();
            case (char)ChampNetPlugin.MessageIDs.ID_DISCONNECT:
                return new EventDisconnected();
            case (char)ChampNetPlugin.MessageIDs.ID_CLIENT_JOINED:
                return new EventClientJoined();
            case (char)ChampNetPlugin.MessageIDs.ID_CLIENT_LEFT:
                return new EventClientLeft();
            case (char)ChampNetPlugin.MessageIDs.ID_PLAYER_REQUEST_MOVEMENT:
                return new EventRequestMovement();
            case (char)ChampNetPlugin.MessageIDs.ID_UPDATE_GAMESTATE:
                return new EventGameState(ChampNetPlugin.MessageIDs.ID_UPDATE_GAMESTATE);
            ///*
            //case (char)ChampNetPlugin.MessageIDs.ID_USER_ID:
            //    return new EventNetwork.EventUserID();
            case (char)ChampNetPlugin.MessageIDs.ID_BATTLE_REQUEST:
                return new EventBattleRequest();
            case (char)ChampNetPlugin.MessageIDs.ID_BATTLE_RESPONSE:
                return new EventBattleResponse();
            case (char)ChampNetPlugin.MessageIDs.ID_BATTLE_PROMPT_SELECTION:
                return new EventBattlePromptSelection();
            case (char)ChampNetPlugin.MessageIDs.ID_BATTLE_OPPONENT_DISCONNECTED:
                return new EventBattleOpponentDisconnected();
            case (char)ChampNetPlugin.MessageIDs.ID_BATTLE_RESULT:
                return new EventBattleResult();
            //*/
            case (char)ChampNetPlugin.MessageIDs.ID_CLIENT_SCORE_UP:
                return new EventGameState(ChampNetPlugin.MessageIDs.ID_CLIENT_SCORE_UP);
            default:
                return new EventNetwork((byte)id);
        }
    }

    /// <summary>
    /// The event identifier (ChampNetPlugin::MessageIds)
    /// </summary>
    /// <remarks>
    /// Author: Dustin Yost
    /// </remarks>
    [BitSerialize(0)]
    public byte eventID;

    /// <summary>
    /// The amount of time it took to get the packet from source to destination
    /// </summary>
    public float deltaTime;
    
    public EventNetwork(byte id)
    {
        this.eventID = id;
    }

    /// <summary>
    /// Returns the size of the packet (the size for a potential byte[])
    /// </summary>
    /// <returns>the integer length of a byte array to hold this event's data</returns>
    /// <remarks>
    /// Author: Dustin Yost
    /// </remarks>
    virtual public int GetSize()
    {
        return 0;
    }

    /// <summary>
    /// Deserializes data from a byte array into this event's data
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="lastIndex">The last index.</param>
    /// <remarks>
    /// Author: Dustin Yost
    /// </remarks>
    virtual public void Deserialize(byte[] data, ref int lastIndex)
    {
    }
    
    /// <summary>
    /// Serializes data from this event into a byte array
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="lastIndex">The last index.</param>
    /// <remarks>
    /// Author: Dustin Yost
    /// </remarks>
    virtual public void Serialize(ref byte[] data, ref int lastIndex)
    {
    }
    
    /// <summary>
    /// Write some byte array into another byte array at some offset
    /// </summary>
    /// <param name="dest">The data to copy.</param>
    /// <param name="offset">The offset in bytes.</param>
    /// <param name="source">The data object to copy into at the offset.</param>
    /// <remarks>
    /// Author: Dustin Yost
    /// </remarks>
    public static void WriteTo(ref byte[] dest, ref int offset, byte[] source)
    {
        // copy all data from the source to the destination, starting at some offset
        System.Array.Copy(source, 0, dest, offset, source.Length);
        offset += source.Length;
    }

    /// <summary>
    /// Processes this event to affect the actual environment
    /// </summary>
    /// <remarks>
    /// Author: Dustin Yost
    /// </remarks>
    virtual public void Execute()
    {
        //ChampNetPlugin.MessageIDs message = (ChampNetPlugin.MessageIDs)this.id;
        //Debug.Log("Execute event with id: " + message + "(" + this.id + ")");
    }
      
}
