/*
Names and ID: Christopher Brennan: 1028443, Dustin Yost: 0984932, Jacob Ruth: 0890406
Course Info: EGP-405-01 
Project Name: Project 3: Synchronized Networking
Due: 11/22/17
Certificate of Authenticity (standard practice): “We certify that this work is entirely our own.  
The assessor of this project may reproduce this project and provide copies to other academic staff, 
and/or communicate a copy of this project to a plagiarism-checking service, which may retain a copy of the project on its database.”
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ID = System.UInt32;
using System.Linq;
using System;

/// \addtogroup client
/// @{

/// <summary>
/// Holds all persistent data from network intergration for local usage
/// </summary>
/// \author Dustin Yost
public class GameState : ScriptableObject, ISerializing
{

    /// <summary>
    /// A state class for all Player objects
    /// </summary>
    [System.Serializable]
    public class Player : IList<MonsterDataObject>
    {

        /// <summary>
        /// The serialized size of the Player object
        /// </summary>
        public static int SIZE =
            sizeof(ID) // clientID
            + sizeof(ID) // playerID
            + sizeof(ID) // localID
            + sizeof(int) + (SIZE_MAX_NAME * sizeof(char))// name
            + (sizeof(float) * 3) // color
            + (sizeof(float) * 3) // position
            + (sizeof(float) * 3) // velocity
            + (sizeof(float) * 3) // acceleration
            + sizeof(bool) // inBattle
            + sizeof(uint) // wins
            + sizeof(uint) // rank
            ;

        /// <summary>
        /// The maximum length of <see cref="GameState.Player.name"/>
        /// </summary>
        public static int SIZE_MAX_NAME = 10;

        /// <summary>
        /// A unique identifier of the client which controls this player.
        /// Unqiue across all peers in the network. 
        /// </summary>
        [BitSerialize(0)]
        public ID clientID;

        /// <summary>
        /// If the player id is from a local player (controlled on this game) or networked (controlled by a peer)
        /// </summary>
        public bool isLocal {
            get
            {
                return this.clientID == GameManager.INSTANCE.state.getClientID();
            }
        }

        // Indentification

        /// <summary>
        /// A unique identifier of the player object.
        /// Unique across all peers in the network.
        /// </summary>
        [Tooltip("The unique identifier for this specific player")]
        [BitSerialize(1)]
        public ID playerID;

        /// <summary>
        /// A unique identifier of the player object in a local context.
        /// Unique across all players with the same <see cref="clientID"/>.
        /// </summary>
        [Tooltip("The unique identifier for this player on the local machine")]
        [BitSerialize(2)]
        public ID localID;

        /// <summary>
        /// The name identifier of the player. Not unique. Enforced max of <see cref="SIZE_MAX_NAME"/>.
        /// </summary>
        [Tooltip("The name of this player with 10 characters max")]
        [BitSerialize(3)]
        public string name;

        /// <summary>
        /// The color-overlay.
        /// </summary>
        [Tooltip("The color of the character for the player")]
        [BitSerialize(4)]
        public Color color;

        // Physics

        /// <summary>
        /// The position of the player in the open-world.
        /// </summary>
        [Tooltip("The current position of the player")]
        [BitSerialize(5)]
        public Vector3 position;

        /// <summary>
        /// The velocity of the player in the open-world.
        /// </summary>
        [Tooltip("The velocity of the player")]
        [BitSerialize(6)]
        public Vector3 velocity;

        /// <summary>
        /// The accelleration of the player in the open-world.
        /// </summary>
        [Tooltip("The acceleration of the player")]
        [BitSerialize(7)]
        public Vector3 accelleration;

        // Battle

        /// <summary>
        /// If the player is presently in a battle with a player or AI.
        /// </summary>
        [Tooltip("If the player is in battle")]
        [BitSerialize(8)]
        public bool inBattle;

        /// <summary>
        /// The player <see cref="ID"/> of the opponent. Could be irrelevant if player is in local battle with AI.
        /// </summary>
        public ID playerIDOpponent;

        /// <summary>
        /// Options of types of battle moves.
        /// </summary>
        public enum EnumBattleSelection
        {
            ATTACK, SWAP, FLEE,
        }

        /// <summary>
        /// The type of action the player makes in battle.
        /// </summary>
        public EnumBattleSelection battleSelection;

        /// <summary>
        /// The descriptor for the type of action (narrows down choice to options like
        /// which attack a cretin makes, or what cretin is being swapped to).
        /// </summary>
        public int battleChoice;

        // Scoreboard

        /// <summary>
        /// The total number of wins since entering the server.
        /// </summary>
        [Tooltip("Total number of wins since entering the server")]
        [BitSerialize(9)]
        public uint wins;

        /// <summary>
        /// The rank of the player in the scoreboard.
        /// The higher <see cref="wins"/> is compared to other players, the 
        /// </summary>
        [Tooltip("Scoreboard rank of the player")]
        [BitSerialize(10)]
        public int rank;

        /// <summary>
        /// A list of all the cretins the player has.
        /// </summary>
        [BitSerialize(11)]
        public List<uint> monsterIDs;

        // Not-Serialized

        // Game Objects

        /// <summary>
        /// The reference to the GameObject in the local unity instance
        /// </summary>
        public PlayerReference objectReference;

        /// <summary>
        /// Reference to the render output for this <see cref="objectReference"/>
        /// </summary>
        public RenderTexture cameraTexture;

        public int oldRank;

        public bool canLocalBattle;

        public Player()
        {
            this.canLocalBattle = true;
            this.monsterIDs = new List<uint>();
        }

        // ~~~~~ Data copy

        /// <summary>
        /// Copy data from a deserialized player state into this player state
        /// </summary>
        /// <param name="info">The newly deserialized player state.</param>
        /// <param name="deltaTime">The time to integrate physics over.</param>
        public void copyFromGameState(Player info, float deltaTime)
        {
            this.clientID = info.clientID;
            this.playerID = info.playerID;
            this.name = info.name;
            this.color = info.color;
            this.position = info.position;
            this.velocity = info.velocity;
            this.accelleration = info.accelleration;
            this.inBattle = info.inBattle;
            this.wins = info.wins;
            this.oldRank = this.rank;
            this.rank = info.rank;
            this.monsterIDs = info.monsterIDs;

            if (this.objectReference != null)
            {
                this.objectReference.integrateInfo(this);
            }

            //this.integratePhysics(deltaTime);
        }

        /// <summary>
        /// Integrate physics in this object (accelleration->velocity, velocity->position).
        /// </summary>
        /// <param name="deltaTime">The time to integrate over.</param>
        public void integratePhysics(float deltaTime)
        {
            this.velocity += this.accelleration * deltaTime;
            this.position += this.velocity * deltaTime;
        }

        // IList

        public IList<MonsterDataObject> monsters
        {
            get
            {
                return this;
            }
        }
        
        public MonsterDataObject this[int index] {
            get
            {
                return GameManager.INSTANCE.state.allMonsters[this.monsterIDs[index]];
            }
            set
            {
                this.monsterIDs[index] = value.monsterStat.id;
            }
        }
        public int IndexOf(MonsterDataObject item) { return this.monsterIDs.IndexOf(item.monsterStat.id); }
        public void Insert(int index, MonsterDataObject item) { this.monsterIDs.Insert(index, item.monsterStat.id); }
        public void RemoveAt(int index) { this.monsterIDs.RemoveAt(index); }
        public void Add(MonsterDataObject item) {
            this.monsterIDs.Add(item.monsterStat.id);
        }
        int ICollection<MonsterDataObject>.Count
        {
            get
            {
                return this.monsterIDs.Count;
            }
        }
        bool ICollection<MonsterDataObject>.IsReadOnly
        {
            get
            {
                return false;
            }
        }
        public void Clear() { this.monsterIDs.Clear(); }
        public bool Contains(MonsterDataObject item) { return this.monsterIDs.Contains(item.monsterStat.id); }
        public void CopyTo(MonsterDataObject[] array, int arrayIndex)
        {
            this.monsterIDs.CopyTo(System.Array.ConvertAll(array, x => x.monsterStat.id), arrayIndex);
        }
        public bool Remove(MonsterDataObject item) { return this.monsterIDs.Remove(item.monsterStat.id); }
        public IEnumerator<MonsterDataObject> GetEnumerator()
        {
            return GameManager.INSTANCE.state.allMonsters.TakeWhile((MonsterDataObject item) => { return this.Contains(item); }).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GameManager.INSTANCE.state.allMonsters.GetEnumerator();
        }

    }

    /// <summary>
    /// Not Serialized.
    /// Used to store the requests for players on world-enter
    /// </summary>
    public ConnectMenu.PlayerDescriptor[] playerRequest = null;

    [HideInInspector]
    public bool editorFoldoutPlayers;
    public Dictionary<ID, bool> editorFoldouts;

    [ToDo("Serialize")]
    public bool isLocalGame;

    /// <summary>
    /// A unique identifier (across all peers in the network) of the client which controls this player
    /// </summary>
    private uint _clientID;
    /// <summary>
    /// A unique identifier (across all peers in the network) of the client which controls this player
    /// </summary>
    [BitSerialize(0)]
    public int clientID;

    public uint getClientID() { return this._clientID; }

    public MonsterDataObject[] allMonsters;
    public MonsterDataObject[] starters;

    public float deltaTime;
    public Dictionary<ID, Player> players;

    private List<Player> playersToAdd;
    private List<Player> playersToRemove;
    private Dictionary<uint, GameState.Player> playersLocal;
    private Dictionary<uint, GameState.Player> playersConnected;
    public Dictionary<uint, Player> localPlayers
    {
        get
        {
            return playersLocal;
        }
    }
    public Dictionary<uint, Player> connectedPlayers
    {
        get
        {
            return playersConnected;
        }
    }

    private void OnEnable()
    {
        this.editorFoldouts = new Dictionary<ID, bool>();
        this.players = new Dictionary<ID, Player>();

        this.playersToAdd = new List<Player>();
        this.playersToRemove = new List<Player>();
        this.playersLocal = new Dictionary<uint, GameState.Player>();
        this.playersConnected = new Dictionary<uint, GameState.Player>();
    }

    public bool HasPlayer(ref Player info)
    {
        if (this.players.ContainsKey(info.playerID))
        {
            info = this.players[info.playerID];
            return true;
        }
        return false;
    }

    /// <summary>
    /// Add a player to the current game object scene via its info
    /// </summary>
    /// <param name="info"></param>
    [ToDo("FindGameObjectWithTag('AllPlayers') should be cached")]
    public void AddPlayer(Player info)
    {
        // Create the player reference object
        GameManager gm = GameManager.INSTANCE;
        GameObject playerObject = GameManager.Instantiate(
            info.isLocal ? gm.playerPrefab : gm.playerNetworkPrefab,
            GameObject.FindGameObjectWithTag("AllPlayers").transform
        );
        info.objectReference = playerObject.GetComponent<PlayerReference>();

        // check if second player, and tweak accordingly
        if (info.isLocal && info.localID > 0)
        {
            Transform moveTarget = info.objectReference.moveTarget;

            Destroy(playerObject.GetComponent<PlayerReference>());
            Destroy(playerObject.GetComponent<InputResponse>());
            info.objectReference = playerObject.AddComponent<PlayerLocalMultiplayer>();
            info.objectReference.moveTarget = moveTarget;
            info.objectReference.sprite = playerObject.GetComponentInChildren<Animator>().transform;
            foreach (SpriteRenderer r in playerObject.GetComponentsInChildren<SpriteRenderer>())
                if (r.gameObject.name == "Overlay")
                    info.objectReference.overlay = r;
        }

        if (info.isLocal)
        {
            // Add render texture for player camera
            //Camera mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            info.cameraTexture = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.ARGB32);
            info.cameraTexture.antiAliasing = 2;
            info.cameraTexture.Create();
            playerObject.GetComponentInChildren<Camera>(true).targetTexture = info.cameraTexture;
            GameManager.INSTANCE.StartCoroutine(this.AddPlayerToCamera(info));

            Color playerColor = info.color;

            // Add a monster based on which color chanel has the greatest value
            if (playerColor.r > info.color.g && playerColor.r > playerColor.b) // Add charmander
            {
                info.monsters.Add(this.starters[0]);
            }
            else if (playerColor.g > info.color.r && playerColor.g > playerColor.b) // Add bulbasaur
            {
                info.monsters.Add(this.starters[1]);
            }
            else if (playerColor.b > info.color.r && playerColor.b > playerColor.g) // Add squartile
            {
                info.monsters.Add(this.starters[2]);
            }
            else // No one color chanel is greater, Select a random one.
            {
                info.monsters.Add(this.starters[UnityEngine.Random.Range(0, this.starters.Length)]);
            }
            EventPlayerAddMonster.Dispatch(info.playerID, info.monsterIDs[0]); // set via info.monsters
        }

        // Set the info for the player script
        info.objectReference.setInfo(info);

        info.objectReference.gameObject.name = info.name;

        // Add player to total list
        this.players.Add(info.playerID, info);
        // Add player to the appropriate sublist
        if (info.isLocal) this.AddPlayerLocal(info); else this.AddPlayerConnected(info);
    }

    private IEnumerator AddPlayerToCamera(Player info)
    {
        while (GameManager.INSTANCE.mainCamera == null) yield return null;
        GameManager.INSTANCE.mainCamera.SetTexture((int)info.localID, info.cameraTexture);
    }

    public void RemovePlayer(Player info)
    {
        if (this.HasPlayer(ref info))
        {
            //GameManager.INSTANCE.getScoreBoard().removePlayerOnLeave(info.rank);

            this.players.Remove(info.playerID);

            if (this.playersLocal.ContainsKey(info.playerID))
                this.playersLocal.Remove(info.playerID);
            if (this.playersConnected.ContainsKey(info.playerID))
                this.playersConnected.Remove(info.playerID);

            if (info.objectReference != null)
                Destroy(info.objectReference.gameObject);
        }
    }

    public void AddPlayerLocal(Player info)
    {
        this.playersLocal.Add(info.playerID, info);
    }

    public void AddPlayerConnected(Player info)
    {
        this.playersConnected.Add(info.playerID, info);
    }

    public void SpawnLocalMultiplayer()
    {
        //NetInterface.INSTANCE.Dispatch(new EventRequestPlayer(this.clientID, (uint)this.playersLocal.Count));
    }

    public void SpawnLocalPlayer()
    {
        Player player = new Player();
        player.clientID = 0;
        player.localID = (uint)this.players.Count;
        player.playerID = player.localID;
        player.position = Vector3.zero;
        player.name = "Player " + (player.localID + 1);
        switch (player.localID)
        {
            case 0: player.color = Colors.PuceRed; break;
            case 1: player.color = Colors.BlizzardBlue; break;
            case 2: player.color = Colors.EmeraldGreen; break;
            default: player.color = Colors.YellowOrange; break;
        }
        this.AddPlayer(player);
    }

    public void SpawnPlayer()
    {
        if (this.isLocalGame) this.SpawnLocalPlayer();
        else this.SpawnLocalMultiplayer();
    }

    // ~~~~~ ISerializing

    public int GetSize()
    {
        // clientID + # of players + amount of space required for players
        return //sizeof(uint) +
            sizeof(int) + (this.players.Count * Player.SIZE);
    }

    /// <summary>
    /// Serializes data from this event into a byte array.
    /// Unused for GameState. GameStates are ONLY serialized via server.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="lastIndex">The last index.</param>
    /// \author Dustin Yost
    public void Serialize(ref byte[] data, ref int lastIndex)
    {
    }

    /// <summary>
    /// Deserializes data from a byte array into this event's data
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="lastIndex">The last index.</param>
    /// \author Dustin Yost
    public void Deserialize(byte[] data, ref int lastIndex)
    {

        /*
        // Deserialize clientID
        int clientID = System.BitConverter.ToInt32(data, lastIndex);
        lastIndex += sizeof(ID);
        //*/
        // Check for valid clientID (if < 0, then the gamestate was broadcasted)
        if (this.clientID >= 0)
        {
            this._clientID = (uint)clientID;
        }

        // Deserialize player count
        int playerCount = System.BitConverter.ToInt32(data, lastIndex);
        lastIndex += sizeof(System.Int32);

        List<ID> playersFromData = new List<ID>();
        // Read all players, keeping track of the IDs read
        for (int i = 0; i < playerCount; i++)
        {
            // Deserialize the player data
            Player player = (Player)BitSerializeAttribute.Deserialize(new Player(), data, ref lastIndex);
            //player.objectReference = null;
            //player.Deserialize(data, ref lastIndex);

            playersFromData.Add(player.playerID);

            // Check if the ID is already in the map
            if (this.players.ContainsKey(player.playerID))
            {
                this.players[player.playerID].copyFromGameState(player, this.deltaTime);
            }
            // playerID not in map, add
            else
            {
                this.playersToAdd.Add(player);
            }

        }

        // some players have been removed from gamestate data
        if (playersFromData.Count < this.players.Count)
        {
            // find all players no longer in the gamestate data
            foreach (ID playerID in this.players.Keys)
            {
                if (!playersFromData.Contains(playerID))
                {
                    // mark those players for removal from game
                    this.playersToRemove.Add(this.players[playerID]);
                }
            }
        }

    }

    // ~~~~~ Update

    public void FixedUpdate()
    {
        // Remove all players that are set to be removed
        foreach (Player player in this.playersToRemove)
        {
            this.RemovePlayer(player);
        }
        this.playersToRemove.Clear();
        // Add all players that are set to be added
        while (this.playersToAdd.Count > 0)
        {
            Player player = this.playersToAdd[0];
            this.playersToAdd.RemoveAt(0);
            // If updates have been received before the list got updated, there could be overlap
            if (this.players.ContainsKey(player.playerID))
            {
                this.players[player.playerID].copyFromGameState(player, 0);
            }
            else
            {
                this.AddPlayer(player);
            }
        }
    }

}

/// @}
