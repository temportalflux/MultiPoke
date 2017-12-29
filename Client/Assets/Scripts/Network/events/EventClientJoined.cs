using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Client->Server: Tell server that we joined
 * Server->Client: Tell the client of its ID and first playerID
 */
public class EventClientJoined : EventNetwork
{
    
    public ConnectMenu.PlayerDescriptor[] players;

    public EventClientJoined() : base((byte)ChampNetPlugin.MessageIDs.ID_CLIENT_JOINED)
    {
    }

    public EventClientJoined(ConnectMenu.PlayerDescriptor[] playerRequests): this()
    {
        this.players = playerRequests;
    }

    public override int GetSize()
    {
        return base.GetSize() + (
            sizeof(int) +
            this.players.Length * (sizeof(int) + (sizeof(char) * GameState.Player.SIZE_MAX_NAME) + (sizeof(float) * 3))
        );
    }

    public override void Serialize(ref byte[] data, ref int lastIndex)
    {
        base.Serialize(ref data, ref lastIndex);

        // write # of players
        WriteTo(ref data, ref lastIndex, System.BitConverter.GetBytes(this.players.Length));

        for (int localID = 0; localID < this.players.Length; localID++)
        {

            char[] chars = this.players[localID].name.ToCharArray();
            WriteTo(ref data, ref lastIndex, System.BitConverter.GetBytes(chars.Length));
            for (int i = 0; i < chars.Length; i++)
            {
                WriteTo(ref data, ref lastIndex, System.BitConverter.GetBytes(chars[i]));
            }

            WriteTo(ref data, ref lastIndex, System.BitConverter.GetBytes(this.players[localID].color.r));
            WriteTo(ref data, ref lastIndex, System.BitConverter.GetBytes(this.players[localID].color.g));
            WriteTo(ref data, ref lastIndex, System.BitConverter.GetBytes(this.players[localID].color.b));
        }

    }

}
