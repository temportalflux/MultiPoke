using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/**
 * Searches for updates from input controllers and sends to listeners
 */
public class InputResponse : MonoBehaviour {

    // The different types of updates available
    [System.Serializable]
    public enum UpdateEvent
    {
        // Never triggered, do not use unless indicating null
        NONE,
        // When a button is initially pressed
        DOWN,
        // When a button is initially released
        UP,
        // When a button or axis/trigger is being held
        TICK,
    }

    // The listener class for handling all button effects
    [System.Serializable]
    public class Listener<T>
    {
        [Tooltip("The type of update desired (not None)")]
        public UpdateEvent type;

        [Tooltip("The button or axis being pressed")]
        public T key;

        [Tooltip("Indicates if certain input types should be filtered out")]
        public bool useKeyboard = true, useMouse = false, useGamepad = true;

    }

    // The listener class for buttons (required for UnityEvents)
    [System.Serializable]
    public class ListenerButton : Listener<MappedButton>
    {

        [System.Serializable]
        public class InputEvent : UnityEvent<InputDevice, UpdateEvent, MappedButton> { }

        [Tooltip("The action to perform when a button is changed")]
        public InputEvent action;

    }

    // The listener class for axes (required for UnityEvents)
    [System.Serializable]
    public class ListenerAxis : Listener<MappedAxis>
    {

        [System.Serializable]
        public class InputEvent : UnityEvent<InputDevice, UpdateEvent, MappedAxis, float> { }

        [System.Serializable]
        public class InputEventButton : UnityEvent<InputDevice, UpdateEvent, MappedAxis, AxisDirection> { }

        [Tooltip("The action to perform when a axis is changed")]
        public InputEvent action;

        [Tooltip("The action to perform when a axis is changed")]
        public InputEventButton actionButton;

    }

    [Range(1, 4)]
    [Tooltip("The controller number")]
    public int inputId = 1;

    [Tooltip("The list of listeners for buttons")]
    public ListenerButton[] listenerButtons;
    [Tooltip("The list of listeners for axes")]
    public ListenerAxis[] listenerAxes;


    [System.Serializable]
    public class KeyboardEvent : UnityEvent<InputDevice, char> { }
    public KeyboardEvent onKeyInput;

    // The dictionary mapping of listeners for buttons from Update type -> Button -> Listener action
    private Dictionary<UpdateEvent, Dictionary<MappedButton, List<ListenerButton>>> dictListenerButtons;
    // The dictionary mapping of listeners for axes from Update type -> Button -> Listener action
    private Dictionary<UpdateEvent, Dictionary<MappedAxis, List<ListenerAxis>>> dictListenerAxes;

    private void Start()
    {
        // Create the dictionaries
        this.dictListenerButtons = new Dictionary<UpdateEvent, Dictionary<MappedButton, List<ListenerButton>>>();
        this.dictListenerAxes = new Dictionary<UpdateEvent, Dictionary<MappedAxis, List<ListenerAxis>>>();

        // Populate the dictionaries from the arrays
        this.forSet(this.listenerButtons,
            (ListenerButton listener) =>
            {
                // ensure there is a mapping for the update type
                if (!this.dictListenerButtons.ContainsKey(listener.type))
                    this.dictListenerButtons.Add(listener.type, new Dictionary<MappedButton, List<ListenerButton>>());
                // ensure there is a mapping for the button
                if (!this.dictListenerButtons[listener.type].ContainsKey(listener.key))
                    this.dictListenerButtons[listener.type].Add(listener.key, new List<ListenerButton>());
                // ensure there is a mapping for the action
                if (!this.dictListenerButtons[listener.type][listener.key].Contains(listener))
                    this.dictListenerButtons[listener.type][listener.key].Add(listener);
            }
        );
        this.forSet(this.listenerAxes,
            (ListenerAxis listener) =>
            {
                // ensure there is a mapping for the update type
                if (!this.dictListenerAxes.ContainsKey(listener.type))
                    this.dictListenerAxes.Add(listener.type, new Dictionary<MappedAxis, List<ListenerAxis>>());
                // ensure there is a mapping for the axis
                if (!this.dictListenerAxes[listener.type].ContainsKey(listener.key))
                    this.dictListenerAxes[listener.type].Add(listener.key, new List<ListenerAxis>());
                // ensure there is a mapping for the action
                if (!this.dictListenerAxes[listener.type][listener.key].Contains(listener))
                    this.dictListenerAxes[listener.type][listener.key].Add(listener);
            }
        );
    }

