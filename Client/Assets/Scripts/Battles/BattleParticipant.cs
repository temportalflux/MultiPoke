using UnityEngine;

/// \addtogroup client
/// @{

/// <summary>
/// Class to hold information about a participant in a battle.
/// Could be a local player, networked player, or cretin AI.
/// </summary>
public class BattleParticipant
{

    /// <summary>
    /// The Player info for the controlling player.
    /// Null if the controller is the opponent for a local player vs AI battle.
    /// </summary>
    public GameState.Player playerController;

    /// <summary>
    /// The current cretin in battle. If <see cref="playerController"/> is non-null,
    /// <see cref="currentCretin"/> = <code><see cref="playerController"/>.monsters[<see cref="currentCretinIndex"/>]</code>.
    /// </summary>
    public MonsterDataObject currentCretin;

    /// <summary>
    /// The current index of the cretin in the controller.
    /// -1 if the controller is the opponent for a local player vs AI battle.
    /// </summary>
    public int currentCretinIndex;

    /// <summary>
    /// The latest selection of the player/AI.
    /// </summary>
    public GameState.Player.EnumBattleSelection selection;

    /// <summary>
    /// The latest selection choice of the player/AI.
    /// </summary>
    public int selectionChoice = -1;

    /// <summary>
    /// Instantiate the participant as a player, networked or local.
    /// </summary>
    /// <param name="player">The player state.</param>
    /// <param name="cretin">The cretin index in <code><see cref="GameState.Player"/>.monsters</code></param>
    public BattleParticipant(GameState.Player player, int cretin)
    {
        this.playerController = player;
        this.currentCretinIndex = cretin;
        this.fetchCurrentCretin();
    }

    public int maxHP = 100;

    private int _hp;
    public int CurrentHP
    {
        get { return _hp = Mathf.Clamp(_hp, 0, maxHP); }
        set { _hp = Mathf.Clamp(value, 0, maxHP); }
    }

    /// <summary>
    /// Instantiate the participant as a cretin AI.
    /// </summary>
    /// <param name="cretinAI">The monster stats for the battling cretin.</param>
    public BattleParticipant(MonsterDataObject cretinAI)
    {
        this.playerController = null;
        this.currentCretinIndex = -1;
        this.currentCretin = cretinAI;
    }

    /// <summary>
    /// Gets the cretin object from the current index.
    /// </summary>
    private void fetchCurrentCretin()
    {
        this.currentCretin = this.playerController.monsters[this.currentCretinIndex];
    }

    /// <summary>
    /// Swaps the current cretin index for PLAYER participants.
    /// Performs <see cref="fetchCurrentCretin"/> once the index is set.
    /// </summary>
    /// <param name="index">The new index for the cretin.</param>
    public void swapCretinTo(int index)
    {
        // Can only be performed on players
        Debug.Assert(this.isPlayer());
        this.currentCretinIndex = index;
        this.fetchCurrentCretin();
    }

    /// <summary>
    /// Checks if the participant is a player.
    /// </summary>
    /// <returns><code>True</code>, if <see cref="playerController"/> is non-null.</returns>
    public bool isPlayer()
    {
        return this.playerController != null;
    }

    public void onPreExit()
    {
        if (this.isPlayer())
        {
            // send packet to server saying done battle
            NetInterface.INSTANCE.Dispatch(new EventBattleResultResponse(this.playerController.playerID));
        }
    }

}
/// @}
