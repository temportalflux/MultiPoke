using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
     * Event: Some player requests battle with another
     */
public class EventBattleRequest : EventBattle
{

    public EventBattleRequest() : base((byte)ChampNetPlugin.MessageIDs.ID_BATTLE_REQUEST) { }

    public EventBattleRequest(uint sender, uint receiver) : this()
    {
        this.idSender = sender;
        this.idReceiver = receiver;
    }

    public override void Execute()
    {
        // if in battle then refuse battle request
        if (GameManager.INSTANCE.state.players[this.idReceiver].inBattle)
        {
            // Some user (requester) has asked us (receiver) to battle
            Debug.Log("Received request to battle from " + this.idSender + "... auto Declinning");
            NetInterface.INSTANCE.Dispatch(new EventBattleResponse(this.idReceiver, this.idSender, false));
        }
        // if not in battle then prompt user to answer battle request
        else
        {
            // ask player for answer and then from answer send response
            // tldr: delete whats below and create a menu that recieves player answer to battle
            Debug.Log("Received request to battle from " + this.idSender);

            // saves ID's for when player has to choose yes or no
            GameManager.INSTANCE.setResponseIDs(this.idReceiver, this.idSender);

            //NetInterface.INSTANCE.Dispatch(new EventBattleResponse(this.idReceiver, this.idSender, true));
        }
    }
}
