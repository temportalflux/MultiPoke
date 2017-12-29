using System.Collections;
using System.Collections.Generic;
using System.Text;
using ChampNetPlugin;
using UnityEngine;

using Netty = ChampNetPlugin.Network;

/// \addtogroup client
/// @{

/// <summary>
/// The interface to work with the plugin to fetch and send packet data
/// </summary>
/// \author Dustin Yost
public class NetInterface : Singleton<NetInterface>
{

    /// <summary>
    /// The singleton instance
    /// </summary>
    private static NetInterface _instance = null;

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>
    /// The instance.
    /// </value>
    public static NetInterface INSTANCE { get {  return _instance; } }

    /// <summary>
    /// The server address
    /// </summary>
    private string serverAddress = "127.0.0.1";

    /// <summary>
    /// The server port
    /// </summary>
    private int serverPort = 425;
    
    /// <summary>
    /// The collection of events since last fetch
    /// </summary>
    private Queue<EventNetwork> events;

    void Awake()
    {
        // Load the singleton instance variable before we need to use it (Start)
        this.loadSingleton(this, ref NetInterface._instance);
        this.events = new Queue<EventNetwork>();
    }

    void Start()
    {
        Debug.Log("Creating network");
        Netty.Create();
        //Netty.SetDebugCallback();
    }

    void OnDestroy()
    {
        Debug.Log("Destroying network");
        this.Dispatch(new EventClientLeft(GameManager.INSTANCE.state.getClientID()));
        Netty.Destroy();
    }

    /// <summary>
    /// Starts the client, connects the specified address and port.
    /// </summary>
    /// <param name="address">The address.</param>
    /// <param name="port">The port.</param>
    public void Connect(string address, int port)
    {
        Netty.StartClient();
        this.serverAddress = address;
        this.serverPort = port;
        Netty.ConnectToServer(address, port);
    }

    /// <summary>
    /// Disconnects from the server.
    /// </summary>
    public void Disconnect()
    {
        Netty.Disconnect();
    }

    /// <summary>
    /// Update the network and process network events on a fixed timestep
    /// </summary>
    [ToDo("This can be pushed into separate coroutines with mutex")]
    private void FixedUpdate()
    {
        this.UpdateNetwork();
        this.ProcessEvents();
    }

    /// <summary>
    /// Fetch and process network updates.
    /// </summary>
    private void UpdateNetwork()
    {
        // Get all packets since last update
        Netty.FetchPackets();

        string address;
        byte[] data;
        ulong transmitTime;
        // Poll all packet data
        while (Netty.PollPacket(out address, out data, out transmitTime))
        {
            int id = (int)data[0];
            float deltaTime = transmitTime * 0.001f;
            //Debug.Log("Packet transmit at time " + transmitTime);
            // Handle all packets from the plugin interface
            this.HandlePacket(id, address, data, deltaTime);
        }
    }

    /// <summary>
    /// Handles each packet and pushes it into an event
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="address">The address.</param>
    /// <param name="data">The data.</param>
    private void HandlePacket(int id, string address, byte[] data, float transmitDeltaTime)
    {
        // Create the event to be processed
        EventNetwork evt = EventNetwork.createEvent(id);
        evt.deltaTime = transmitDeltaTime;
        // Read off the data of the packet
        BitSerializeAttribute.Deserialize(evt, data);
        // Push the event + data into the queue for processing
        events.Enqueue(evt);
    }

    /// <summary>
    /// Processes the events in the event queue.
    /// </summary>
    private void ProcessEvents()
    {
        // While there are events
        EventNetwork evt;
        while (this.PollEvent(out evt))
        {
            // execute them
            evt.Execute();
        }
    }

    /// <summary>
    /// Checks for events in the queue and returns the first if there are any.
    /// </summary>
    /// <param name="evt">The event.</param>
    /// <returns>true if there is an event</returns>
    private bool PollEvent(out EventNetwork evt)
    {
        // ensure the event out is always something
        evt = null;
        // check if there are events in the queue
        if (this.events.Count > 0)
        {
            // get the first event
            evt = this.events.Dequeue();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Dispatches the specified event.
    /// </summary>
    /// <param name="evt">The evt.</param>
    public void Dispatch(EventNetwork evt)
    {
        // Get the server key pair
        KeyValuePair<string, int> server = NetInterface.INSTANCE.getServer();
        // Call other dispatch with more details
        this.Dispatch(evt, server.Key, server.Value);
    }

    /// <summary>
    /// Dispatches the specified event.
    /// </summary>
    /// <param name="evt">The event.</param>
    /// <param name="address">The server address.</param>
    /// <param name="port">The server port.</param>
    public void Dispatch(EventNetwork evt, string address, int port)
    {
        byte[] data = BitSerializeAttribute.Serialize(evt);
        // Send the event to the address
        Netty.SendByteArray(address, port, data, data.Length);
    }

    /// <summary>
    /// Gets the server IP details.
    /// </summary>
    /// <returns>the server address/port</returns>
    public KeyValuePair<string, int> getServer()
    {
        return new KeyValuePair<string, int>(this.serverAddress, this.serverPort);
    }

}

/// @}
