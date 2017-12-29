using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PauseMenu : MonoBehaviour {

    public PlayerReference trainer;

    private Text msg;
    private int display;

    // Use this for initialization
    void Start ()
    {

        this.display = 1;

        foreach (MonsterDataObject data in this.trainer.getInfo().monsters)
        {
            this.msg = GameObject.Find("Cretin Name (" + display + ")").GetComponent<Text>();
            this.msg.text = data.GetMonsterName;

            this.msg = GameObject.Find("Cretin Attack (" + display + ")").GetComponent<Text>();
            this.msg.text = "Atk: " + data.GetAttack.ToString();

            this.msg = GameObject.Find("Cretin Defense (" + display + ")").GetComponent<Text>();
            this.msg.text = "Def: " + data.GetDefense.ToString();

            this.msg = GameObject.Find("Cretin Special Attack (" + display + ")").GetComponent<Text>();
            this.msg.text = "Spc Atk: " + data.GetSpecialAttack.ToString();

            this.msg = GameObject.Find("Cretin Special Defense (" + display + ")").GetComponent<Text>();
            this.msg.text = "Spc Def: " + data.GetSpecialDefense.ToString();

            this.msg = GameObject.Find("Cretin Speed (" + display + ")").GetComponent<Text>();
            this.msg.text = "Spd: " + data.GetSpeed.ToString();

            this.msg = GameObject.Find("Cretin HP (" + display + ")").GetComponent<Text>();
            //this.msg.text = "HP: " + data.CurrentHP.ToString() + "/" + data.GetMaxHp.ToString();

            display += 1;
        }

    }

    public void HealCretin()
    {
        this.display = 1;
        foreach (MonsterDataObject data in this.trainer.getInfo().monsters)
        {
            //data.Heal();
            this.msg = GameObject.Find("Cretin HP (" + display + ")").GetComponent<Text>();
            //this.msg.text = "HP: " + data.CurrentHP.ToString() + "/" + data.GetMaxHp.ToString();

            this.display += 1;
        }
    }

}
