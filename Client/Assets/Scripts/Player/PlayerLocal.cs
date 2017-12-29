using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof( PlayerCharacterController))]
[RequireComponent(typeof(PlayerInputController))]
public class PlayerLocal : PlayerReference
{
    protected PlayerInputController _pic;

    protected override void Awake()
    {
        base.Awake();
        _pic = GetComponent<PlayerInputController>();
    }

    /// <summary>
    /// Challenges the random player to a networked battle
    /// </summary>
    /// <remarks>
    /// Author: Dustin Yost
    /// </remarks>
    public void challengeRandomPlayer()
    {
        PlayerReference player = GameManager.INSTANCE.getRandomPlayer(this.playerInfo);
        if (player != null)
        {
            this.requestBattle(player);
        }
    }

    /// <summary>
    /// Challenges another player to a networked battle
    /// </summary>
    /// <remarks>
    /// Author: Christopher Brennan
    /// </remarks>
    public void challengeNetworkedPlayer()
    {
        PlayerReference player = _pic.CharacterFacing(this.moveTarget.position);
        if (player != null)
        {
            this.requestBattle(player);
        }
    }

    public void onChallengeBy(MonsterDataObject opponentAI)
    {
        IList<MonsterDataObject> cretins = this.getInfo().monsters;
        if (cretins.Count > 0 && this.getInfo().canLocalBattle)
        {
            this.getInfo().canLocalBattle = false;
            EventBattleLocalToggle.Dispatch(this.getID(), true);
            BattleParticipant me = new BattleParticipant(this.getInfo(), 0);
            BattleParticipant opponent = new BattleParticipant(opponentAI);
            GameManager.INSTANCE.LoadBattleScene(me, opponent, false);
        }
    }

    /// <summary>
    /// Notifies the server of a battle request
    /// </summary>
    /// <param name="player">The player.</param>
    /// <remarks>
    /// Author: Dustin Yost
    /// </remarks>
    public void requestBattle(PlayerReference player)
    {
        NetInterface.INSTANCE.Dispatch(new EventBattleRequest(this.getID(), player.getID()));
    }

    public override void setInfo(GameState.Player info)
    {
        base.setInfo(info);
        this.GetComponent<InputResponse>().inputId = (int)info.localID + 1;
    }

    public void requestMove()
    {
        GameState.Player info = this.getInfo();
        
        if (info.inBattle) return;

        Vector3 position, deltaMove;
        _pic.Move(this.moveTarget.position, out position, out deltaMove);

        Vector3 velocity = _pic._input.normalized * _pcc.speed;

        //Debug.Log("Sending " + position);
        NetInterface.INSTANCE.Dispatch(new EventRequestMovement(
            info.clientID, info.playerID,
            this.moveTarget.position.x, this.moveTarget.position.y,
            velocity.x, velocity.y
        ));
    }

    protected override void Update()
    {
        base.Update();
        if (GameManager.INSTANCE.mainCamera != null)
        {
            Camera camera = this.GetComponentInChildren<Camera>(true);
            float camZ = camera.transform.position.z;
            Vector3 playerPos = this.transform.position;
            bool doUseCamera;
            Vector3 pos = GameManager.INSTANCE.mainCamera.SetPosition((int)this.getInfo().localID, playerPos, out doUseCamera);
            camera.gameObject.SetActive(doUseCamera);
            camera.transform.position = (doUseCamera ? pos.normalized * 2 + playerPos : this.transform.position) + Vector3.forward * camZ;
        }

        if (GameManager.INSTANCE.state.isLocalGame && this._pic != null)
        {
            Vector3 position, deltaMove;
            this._pic.Move(this.moveTarget.position, out position, out deltaMove);
            this.playerInfo.position += deltaMove;
        }

    }

    /// <summary>
    /// Only used to test scoreboard incrementation
    /// </summary>
    /// <param name="device"></param>
    /// <param name="type"></param>
    /// <param name="action"></param>
    /// <Author> Christopher Brennan </Author>
    public void down(InputDevice device, InputResponse.UpdateEvent type, MappedButton action)
    {

        GameState.Player info = this.getInfo();

        switch (action)
        {
            case MappedButton.DENY:
                //NetInterface.INSTANCE.Dispatch(new EventIncrementScore(info.clientID, info.playerID, info.wins));
                break;
            default:
                break;
        }
    }

}
