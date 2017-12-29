// Author: Jake Ruth
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// \addtogroup client
/// @{

public enum MenuState
{
    MAIN_MENU,
    ATTACK_MENU,
    ITEM_MENU,
    SWITCH_MENU,
    FORFEIT_MENU,
    WAITING
}

public class BattleUIController : MonoBehaviour
{
    private MenuState _menuState;

    public MenuState menuState
    {
        get { return _menuState; }
        set
        {
            _menuState = value;

            mainMenuGameObject.SetActive(_menuState == MenuState.MAIN_MENU);
            attackMenuGameObject.SetActive(_menuState == MenuState.ATTACK_MENU);
            itemsMenuGameObject.SetActive(_menuState == MenuState.ITEM_MENU);
            switchMenuGameObject.SetActive(_menuState == MenuState.SWITCH_MENU);
            forfeitMenuGameObject.SetActive(_menuState == MenuState.FORFEIT_MENU);
            waitingGameObject.SetActive(_menuState == MenuState.WAITING);

            switch (_menuState)
            {
                case MenuState.MAIN_MENU:
                    break;
                case MenuState.ATTACK_MENU:

                    MonsterDataObject localCretin = battleHandler.participant1.currentCretin;
                    if (battleHandler.participant2.isPlayer() && battleHandler.participant2.playerController.isLocal)
                        localCretin = battleHandler.participant2.currentCretin;

                    
                    List<AttackObject> availableAttacks = localCretin.GetAvailableAttacks;
                    for (int i = 0; i < attackButtons.Length; i++)
                    {
                        bool hasMatchingAttack = i < availableAttacks.Count;
                        attackButtons[i].interactable = hasMatchingAttack;
                        if (hasMatchingAttack)
                        {
                            Text attackTextBox = attackButtons[i].GetComponentInChildren<Text>();
                            attackTextBox.text = availableAttacks[i].attackName;
                        }
                    }
                    break;
                case MenuState.ITEM_MENU:
                    break;
                case MenuState.SWITCH_MENU:
                    //Debug.Assert(battleHandler.participant1.isPlayer());

                    IList<MonsterDataObject> availableMonsters = battleHandler.participant1.playerController.monsters;
                    if (battleHandler.participant2.isPlayer() && battleHandler.participant2.playerController.isLocal)
                        availableMonsters = battleHandler.participant2.playerController.monsters;

                    for (int i = 0; i < cretinButtons.Length; i++)
                    {
                        bool hasMatchingMonster = i < availableMonsters.Count;
                        cretinButtons[i].interactable =
                            hasMatchingMonster || battleHandler.participant1.currentCretinIndex == i;

                        cretinButtons[i].GetComponentInChildren<Text>().text =
                            hasMatchingMonster ? availableMonsters[i].GetMonsterName : "N/A";
                    }
                    break;
                case MenuState.FORFEIT_MENU:
                    break;
                case MenuState.WAITING:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    [Header("Transform Dependencies")]
    public BattleHandler battleHandler;

    [Header("Base menu holders")]
    public GameObject mainMenuGameObject;
    public GameObject attackMenuGameObject;
    public GameObject itemsMenuGameObject;
    public GameObject switchMenuGameObject;
    public GameObject forfeitMenuGameObject;
    public GameObject waitingGameObject;

    [Header("Attack Variables")]
    public Button[] attackButtons;

    [Header("Switch Variables")]
    public Button[] cretinButtons;

    //[Header("Items Variables")]
    // coming soon. HA....

    //[Header("Forfeit Variables")]

    [Header("Waiting Variables")]
    public Text waitingText;

    [Header("Cretin Display Variables")]
    public Text localCretinNameText;
    public Text localCretinHealthText;
    public Image localHPBar;
    public Image localCretinImage;

    public Text otherCretinNameText;
    public Text otherCretinHealthText;
    public Image otherHPBar;
    public Image otherCretinImage;

    void Start()
    {
        menuState = MenuState.MAIN_MENU;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace))
        {
            BackButtonClicked();
        }
    }

    // Must be int, not uint, so unity's inspector can use it during OnClick
    public void ButtonClicked(int buttonIndex)
    {
        Debug.Log("Button clicked: " + buttonIndex);
        switch (menuState)
        {
            case MenuState.MAIN_MENU:

                switch (buttonIndex)
                {
                    case 1:
                        menuState = MenuState.ATTACK_MENU;
                        break;
                    case 2:
                        menuState = MenuState.SWITCH_MENU;
                        break;
                    case 3:
                        menuState = MenuState.ITEM_MENU;
                        break;
                    case 4:
                        menuState = MenuState.FORFEIT_MENU;
                        break;
                    default:
                        break;
                }

                break;
            case MenuState.ATTACK_MENU:

                battleHandler.SendBattleOption(true, GameState.Player.EnumBattleSelection.ATTACK, (uint)buttonIndex);
                menuState = MenuState.WAITING;

                break;
            case MenuState.ITEM_MENU:

                // Eventually do something... maybe.
                menuState = MenuState.MAIN_MENU;

                break;
            case MenuState.SWITCH_MENU:

                battleHandler.SendBattleOption(true, GameState.Player.EnumBattleSelection.SWAP, (uint)buttonIndex);
                menuState = MenuState.WAITING;

                break;
            case MenuState.FORFEIT_MENU:
                switch (buttonIndex)
                {
                    case 1:
                        battleHandler.SendBattleOption(true, GameState.Player.EnumBattleSelection.FLEE, 0);
                        menuState = MenuState.WAITING;
                        break;
                    default:
                        BackButtonClicked();
                        break;
                }
                break;
            case MenuState.WAITING:
                SetFlavorText("Waiting for opponnet's response");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void BackButtonClicked()
    {
        if (menuState != MenuState.WAITING && menuState != MenuState.MAIN_MENU)
            menuState = MenuState.MAIN_MENU;
    }

    public void SetFlavorText(string text)
    {
        waitingText.text = text;
    }

    public void UpdateCretinDisplayData(BattleParticipant local, BattleParticipant other)
    {
        localCretinNameText.text = local.currentCretin.GetMonsterName;
        localCretinHealthText.text =
            string.Format("{0} / {1}", local.CurrentHP, local.maxHP);
        localHPBar.transform.localScale = new Vector3(local.CurrentHP / (float)local.maxHP, 1, 1);
        localCretinImage.sprite = local.currentCretin.monsterStat.monsterPicture;

        otherCretinNameText.text = other.currentCretin.GetMonsterName;
        otherCretinHealthText.text =
            string.Format("{0} / {1}", other.CurrentHP, other.maxHP);
        otherHPBar.transform.localScale = new Vector3(other.CurrentHP / (float)other.maxHP, 1, 1);
        otherCretinImage.sprite = other.currentCretin.monsterStat.monsterPicture;
    }
}
