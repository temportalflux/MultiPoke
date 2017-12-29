/* Author: Jake Ruth
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterType
{
    NORMAL,
    FIRE,
    WATER,
    GRASS
}

public enum MoveKind
{
    PHYSICAL,
    SPECIAL,
    STATUS
}

[Serializable]
[CreateAssetMenu(menuName = "Asset/ Create New Attack", order = 100)]
public class AttackObject : ScriptableObject
{
    public string attackName;
    public MonsterType type;
    public MoveKind physicalOrSpecial;
    public bool doesDamage;
    public int power;

    public bool doesModifiesStats;
    public bool doesModifyOpponentStats; // if true, affect opponent, else affect user
    public int modifyAttackValue;
    public int modifyDefenseValue;
    public int modifySpecialAttackValue;
    public int modifySpecialDefenseValue;
    public int modifyAccuracyValue;
    public int modifySpeedValue;

    public int accuracy;
    public int powerPoints;

    public void UseAttack()
    {
        
    }
}