    /**
     * Iterate through any set and do some action
     */
    private void forSet<T>(IEnumerable<T> set, UnityAction<T> iteration) {
        foreach (T item in set)
        {
            iteration.Invoke(item);
        }
    }

    public void Update()
    {
        if (ManagerInput.INSTANCE == null) return;
        if (ManagerInput.INSTANCE.isInUse()) return;
        // Check for all inputs
        //MappedInput.inputDevices.ForEach(this.updateInput);
        if (MappedInput.activeDevice != null)
            this.updateInput(MappedInput.activeDevice);
    }

    // Check for updates in some input
    void updateInput(InputDevice device)
    {
        bool isMouse = device is MouseInputDevice;
        bool isKeyboard = device is KeyboardInputDevice;
        bool isGamepad = device is GamepadInputDevice;
        
        // Check keyboard
        if (isKeyboard)
        {
            foreach (char c in Input.inputString)
            {
                this.onKeyInput.Invoke(device, c);
            }
        }

        int inputID = isMouse || isKeyboard ? 1 : (device as GamepadInputDevice).gamepad.deviceId;

        if (this.inputId != inputID) return;

        float value;

        bool active;
        // check all mappings currently being tracked
        foreach (UpdateEvent eventType in this.dictListenerButtons.Keys)
        {
            
            foreach (MappedButton mapping in this.dictListenerButtons[eventType].Keys)
            {

                // Get the appropriate event
                switch (eventType)
                {
                    case UpdateEvent.DOWN:
                        active = device.GetButtonDown(mapping);
                        break;
                    case UpdateEvent.TICK:
                        active = device.GetButtonUp(mapping);
                        break;
                    case UpdateEvent.UP:
                        active = device.GetButton(mapping);
                        break;
                    default:
                        active = false;
                        break;
                }

                // Send the event to the listeners for said update type and mapping
                if (active)
                {
                    this.forSet(this.dictListenerButtons[eventType][mapping],
                        (ListenerButton listener) => {
                            if (listener.useMouse && isMouse || listener.useKeyboard && isKeyboard || listener.useGamepad && isGamepad)
                            {
                                listener.action.Invoke(device, eventType, mapping);
                            }
                        }
                    );
                }
            }
            
           
        }
        
        // check all mappings currently being tracked
        foreach (UpdateEvent eventType in this.dictListenerAxes.Keys)
        {
            foreach (MappedAxis mapping in this.dictListenerAxes[eventType].Keys)
            {

                value = device.GetAxis(mapping);

                // Send the event to the listeners for said update type and mapping
                this.forSet(this.dictListenerAxes[eventType][mapping],
                    (ListenerAxis listener) =>
                    {

                        foreach (AxisDirection direction in System.Enum.GetValues(typeof(AxisDirection)))
                        {
                            // Get the appropriate event
                            switch (eventType)
                            {
                                case UpdateEvent.DOWN:
                                    active = device.GetAxisButtonDown(mapping, direction);
                                    break;
                                case UpdateEvent.TICK:
                                    active = device.GetAxisButton(mapping, direction);
                                    break;
                                case UpdateEvent.UP:
                                    active = device.GetAxisButtonUp(mapping, direction);
                                    break;
                                default:
                                    active = false;
                                    break;
                            }
                            if (active)
                            {
                                if (listener.useMouse && isMouse || listener.useKeyboard && isKeyboard || listener.useGamepad && isGamepad)
                                {
                                    listener.actionButton.Invoke(device, eventType, mapping, direction);
                                }
                            }
                        }

                        if (listener.useMouse && isMouse || listener.useKeyboard && isKeyboard || listener.useGamepad && isGamepad)
                        {
                            listener.action.Invoke(device, eventType, mapping, value);
                        }

                    }
                );
            }
            
        }
        
    }

}
