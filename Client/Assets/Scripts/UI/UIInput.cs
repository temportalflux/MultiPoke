using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIInput : MonoBehaviour {

    public UnityEngine.UI.Selectable[] fields;
    public RectTransform selector;

    public int numberOfColumns = 1;

    private int selected;

    private UnityEngine.UI.Selectable selection
    {
        get
        {
            return this.fields[this.selected];
        }
    }
    
	void Start ()
    {

        this.selected = 0;
        
    }
	
	public void down(InputDevice device, InputResponse.UpdateEvent type, MappedButton action)
    {
        switch (action)
        {
            case MappedButton.CONFIRM:
                if (this.selection is UnityEngine.UI.Button)
                {
                    ((UnityEngine.UI.Button)this.selection).onClick.Invoke();
                }
                else
                if (this.selection is UnityEngine.UI.InputField)
                {
                    //this.selection.GetComponent<InputField>().text = "Welcome";
                    Debug.Log(this.selection);
                    if (ManagerInput.INSTANCE.isInUse())
                    {
                        this.selection.GetComponent<InputField>().DeactivateInputField();
                        ManagerInput.INSTANCE.Unlock();
                    }
                    else
                    {
                        ManagerInput.INSTANCE.Lock();
                        this.selection.GetComponent<InputField>().ActivateInputField();
                    }
                }
                break;
            default:
                break;
        }
    }

    public void down(InputDevice device, InputResponse.UpdateEvent type, MappedAxis action, AxisDirection value)
    {
        if (value != AxisDirection.Centered)
        {

            switch (action)
            {
                case MappedAxis.Vertical:
                    
                    // Add value to current selection
                    this.selected = (this.selected - ((int)value) * numberOfColumns);
                    // Ensure yield is negative
                    if (this.selected < 0) this.selected += this.fields.Length;
                    // Find remainder
                    this.selected %= this.fields.Length;

                    this.selector.position = new Vector3(this.selector.position.x, this.fields[this.selected].gameObject.transform.position.y, 0);
                    
                    break;
                case MappedAxis.Horizontal:

                    // Add value to current selection
                    this.selected = (this.selected + (int)value);
                    // Ensure yield is negative
                    if (this.selected < 0) this.selected += this.fields.Length;
                    // Find remainder
                    this.selected %= this.fields.Length;

                    this.selector.position = new Vector3(this.selector.position.x, this.fields[this.selected].gameObject.transform.position.y, 0);


                    if (this.selector.rotation.z == 0)
                    {
                        this.selector.localEulerAngles = new Vector3(this.selector.rotation.x, this.selector.rotation.y, 180);
                    }
                    else
                    {
                        this.selector.localEulerAngles = new Vector3(this.selector.rotation.x, this.selector.rotation.y, 0);
                    }

                    break;
                default:
                    break;
            }

        }
    }

}
