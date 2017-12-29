using System;

/// \addtogroup client
/// @{

/// <summary>
/// An <see cref="Attribute"/> to indicate things to do.
/// Taken from https://www.linkedin.com/pulse/alternative-way-manage-to-dos-unity-using-c-custom-steve-sedlmayr.
/// </summary>
[ToDo("Add implementation for inspector usage from source url.")]
public class ToDo : Attribute
{

    /// <summary>
    /// The message in the todo.
    /// </summary>
    private string _message;

    /// <summary>
    /// The priority of the todo.
    /// </summary>
    private int _priority = Int32.MinValue;

    /// <summary>
    /// Gets the message.
    /// </summary>
    public string message
    {
        get
        {
            return _message;
        }
    }

    /// <summary>
    /// Gets the priority.
    /// </summary>
    public int priority
    {
        get
        {
            return _priority;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ToDo"/> class.
    /// </summary>
    public ToDo() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ToDo"/> class.
    /// </summary>
    /// <param name="message">Message.</param>
    /// <param name="priority">Priority.</param>
    public ToDo(string message, int priority = Int32.MinValue)
    {
        this._message = message;
        this._priority = priority;
    }

}
/// @}
