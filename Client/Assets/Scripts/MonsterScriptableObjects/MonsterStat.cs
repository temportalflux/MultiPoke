/* Author: Jake Ruth
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Asset/ Create new Monster", order = 100)]
public class MonsterStat : ScriptableObject
{
    public uint id;
    [Header("Monster Stats")]
    public string monsterName;
	public Sprite monsterPicture;
    public List<MonsterType> types;
    [Space]
    public int maxHp;
    public int attack;
    public int defense;
    public int specialAttack;
    public int specialDefense;
    public int speed;
    [Space]
    public List<AttackObject> availableAttacks;

    void OnValidate()
    {
        if (types.Count > 2)
        {
            types.RemoveAt(types.Count - 1);
            Debug.LogWarning("Types can only be a max of two");
        }

        if (availableAttacks.Count > 4)
        {
            availableAttacks.RemoveAt(types.Count - 1);
            Debug.LogWarning("available attacks can only be a max of four");
        }
        
    }
}
