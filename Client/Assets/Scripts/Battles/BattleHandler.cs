using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

/// \addtogroup client
/// @{

/// <summary>
/// 
/// </summary>
/// <author>Jake Ruth</author>
public class BattleHandler : MonoBehaviour
{
    public enum BattleState
    {
        WAITING_ON_INPUT,
        DISPLAYING_TURN
    }
 
    [Header("Transform Dependencies")]
    public BattleUIController battleUIController;

    /// <summary>
    /// Indicates that the current battle is between networked players
    /// </summary>
    public bool isNetworked;
    
    public BattleParticipant participant1;
    public BattleParticipant participant2;

    /// <summary>
    /// Called To set up the battle scene. Must be called at the start
    /// </summary>
    /// <param name="first">The local player</param>
    /// <param name="second">Other opponent player</param>
    /// <param name="isNetworkedBattle">Is the battle a networked battle or not</param>
    public void SetUpBattle(BattleParticipant first, BattleParticipant second, bool isNetworkedBattle)
    {
        isNetworked = isNetworkedBattle;
        this.participant1 = first;
        this.participant2 = second;

        participant1.CurrentHP = int.MaxValue;
        participant2.CurrentHP = int.MaxValue;

        if(participant1.isPlayer() && participant1.playerController.isLocal)
            battleUIController.UpdateCretinDisplayData(participant1, participant2);
        else
            battleUIController.UpdateCretinDisplayData(participant2, participant1);
    }

    /// <summary>
    /// Used to send a battle selection
    /// </summary>
    /// <param name="isLocalPlayer">if is <code>true</code> then the selection is from the "local" player (local = main player, not AI, not networked)</param>
    /// <param name="selection">The selection option</param>
    /// <param name="selectionIndex">the index of the selection</param>
    /// <returns>returns <code>False</code> if the selection index is not positive</returns>
    [ToDo("add networked portion here")]
    public bool SendBattleOption(bool isLocalPlayer, GameState.Player.EnumBattleSelection selection, uint selectionIndex)
    {
        if (selectionIndex < 0)
            return false;
        
        // This is a networked battle, so all logic on when players have gone is handled via server
        if (isNetworked)
        {
            BattleParticipant localControlled = null, remoteControlled = null;
            if (participant1.playerController.isLocal)
            {
                localControlled = participant1;
                remoteControlled = participant2;
            }
            else if (participant2.playerController.isLocal)
            {
                localControlled = participant2;
                remoteControlled = participant1;
            }

            if (localControlled != null && remoteControlled != null)
            {
                NetInterface.INSTANCE.Dispatch(new EventBattleSelection(localControlled.playerController.playerID, remoteControlled.playerController.playerID, selection, selectionIndex));
            }
        }
        // This is a battle between a player and AI, so everything is handled locally
        else
        {
            // The executor is the player
            if (isLocalPlayer)
            {
                this.participant1.selection = selection;
                this.participant1.selectionChoice = (int)selectionIndex;
            }
            // The executor is the AI
            else
            {
                this.participant2.selection = selection;
                this.participant2.selectionChoice = (int)selectionIndex;
            }
            // Both have picked their selection
            if (this.participant1.selectionChoice != -1 && this.participant2.selectionChoice != -1)
            {
                StartCoroutine(HandleResponse(this.participant1, this.participant2));
            }

            if (isLocalPlayer)
            {
                // AI now needs to make decision
                this.submitAIDecision(this.participant2);
            }

        }

        return true;
    }

    private void submitAIDecision(BattleParticipant ai)
    {
        GameState.Player.EnumBattleSelection selection;

        selection = GameState.Player.EnumBattleSelection.ATTACK;

        switch (selection)
        {
            case GameState.Player.EnumBattleSelection.ATTACK:
                uint selectionIndex = (uint)Random.Range(
                    0, ai.currentCretin.GetAvailableAttacks.Count
                ) + 1;
                this.SendBattleOption(false, selection, selectionIndex);
                break;
            default:
                break;
        }
    }

