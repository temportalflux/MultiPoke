using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Event: A battle has finished and the entire world needs to know about it
 */
public class EventBattleResult : EventBattle
{

    [BitSerialize(3)]
    public uint playerIDWinner;

    public EventBattleResult() : base((byte)ChampNetPlugin.MessageIDs.ID_BATTLE_RESULT)
    {
    }

    public EventBattleResult(uint winner, uint loser) : this()
    {
        this.idSender = this.playerIDWinner = winner;
        this.idReceiver = loser;
    }

    public override void Execute()
    {
        GameState.Player sender = GameManager.INSTANCE.state.players[this.idSender];
        GameState.Player receiver = GameManager.INSTANCE.state.players[this.idReceiver];
        GameState.Player winner = GameManager.INSTANCE.state.players[this.playerIDWinner];

        Debug.Log(string.Format("Battle between {0}({1}) and {2}({3} was won by {4}({5})",
            sender.name, sender.playerID, receiver.name, receiver.playerID, winner.name, winner.playerID));

        if (receiver.playerID == sender.playerID)
        {
            EventBattleLocalToggle.Dispatch(sender.localID, false);
        }
        else
        {
            EventBattleResultResponse.Dispatch(sender.isLocal ? sender.playerID : receiver.playerID);
        }

        GameManager.INSTANCE.UnloadBattleScene();
    }

    public static void Dispatch(uint winner, uint loser)
    {
        NetInterface.INSTANCE.Dispatch(new EventBattleResult(winner, loser));
    }

}
