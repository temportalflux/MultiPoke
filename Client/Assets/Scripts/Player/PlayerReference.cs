using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReference : MonoBehaviour {

	protected PlayerCharacterController _pcc;

    public Transform sprite;
    public SpriteRenderer overlay;
    public Transform moveTarget;

    private Animator _anim;
    //private Vector3 screenPos;

    /// <summary>
    /// Score of the player
    /// </summary>
    public uint score
    {
        get
        {
            return this.playerInfo.wins;
        }
        set
        {
            this.playerInfo.wins = value;
        }
    }

    /// <summary>
    /// Current Rank of the player.
    /// Ask @Hypocripe (Chris Brennan) about how its sorted.
    /// </summary>
    public int rank
    {
        get
        {
            return this.playerInfo.rank;
        }
        set
        {
            this.playerInfo.rank = value;
        }
    }

    protected virtual void Awake()
    {
        this._pcc = GetComponent<PlayerCharacterController>();
    }

    protected GameState.Player playerInfo;

    /// <summary>
    /// The player identifier
    /// </summary>
    private uint playerID
    {
        get
        {
            return this.playerInfo.playerID;
        }
        set
        {
            this.playerInfo.playerID = value;
        }
    }

    /// <summary>
    /// Gets the identifier.
    /// </summary>
    /// <returns></returns>
    public uint getID()
    {
        return this.playerID;
    }

    public void setID(uint id)
    {
        this.playerID = id;
    }

    /// <summary>
    /// Update the object to have some transform properties
    /// </summary>
    /// <param name="posX">The position x.</param>
    /// <param name="posY">The position y.</param>
    /// <param name="velX">The rot z.</param>
    /// <param name="velY"></param>
    /// <remarks>
    /// Author: Dustin Yost
    /// </remarks>
    public void integrateInfo(GameState.Player playerInfo, bool forceSnap = false)
    {
        this.playerInfo = playerInfo;
        
        this.moveTarget.position = this.playerInfo.position;
        if (forceSnap)
        {
            this.transform.position = this.moveTarget.position;
        }
        
        //this.sprite.rotation = Quaternion.Euler(0, 0, rotZ);

        this.overlay.color = this.playerInfo.color;
        //transform.Find("MiniMapIcon").GetComponent<SpriteRenderer>().color = this.playerInfo.color;

        Transform t = this.transform.Find("MiniMapIcon");
        SpriteRenderer s = t.GetComponent<SpriteRenderer>();
        s.color = this.playerInfo.color;


    }

    public void resetDelay()
    {
        StartCoroutine(this.battleDelay());
    }

    private IEnumerator battleDelay()
    {
        this.getInfo().canLocalBattle = false;
        float wait = UnityEngine.Random.Range(5.0f, 10);
        yield return new WaitForSeconds(wait);
        this.getInfo().canLocalBattle = true;
    }

    public GameState.Player getInfo()
    {
        return this.playerInfo;
    }

    virtual public void setInfo(GameState.Player info)
    {
        this.integrateInfo(info, true);
    }

    protected virtual void Update()
    {
        //this.screenPos = Camera.main.WorldToScreenPoint(transform.position);
        

    }

    private void OnGUI()
    {
        //GUI.Label(new Rect(screenPos.x, screenPos.y, 100, 50), this.playerInfo.name);
    }

    public Vector3 displacement;

    private void FixedUpdate()
    {

        // Integrate all physics
        this.playerInfo.integratePhysics(Time.deltaTime);

        if (this._anim == null ) _anim = GetComponentInChildren<Animator>();

        if (!this.playerInfo.inBattle)
        {
            this.moveTarget.position = this.playerInfo.position;

            if (this._anim != null) // can happen during instantiation of object (via setInfo)
            {
                displacement = this.moveTarget.position - this.transform.position;
                if (Mathf.Abs(displacement.x) > Mathf.Abs(displacement.y))
                    displacement.y = 0;
                else
                    displacement.x = 0;

                _anim.SetFloat("velX", displacement.x);
                _anim.SetFloat("velY", displacement.y);
                _anim.SetBool("walking", Mathf.Abs(displacement.x) > 0.001f || Mathf.Abs(displacement.y) > 0.001f);

                //Debug.Log (displacement);
            }

            this.transform.position = Vector3.Lerp(this.transform.position, this.moveTarget.position, 0.1f);
        }
        else
        {
            _anim.SetBool("walking", false);
        }
    }

    public virtual void OnLocalInput(Vector3 position, Vector3 deltaMove)
    {
    }

}