    private void getLocalNetwork(BattleParticipant p1, BattleParticipant p2, out BattleParticipant local,
        out BattleParticipant networkOrAI)
    {
        bool p1IsNetworkOrAI = p1.playerController == null || !p1.playerController.isLocal;
        local = !p1IsNetworkOrAI ? p1 : p2;
        networkOrAI = p1IsNetworkOrAI ? p1 : p2;
    }


    [ToDo("handle an ai death (add monster)")]
    public IEnumerator HandleResponse(BattleParticipant p1, BattleParticipant p2)
    {
        BattleParticipant local, networkOrAI;
        getLocalNetwork(p1, p2, out local, out networkOrAI);
        local.selectionChoice -= 1;
        networkOrAI.selectionChoice -= 1;

        bool hasFlee = false;

        // Check to see if either selection was to flee
        if (local.selection == GameState.Player.EnumBattleSelection.FLEE)
        {
            // Local keeper fled
            battleUIController.SetFlavorText("You Fled the battle");
            yield return  new WaitForSeconds(2.0f);

            this.BattleIsOver(local, networkOrAI);
            hasFlee = true;
        }

        if (networkOrAI.selection == GameState.Player.EnumBattleSelection.FLEE)
        {
            // other keeper fled
            battleUIController.SetFlavorText("Your opponent Fled the battle");
            yield return new WaitForSeconds(2.0f);

            this.BattleIsOver(networkOrAI, local);
            hasFlee = true;
        }

        // Calculate who attacks first
        bool localCreitenIsFaster = local.currentCretin.GetSpeed > networkOrAI.currentCretin.GetSpeed;

        if (this.isNetworked)
        {
            if (local.selection == GameState.Player.EnumBattleSelection.FLEE && local.selection == networkOrAI.selection)
            {
                // Both fled
                uint winner = local.playerController.playerID;
                uint loser = networkOrAI.playerController.playerID;
                // Winner = lowest playerID
                if (winner > loser)
                {
                    uint tmp = winner;
                    winner = loser;
                    loser = tmp;
                }
                // If I am winner
                if (local.playerController.playerID == winner)
                {
                    EventBattleResult.Dispatch(winner, loser);
                }
            }
        }

        if (hasFlee) yield break;

        // Check to see if either selection was to switch
        if (local.selection == GameState.Player.EnumBattleSelection.SWAP)
        {
            local.swapCretinTo(local.selectionChoice);
            // local keeper swapped a creiten
            battleUIController.SetFlavorText("You swapped in " + local.currentCretin.GetMonsterName);
            yield return  new WaitForSeconds(2.0f);
        }

        if (networkOrAI.selection == GameState.Player.EnumBattleSelection.SWAP)
        {
            networkOrAI.swapCretinTo(networkOrAI.selectionChoice);
            // other keeper swapped a creiten
            battleUIController.SetFlavorText("Your opponent swapped in " + networkOrAI.currentCretin.GetMonsterName);
            yield return new WaitForSeconds(2.0f);
        }

        List<BattleParticipant> faintedParticipants = new List<BattleParticipant>();
        // check to see if either selection was to attack, ordered based on certin speed
        if (localCreitenIsFaster)
        {
            // Check "local" first
            if (local.selection == GameState.Player.EnumBattleSelection.ATTACK)
            {
                ApplyAttack(local, networkOrAI, local.selectionChoice);

                battleUIController.SetFlavorText(string.Format("{0} Used {1}", local.currentCretin.GetMonsterName,
                    local.currentCretin.GetAvailableAttacks[local.selectionChoice].attackName));
                yield return new WaitForSeconds(2.0f);

                if (networkOrAI.CurrentHP <= 0)
                {
                    faintedParticipants.Add(networkOrAI);
                }

                battleUIController.UpdateCretinDisplayData(local, networkOrAI);
            }
            // then check other (network or AI)
            if (networkOrAI.selection == GameState.Player.EnumBattleSelection.ATTACK && faintedParticipants.Count == 0)
            {
                ApplyAttack(networkOrAI, local, networkOrAI.selectionChoice);

                battleUIController.SetFlavorText(string.Format("{0} Used {1}", networkOrAI.currentCretin.GetMonsterName,
                    networkOrAI.currentCretin.GetAvailableAttacks[networkOrAI.selectionChoice].attackName));
                yield return new WaitForSeconds(2.0f);

                if (local.CurrentHP <= 0)
                {
                    faintedParticipants.Add(local);
                }

                battleUIController.UpdateCretinDisplayData(local, networkOrAI);
            }
        }
        else
        {
            if (networkOrAI.selection == GameState.Player.EnumBattleSelection.ATTACK)
            {
                ApplyAttack(networkOrAI, local, networkOrAI.selectionChoice);

                battleUIController.SetFlavorText(string.Format("{0} Used {1}", networkOrAI.currentCretin.GetMonsterName,
                    networkOrAI.currentCretin.GetAvailableAttacks[networkOrAI.selectionChoice].attackName));
                yield return new WaitForSeconds(2.0f);

                if (local.CurrentHP <= 0)
                {
                    faintedParticipants.Add(local);
                }

                battleUIController.UpdateCretinDisplayData(local, networkOrAI);
            }

            if (local.selection == GameState.Player.EnumBattleSelection.ATTACK && faintedParticipants.Count == 0)
            {
                ApplyAttack(local, networkOrAI, local.selectionChoice);

                battleUIController.SetFlavorText(string.Format("{0} Used {1}", local.currentCretin.GetMonsterName,
                    local.currentCretin.GetAvailableAttacks[local.selectionChoice].attackName));
                yield return new WaitForSeconds(2.0f);

                if (networkOrAI.CurrentHP <= 0)
                {
                    faintedParticipants.Add(networkOrAI);
                }

                battleUIController.UpdateCretinDisplayData(local, networkOrAI);
            }
        }

        foreach (BattleParticipant participant in faintedParticipants)
        {
            string message = participant.currentCretin.GetMonsterName;

            message += " has fainted!!!";
            battleUIController.SetFlavorText(message);
            yield return new WaitForSeconds(2.0f);

            if (participant.isPlayer())
            {
                {
                    message = participant.playerController.name;
                    message += " has no more health, and they have lost the battle.";
                    battleUIController.SetFlavorText(message);
                    yield return new WaitForSeconds(2.0f);

                    if (participant == local)
                    {
                        BattleIsOver(local, networkOrAI);
                    }
                    else
                    {
                        BattleIsOver(networkOrAI, local);
                    }
                }
            }
            else
            {
                message = "The battle is over.";
                battleUIController.SetFlavorText(message);
                yield return new WaitForSeconds(2.0f);
                BattleIsOver(networkOrAI, local);
            }
        }
        
        battleUIController.SetFlavorText("Waiting for response...");

        local.selectionChoice = networkOrAI.selectionChoice = -1;

        battleUIController.menuState = MenuState.MAIN_MENU;
    }

