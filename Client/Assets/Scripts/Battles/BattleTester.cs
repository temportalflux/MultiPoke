using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author: Jake Ruth
public class BattleTester : MonoBehaviour
{
    public BattleHandler battleHandler;
    public BattleUIController battleUIController;

    public BattleParticipant localPlayerTest;
    public BattleParticipant otherPlayerTest;

    public MonsterDataObject monsterA;
    public MonsterDataObject monsterB;

    protected bool isBattleSetup;

    public bool IsBattleSetup
    {
        get { return isBattleSetup; }
        set { isBattleSetup = value; }
    }

    void Start()
    {
        IsBattleSetup = false;

        localPlayerTest = new BattleParticipant(monsterA);
        otherPlayerTest = new BattleParticipant(monsterB);
    }
}
