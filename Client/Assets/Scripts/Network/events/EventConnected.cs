using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Event: Notification that the client has been accepted to the server
/// </summary>
/// <remarks>
/// Author: Dustin Yost
/// </remarks>
public class EventConnected : EventNetwork
{

    public EventConnected() : base((byte)ChampNetPlugin.MessageIDs.ID_CLIENT_CONNECTION_ACCEPTED)
    {
    }

    /// <summary>
    /// Processes this event to affect the actual environment
    /// </summary>
    /// <remarks>
    /// Author: Dustin Yost
    /// </remarks>
    override public void Execute()
    {
        // Notify the game manger that the connection request has been satisfied
        GameManager.INSTANCE.onNetworkConnectionHandled.Invoke(true);
        // Tell the server we have connected
        NetInterface.INSTANCE.Dispatch(new EventClientJoined(GameManager.INSTANCE.state.playerRequest));
        // Begin the game in networked mode
        GameManager.INSTANCE.PlayNetwork();
    }

}

/// <summary>
/// Event: Notification that the client has been rejected from the server
/// </summary>
/// <remarks>
/// Author: Dustin Yost
/// </remarks>
public class EventConnectionRejected : EventNetwork
{

    public EventConnectionRejected() : base((byte)ChampNetPlugin.MessageIDs.ID_CLIENT_CONNECTION_REJECTED)
    {
    }

    /// <summary>
    /// Processes this event to affect the actual environment
    /// </summary>
    /// <remarks>
    /// Author: Dustin Yost
    /// </remarks>
    override public void Execute()
    {
        Debug.Log("Connection Rejected");
        GameManager.INSTANCE.onNetworkConnectionHandled.Invoke(false);
        GameManager.INSTANCE.onNetworkRejected.Invoke("Invalid server");
    }

}

public class EventDisconnected : EventNetwork
{

    public EventDisconnected() : base((byte)ChampNetPlugin.MessageIDs.ID_DISCONNECT)
    {
    }

    /// <summary>
    /// Processes this event to affect the actual environment
    /// </summary>
    /// <remarks>
    /// Author: Dustin Yost
    /// </remarks>
    override public void Execute()
    {
        Debug.Log("Server Disconnected");
        // Exit the game (we have been booted from the server)
        GameManager.INSTANCE.Exit();
    }

}
