using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventPlayerAddMonster : EventNetwork
{

    [BitSerialize(1)]
    public uint playerID;

    [BitSerialize(2)]
    public uint monsterID;

    public EventPlayerAddMonster() : base((byte)ChampNetPlugin.MessageIDs.ID_PLAYER_ADD_MONSTER)
    {
    }

    public EventPlayerAddMonster(uint playerID, uint monsterID) : this()
    {
        this.playerID = playerID;
        this.monsterID = monsterID;
    }

    public static void Dispatch(uint playerID, uint monsterID)
    {
        NetInterface.INSTANCE.Dispatch(new EventPlayerAddMonster(playerID, monsterID));
    }

}

