using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MonsterDataObject //: MonoBehaviour
{
    public MonsterStat monsterStat;

    public string GetMonsterName { get { return monsterStat.monsterName;} }
    public List<MonsterType> GetTypes { get { return monsterStat.types; } }
    //public int GetMaxHp { get { return monsterStat.maxHp; } }
    public int GetAttack { get { return monsterStat.attack; } }
    public int GetDefense { get { return monsterStat.defense; } }
    public int GetSpecialAttack { get { return monsterStat.specialAttack; } }
    public int GetSpecialDefense { get { return monsterStat.specialDefense; } }
    public int GetSpeed { get { return monsterStat.speed; } }
    public List<AttackObject> GetAvailableAttacks { get { return monsterStat.availableAttacks; } }

    // stat stages
    public int statStageAttack;
    public int statStageDefense;
    public int statStageSpecialAttack;
    public int statStageSpecialDefense;
    public int statStageSpeed;

    //private int _hp;

    //public int CurrentHP
    //{
    //    get { return _hp = Mathf.Clamp(_hp, 0, GetMaxHp); }
    //    set { _hp = Mathf.Clamp(value, 0, GetMaxHp); }
    //}

    MonsterDataObject()
    {

    }

    /// <summary>
    /// Get the attack stat based on the move kind
    /// </summary>
    /// <param name="physicalOrSpecial">is the move physical or special?</param>
    /// <param name="applyStatStages">if <code>true</code> then the stat stages are applied</param>
    /// <returns>the modified attack stat</returns>
    public int GetMonsterAttackStat(MoveKind physicalOrSpecial, bool applyStatStages = false)
    {
        int attackStat = physicalOrSpecial == MoveKind.PHYSICAL ? GetAttack : GetSpecialAttack;

        if (!applyStatStages)
            return attackStat;

        int statStage = physicalOrSpecial == MoveKind.PHYSICAL ? statStageAttack : statStageSpecialAttack;
        float statModifier = (2 + Mathf.Abs(statStage)) / 2.0f;
        if (statStage < 0)
            statModifier = 1 / statModifier;

        return (int) (attackStat * statModifier);
    }

    /// <summary>
    /// Get the defense stat based on the move kind.
    /// </summary>
    /// <param name="physicalOrSpecial">is the move physical or special?</param>
    /// <param name="applyStatStages">if <code>true</code> then the stat stages are applied</param>
    /// <returns>the modified defense stat</returns>
    public int GetMonsterDefenseStat(MoveKind physicalOrSpecial, bool applyStatStages = false)
    {
        int defenseStat = physicalOrSpecial == MoveKind.PHYSICAL ? GetDefense : GetSpecialDefense;

        if (!applyStatStages)
            return defenseStat;

        int statStage = physicalOrSpecial == MoveKind.PHYSICAL ? statStageDefense : statStageSpecialDefense;
        float statModifier = (2 + Mathf.Abs(statStage)) / 2.0f;
        if (statStage < 0)
            statModifier = 1 / statModifier;

        return (int) (defenseStat * statModifier);
    }
}
