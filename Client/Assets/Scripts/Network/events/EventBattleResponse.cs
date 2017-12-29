using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
     * Event: Some player responds to a battle request from another player
     */
public class EventBattleResponse : EventBattle
{

    [BitSerialize(3)]
    public bool accepted;

    public EventBattleResponse() : base((byte)ChampNetPlugin.MessageIDs.ID_BATTLE_RESPONSE) { }

    public EventBattleResponse(uint sender, uint receiver, bool accepted) : this()
    {
        this.idSender = sender;
        this.idReceiver = receiver;
        this.accepted = accepted;
    }

    public override void Execute()
    {
        // This was sent back to the requester to notify them of the other's result, so receiver and requester are flipped
        //Debug.Log("Request from " + this.idReceiver + " was" + (this.accepted ? "" : " not ") + " accepted by " + this.idSender);
        if (!this.accepted)
        {
            Debug.Log("Battle rejected.");
        }
    }

}
