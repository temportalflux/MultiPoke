
public class EventBattleLocalToggle : EventBattleResultResponse
{

    [BitSerialize(2)]
    public bool inBattle;

    public EventBattleLocalToggle() : base((byte)ChampNetPlugin.MessageIDs.ID_BATTLE_LOCAL_TOGGLE)
    {

    }

    public EventBattleLocalToggle(uint playerID, bool inBattle = true) : this()
    {
        this.playerID = playerID;
        this.inBattle = inBattle;
    }

    public static void Dispatch(uint playerID, bool inBattle)
    {
        NetInterface.INSTANCE.Dispatch(new EventBattleLocalToggle(playerID, inBattle));
    }

}
