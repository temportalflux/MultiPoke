using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventBattleOpponentDisconnected : EventBattle
{

    public EventBattleOpponentDisconnected() : base((byte)ChampNetPlugin.MessageIDs.ID_BATTLE_OPPONENT_DISCONNECTED) { }

    public EventBattleOpponentDisconnected(uint sender, uint receiver) : this()
    {
        this.idSender = sender;
        this.idReceiver = receiver;
    }

    public override void Execute()
    {
        // idSender has disconnected
        // tell server we (idreceiver) won
        EventBattleResult.Dispatch(this.idReceiver, this.idSender);
    }

}