    /// <summary>
    /// Set the winner and loser of the battle
    /// </summary>
    /// <param name="loser">The losing participant</param>
    /// <param name="winner">the winning participant</param>
    private void BattleIsOver(BattleParticipant loser, BattleParticipant winner)
    {
        if (isNetworked)
        {
            if (winner.playerController.isLocal)
            {
                EventBattleResult eventBattleResult =
                    new EventBattleResult
                    {
                        idSender = winner.playerController.playerID,
                        idReceiver = loser.playerController.playerID,
                        playerIDWinner = winner.playerController.playerID
                    };

                NetInterface.INSTANCE.Dispatch(eventBattleResult);
            }
        }
        else
        {
            if (winner.isPlayer())
            {
                EventPlayerAddMonster.Dispatch(winner.playerController.playerID, loser.currentCretin.monsterStat.id);
            }

            uint playerID = (winner.isPlayer() ? winner : loser).playerController.playerID;
            EventBattleResult.Dispatch(playerID, playerID);
            //GameManager.INSTANCE.UnloadBattleScene();
        }
    }


    private void ApplyAttack(BattleParticipant attacker, BattleParticipant receiver, int attackIndex)
    {
        MonsterDataObject localKeeperMonster = null, otherKeeperMonster = null;

        AttackObject attack = attacker.currentCretin.GetAvailableAttacks[attackIndex];
        
        // The damage the attack will ultimately do
        float damage;

        // The level of the monster
        // for our sake, every monster will be level 50 for now
        float level = 50;

        // the power of the attack being used
        float power = attack.power;

        // the effective attack stat of the user
        float attackStat = attacker.currentCretin.GetMonsterAttackStat(attack.physicalOrSpecial, true);

        // the effective defense stat of the reciever
        float defenseStat = receiver.currentCretin.GetMonsterAttackStat(attack.physicalOrSpecial, true);

        // a modifier if the attack would target multiple monsters (I.E. a doubles battle)
        // for our sake, because there is always one target so the value will be 1
        float targets = 1;

        // a modifier if the attack type matches a particlular weather pattern
        // for our sake, the weather is always clear so the value will be 1
        float weather = 1;

        // a modifier if the attack was a critical hit
        // for our sake, the critical hit will be 1
        float critical = 1;

        // a modifier between 0.85 and 1
        // for our sake, the value will be 1
        float random = 1;

        // a modifier for the Same-Type Attack Bonus (STAB)
        // 1.5 if the type of the attack matches one of the user's types, otherwise 1
        float stab = 1;
        foreach (MonsterType type in attacker.currentCretin.GetTypes)
        {
            if (type == attack.type)
            {
                stab = 1.5f;
                break;
            }
        }

        // a modifier to the attack's type effectiveness, the default is 1
        // the value is halfed, doubled, or multiplied by 0 depending on type match ups from the attack type and the other's types.
        float typeAdvantage = 1;
        foreach (MonsterType type in receiver.currentCretin.GetTypes)
        {
            typeAdvantage *= GetTypeEffectivenes(attack.type, type);
        }
        
        // a modifier to half the attack if the user is burned, and is a physical move
        // for our sake, there is no burning so the value is 1
        float burn = 1;

        // a modifier that is affected by very specific attacks, abilities, or items
        // for our sake, the value is 1
        float other = 1;

        float modifier = targets * weather * critical * random * stab * typeAdvantage * burn * other;

        // didnt work for some reason....
        //damage = ((2 * level / 5 + 2) * power * attackStat * defenseStat / 50 + 2) * modifier;
        damage = 2 * level;
        damage /= 5;
        damage += 2;
        damage *= power;
        damage *= attackStat;
        damage /= defenseStat;
        damage /= 50.0f;
        damage += 2;
        damage *= modifier;

        /*
        Debug.Log("Attack: " + attack.attackName + "\t" +
                  "Total Damage: " + damage + "\t" +
                  "level: " + level + "\t" +
                  "power: " + power + "\n" +
                  "attackStat: " + attackStat + "\t" +
                  "defenseStat: " + defenseStat + "\t" +
                  "Total Modifier: " + modifier + "\t" +
                  "targets: " + targets + "\n" +
                  "weather: " + weather + "\t" +
                  "critical: " + critical + "\t" +
                  "random: " + random + "\t" +
                  "stab: " + stab + "\n" +
                  "typeAdvantage: " + typeAdvantage + "\t" +
                  "burn: " + burn + "\t" +
                  "other: " + other);
        //*/

        // Apply damage
        receiver.CurrentHP -= Mathf.FloorToInt(damage);
    }

    // Type effectiveness match up 2d array hard coded here. 2d arrays dont look nice in unity inspector
    // [attack type][monster type]
    public readonly float[][] typeMatchUp =
    {
        // monster:  normal fire    water   grass    attack 
        new float[] {1.0f,  1.0f,   1.0f,   1.0f}, // normal
        new float[] {1.0f,  1.0f,   0.5f,   2.0f}, // fire
        new float[] {1.0f,  2.0f,   1.0f,   0.5f}, // water
        new float[] {1.0f,  0.5f,   2.0f,   1.0f}, // grass
    };

    /// <summary>
    /// Used to get the type effectiveness match up
    /// </summary>
    /// <param name="attackType"></param>
    /// <param name="monsterType"></param>
    /// <returns></returns>
    private float GetTypeEffectivenes(MonsterType attackType, MonsterType monsterType)
    {
        return typeMatchUp[(int) attackType][(int) monsterType];
    }

    public void onPreExit()
    {
        if (!this.isNetworked)
        {
            this.participant1.onPreExit();
            this.participant2.onPreExit();
        }
    }
}
