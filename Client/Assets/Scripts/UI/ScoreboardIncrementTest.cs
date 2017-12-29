using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScoreboardIncrementTest : MonoBehaviour
{
    private int selected;


    void Start()
    {

        this.selected = 0;

    }

    public void down(InputDevice device, InputResponse.UpdateEvent type, MappedButton action)
    {

        GameState.Player info = GameObject.Find("Player-1").GetComponent<PlayerLocal>().getInfo();

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
