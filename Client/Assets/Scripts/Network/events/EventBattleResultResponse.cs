using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventBattleResultResponse : EventNetwork
{

    [BitSerialize(1)]
    public uint playerID;

    public EventBattleResultResponse(byte id) : base(id)
    {
    }

    public EventBattleResultResponse() : this((byte)ChampNetPlugin.MessageIDs.ID_BATTLE_RESULT_RESPONSE)
    {
    }

    public EventBattleResultResponse(uint playerID) : this()
    {
        this.playerID = playerID;
    }

    public static void Dispatch(uint playerID)
    {
        NetInterface.INSTANCE.Dispatch(new EventBattleResultResponse(playerID));
    }

}
