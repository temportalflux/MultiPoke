using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class ConnectMenu : MonoBehaviour {

    [System.Serializable]
    public class EventConnect : UnityEvent<string, int, PlayerDescriptor[]> { }
    public EventConnect onConnect;

    public struct PlayerDescriptor
    {
        public string name;
        public Color color;
    }

    //[System.Diagnostics.CodeAnalysis.SuppressMessage(null, "CS0414")]
    public string txtAddress = "216.93.149.213";
    private string txtPort = "425", txtPlayerCount = "1";
    //[System.Diagnostics.CodeAnalysis.SuppressMessage(null, "CS0414")]
    private string errorPort = null, errorConnect = null, errorPlayerCount = null;
    private Vector2 scrollPosition = Vector2.zero;

    //private int playerCount;
    private PlayerDescriptor[] players = new PlayerDescriptor[1];

    private bool connecting = false, showGui = true;

    private readonly string[] _names =
    {
        "Ash", "Gary", "Brock", "New Player",
        "Red", "Blue", "Green", "Oak", "Misty",
        "Jake", "Chris", "Dustin", "Jeff", "Blake", "Tim",
        "Molly", "Patrick", "NPH", "John", "DannyB27",
        "April 5th", "Santa", "JohnWBooth", "Ted Bundy",
        "jUSTIN", "Dusty", "Tohper", "Jake Rum", "Jessie", "James",
        "Jesus"
    };

    void OnEnable()
    {
        this.players[0].name = _names[UnityEngine.Random.Range(0, _names.Length)];

        Color c = Colors.ALL_COLORS[UnityEngine.Random.Range(0, Colors.ALL_COLORS.Length)];

        //this.players[0].color.r = UnityEngine.Random.Range(0, 1.0f);
        //this.players[0].color.g = UnityEngine.Random.Range(0, 1.0f);
        //this.players[0].color.b = UnityEngine.Random.Range(0, 1.0f);

        this.players[0].color = c;


    }

    void OnGUI()
    {
        if (!this.showGui) return;

        int width = 200;
        GUI.Box(new Rect(0, 0, width, Screen.height), "");

        GUILayout.BeginVertical(GUILayout.Width(width));
        {
            this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition);

            GUILayout.Label("Address");

            this.txtAddress = GUILayout.TextField(this.txtAddress);

            GUILayout.Label("Port");

            string port = GUILayout.TextField(this.txtPort);
            if (port != null && port.Length > 0)
            {
                try
                {
                    int.Parse(port);
                    this.txtPort = port;
                    this.errorPort = null;
                }
                catch (FormatException e)
                {
                    this.errorPort = "Only numbers allowed " + e.ToString();
                }
                catch (Exception e)
                {
                    this.errorPort = "parse error";
                    Debug.LogError(e);
                }
            }

            if (this.errorPort != null)
            {
                GUILayout.Label(this.errorPort);
            }

            // Player Listings
            /*
            GUILayout.Label("# Players");
            this.txtPlayerCount = GUILayout.TextField(this.txtPlayerCount);
            if (this.txtPlayerCount != null)
            {
                try
                {
                    int pc = 0;
                    if (this.txtPlayerCount.Length > 0)
                    {
                        pc = int.Parse(this.txtPlayerCount);
                    }

                    if (pc <= 2)
                    {
                        if (pc != this.playerCount)
                        {
                            Array.Resize(ref this.players, pc);
                        }
                        if (this.txtPlayerCount.Length > 0)
                            this.playerCount = pc;
                        this.errorPlayerCount = null;
                    }
                    else
                    {
                        this.errorPlayerCount = "No more than 2 players";
                    }

                }
                catch (FormatException e)
                {
                    this.errorPlayerCount = "Only numbers allowed " + e.ToString();
                }
                catch (Exception e)
                {
                    this.errorPlayerCount = "parse error";
                    Debug.LogError(e);
                }
            }
            if (this.errorPlayerCount != null)
            {
                GUILayout.Label(this.errorPlayerCount);
            }
            //*/
            if (this.players.Length > 0)
            {
                for (int localID = 0; localID < this.players.Length; localID++)
                {
                    // Put down listings for the player
                    GUILayout.Label("Player " + (localID + 1));

                    if (this.players[localID].name == null)
                        this.players[localID].name = "Player-" + (localID + 1);
                    //if (this.players[localID].color == null)
                    //    this.players[localID].color = Color.red;

                    GUILayout.Label("Name");
                    this.players[localID].name = GUILayout.TextField(this.players[localID].name, GameState.Player.SIZE_MAX_NAME);

                    GUILayout.Label("Color");
                    // Color picker
                    GUILayout.BeginVertical(GUILayout.Width(width - 10));
                    {
                        GUILayout.Label("Red " + (int)(this.players[localID].color.r * 255));
                        this.players[localID].color.r = GUILayout.HorizontalSlider(
                            this.players[localID].color.r * 255, 0, 255) / 255F;
                        GUILayout.Label("Green " + (int)(this.players[localID].color.g * 255));
                        this.players[localID].color.g = GUILayout.HorizontalSlider(
                            this.players[localID].color.g * 255, 0, 255) / 255F;
                        GUILayout.Label("Blue " + (int)(this.players[localID].color.b * 255));
                        this.players[localID].color.b = GUILayout.HorizontalSlider(
                            this.players[localID].color.b * 255, 0, 255) / 255F;
                    }
                    GUILayout.EndVertical();

                }
            }

            GUILayout.EndScrollView();

            if (!this.connecting)
            {
                if (this.players.Length <= 0)
                {
                    GUILayout.Label("Must have at least 1 player");
                }
                else
                {
                    if (GUILayout.Button("Connect"))
                    {
                        this.errorConnect = null;
                        this.connecting = true;
                        this.onConnect.Invoke(this.txtAddress, int.Parse(this.txtPort), this.players);
                    }
                }
            }

            if (this.errorConnect != null)
            {
                GUILayout.Label(this.errorConnect);
            }

        }
        GUILayout.EndVertical();
    }

    public void onConnectionHandled(bool success)
    {
        this.connecting = false;
        this.showGui = !success;
    }

    public void setConnectionError(string error)
    {
        this.errorConnect = error;
    }

    /*
     EventSystem.current.SetSelectedGameObject(myInputField.gameObject, null);
 myInputField.OnPointerClick(new PointerEventData(EventSystem.current));
     */

}
