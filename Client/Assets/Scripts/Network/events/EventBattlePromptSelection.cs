using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventBattlePromptSelection : EventNetwork
{

    [BitSerialize(1)]
    public uint playerAID;

    [BitSerialize(2)]
    public int _playerASelection;

    public GameState.Player.EnumBattleSelection playerASelection
    {
        get
        {
            return (GameState.Player.EnumBattleSelection)this._playerASelection;
        }
    }

    [BitSerialize(3)]
    public int playerAChoice;

    [BitSerialize(4)]
    public uint playerBID;

    [BitSerialize(5)]
    public int _playerBSelection;

    public GameState.Player.EnumBattleSelection playerBSelection
    {
        get
        {
            return (GameState.Player.EnumBattleSelection)this._playerBSelection;
        }
    }

    [BitSerialize(6)]
    public int playerBChoice;

    public EventBattlePromptSelection() : base((byte)ChampNetPlugin.MessageIDs.ID_BATTLE_PROMPT_SELECTION)
    {
    }

    public override void Execute()
    {
        base.Execute();

        GameObject gameObjectWithTag = GameObject.FindGameObjectWithTag("BattleHandler");
        if (gameObjectWithTag != null)
        {
            BattleHandler battleHandler = gameObjectWithTag.GetComponent<BattleHandler>();
            bool playerAisParticipant1 = battleHandler.participant1.playerController.playerID == playerAID;
            if (playerAisParticipant1)
            {
                battleHandler.participant1.selectionChoice = playerAChoice;
                battleHandler.participant1.selection = (GameState.Player.EnumBattleSelection) _playerASelection;

                battleHandler.participant2.selectionChoice = playerBChoice;
                battleHandler.participant2.selection = (GameState.Player.EnumBattleSelection)_playerBSelection;

                battleHandler.StartCoroutine(battleHandler.HandleResponse(battleHandler.participant1,
                    battleHandler.participant2));
            }
            else
            {
                battleHandler.participant2.selectionChoice = playerAChoice;
                battleHandler.participant2.selection = (GameState.Player.EnumBattleSelection)_playerASelection;

                battleHandler.participant1.selectionChoice = playerBChoice;
                battleHandler.participant1.selection = (GameState.Player.EnumBattleSelection)_playerBSelection;

                battleHandler.StartCoroutine(battleHandler.HandleResponse(battleHandler.participant1,
                    battleHandler.participant2));
            }
        }
        else
        {
            //GameManager.INSTANCE.LoadBattleScene();
            BattleParticipant me = new BattleParticipant(GameManager.INSTANCE.state.players[playerAID], 0);
            BattleParticipant opponent = new BattleParticipant(GameManager.INSTANCE.state.players[playerBID], 0);
            GameManager.INSTANCE.LoadBattleScene(me, opponent, true);
        }
    }

}
