using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerCharacterController))]
public class PlayerInputController : MonoBehaviour
{
    private PlayerCharacterController _pcc;

    // Use this for initialization
	void Start ()
	{
	    _pcc = GetComponent<PlayerCharacterController>();
	}
	
	// Update is called once per frame
	void Update ()
    {
    }

    public void Move(Vector3 start, out Vector3 position, out Vector3 deltaMove)
    {
        switch (latestAxis)
        {
            case 0: // X
                if (Mathf.Abs(_input.x) > 0)
                    _input.y = 0;
                    _LastInputDirection = _input;
                break;
            case 1: // Y
                if (Mathf.Abs(_input.y) > 0)
                    _input.x = 0;
                    _LastInputDirection = _input;
                break;
            default:
                _input = Vector3.zero;
                break;
        }
        _pcc.Move(start, _input.normalized, out deltaMove, out position);
    }

    /// <summary>
    /// Grabs direction player is facing and calls the check to see if there is a player
    /// </summary>
    /// <param name="start"></param>
    /// <Author>
    /// Christopher Brennan
    /// </Author>
    public PlayerReference CharacterFacing(Vector3 start)
    {
        switch (latestDirection)
        {
            case 0: // X
                if (Mathf.Abs(_LastInputDirection.x) > 0)
                    _LastInputDirection.y = 0;
                break;
            case 1: // Y
                if (Mathf.Abs(_LastInputDirection.y) > 0)
                    _LastInputDirection.x = 0;
                break;
            default:
                _LastInputDirection = Vector3.zero;
                break;
        }

        return _pcc.CheckForPlayerRaycast(start, _LastInputDirection.normalized);
    }

    public Vector3 _input;
    public Vector3 _LastInputDirection;
    int latestAxis = -1; // 0 = X, 1 = Y, -1 = none
    int latestDirection = 1; // 0 = X, 1 = Y

    bool isDownX, isDownY;

    public void onAxis(InputDevice device, InputResponse.UpdateEvent evt, MappedAxis axis, AxisDirection value)
    {
        switch (axis)
        {
            case MappedAxis.Horizontal:
                this.onAxisHorizontal(device, evt, axis, value);
                break;
            case MappedAxis.Vertical:
                this.onAxisVertical(device, evt, axis, value);
                break;
            default:
                break;
        }
    }

    public void onAxis(InputDevice device, InputResponse.UpdateEvent evt, MappedAxis axis, float value)
    {
        switch (axis)
        {
            case MappedAxis.Horizontal:
                this.onAxisHorizontal(device, evt, axis, value);
                break;
            case MappedAxis.Vertical:
                this.onAxisVertical(device, evt, axis, value);
                break;
            default:
                break;
        }
    }

    public void onAxisHorizontal(InputDevice device, InputResponse.UpdateEvent evt, MappedAxis axis, AxisDirection value)
    {
        if (value == AxisDirection.Centered) return;
        switch (evt)
        {
            case InputResponse.UpdateEvent.DOWN:
                latestAxis = 0;
                latestDirection = 0;
                isDownX = true;
                break;
            case InputResponse.UpdateEvent.UP:
                isDownX = false;
                latestAxis = isDownY ? 1 : -1;
                latestDirection = 1;
                break;
            default:
                break;
        }
    }

    public void onAxisHorizontal(InputDevice device, InputResponse.UpdateEvent evt, MappedAxis axis, float value)
    {
        _input.x = value;
    }

    public void onAxisVertical(InputDevice device, InputResponse.UpdateEvent evt, MappedAxis axis, AxisDirection value)
    {
        if (value == AxisDirection.Centered) return;
        switch (evt)
        {
            case InputResponse.UpdateEvent.DOWN:
                latestAxis = 1;
                latestDirection = 1;
                isDownY = true;
                break;
            case InputResponse.UpdateEvent.UP:
                isDownY = false;
                latestAxis = isDownX ? 0 : -1;
                latestDirection = 0;
                break;
            default:
                break;
        }
    }

    public void onAxisVertical(InputDevice device, InputResponse.UpdateEvent evt, MappedAxis axis, float value)
    {
        _input.y = value;
    }

}
