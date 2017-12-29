using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventBattleSelection : EventBattle
{
    
    public uint playerId
    {
        get
        {
            return this.idSender;
        }
    }
    
    public uint playerIdOpponent
    {
        get
        {
            return this.idReceiver;
        }
    }

    [BitSerialize(3)]
    public uint _selection;

    public GameState.Player.EnumBattleSelection selection
    {
        set
        {
            this._selection = (uint)value;
        }
    }

    [BitSerialize(4)]
    public uint choice;

    public EventBattleSelection() : base((byte)ChampNetPlugin.MessageIDs.ID_BATTLE_SELECTION)
    {
    }

    public EventBattleSelection(uint playerID, uint opponentID, GameState.Player.EnumBattleSelection selection, uint choice) : this()
    {
        this.idSender = playerID;
        this.idReceiver = opponentID;
        this.selection = selection;
        this.choice = choice;
    }

}
