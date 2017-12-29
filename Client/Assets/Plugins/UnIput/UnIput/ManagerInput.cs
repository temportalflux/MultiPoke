using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GamepadInput))]
public class ManagerInput : MonoBehaviour {

    private static ManagerInput _instance = null;
    public static ManagerInput INSTANCE
    {
        get
        {
            return _instance;
        }
    }

    private static void loadSingleton(ManagerInput inst)
    {

        if (_instance != null)
        {
            Destroy(_instance);
            _instance = null;
        }

        _instance = inst;
        DontDestroyOnLoad(_instance);

    }
    
    void Start () {
        ManagerInput.loadSingleton(this);
	}

    private bool inUse = false;

    public bool isInUse()
    {
        return inUse;
    }

    public void Lock() { inUse = true; }
    public void Unlock() { inUse = false; }

    public void RemoveAllGamepads()
    {
#if UNITY_EDITOR || !UNITY_STANDALONE_WIN
        UnGamepadManager input = this.GetComponent<GamepadInput>().getEditorManager();
        if (input != null)
        {
            foreach (GamepadDevice device in input.gamepads)
            {
                input.RemoveIfDisconnected(device);
            }
            input.RefreshDevices();
        }
#endif
    }

}
