using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// \addtogroup client
/// @{

/// <summary>
/// Handles spawning enemies in tall grass areas
/// </summary>
public class TallGrass : MonoBehaviour
{

    /// <summary>
    /// A list of enemies to randomly pick from
    /// </summary>
    public MonsterDataObject[] randoms;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // And there is a valid enemy/opponent monster
        MonsterDataObject opponent = this.getRandomOpponent();
        if (opponent != null)
        {
            // And the player is non-null
            PlayerLocal player = collision.gameObject.GetComponent<PlayerLocal>();
            if (player != null)
            {
                player.resetDelay();
            }
        }
    }

    /// <summary>
    /// On <see cref="PlayerLocal"/> walk over...
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay2D(Collider2D other)
    {
        // If there is a successful 25% chance
        float chance = UnityEngine.Random.Range(0, 1.0f) * 1000;
        if (chance < 20)
        {
            // And there is a valid enemy/opponent monster
            MonsterDataObject opponent = this.getRandomOpponent();
            if (opponent != null)
            {
                // And the player is non-null
                PlayerLocal player = other.GetComponent<PlayerLocal>();
                if (player != null)
                {
                    // Challenge the player
                    player.onChallengeBy(opponent);
                }
            }
        }
        
    }

    /// <summary>
    /// Return a random monster/opponent from <see cref="randoms"/>.
    /// </summary>
    /// <returns><see cref="MonsterDataObject"/></returns>
    private MonsterDataObject getRandomOpponent()
    {
        return randoms.Length > 0 ? this.randoms[UnityEngine.Random.Range(0, this.randoms.Length)] : null;
    }

}
/// @}
