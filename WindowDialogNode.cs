using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to represent a dialog node in the dialog editor window. 
/// Basically a GUI capsule for the DialodNode class.
/// </summary>
public class WindowDialogNode
{
    /// <summary>
    /// The width of all dialog nodes in the GUI editor.
    /// </summary>
    public static readonly float width = 80;

    /// <summary>
    /// The height of all dialog nodes in the GUI editor.
    /// </summary>
    public static readonly float height = 50;

    private Vector2 _position;
    /// <summary>
    /// Gets or sets the position.
    /// </summary>
    /// <value>The position.</value>
    public Vector2 Position
    {
        get
        {
            return _position;
        }
        set
        {
            _position = value;
            this._rect.position = _position;
        }
    }

    /// <summary>
    /// Dialog node content.
    /// </summary>
    public DialogNode content;

    private Rect _rect;
    /// <summary>
    /// Gets or sets the rect.
    /// </summary>
    /// <value>The rect.</value>
    public Rect Rect
    {
        get
        {
            return this._rect;
        }
        set
        {
            this._rect = value;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:WindowDialogNode"/> class.
    /// </summary>
    /// <param name="position">Position.</param>
    public WindowDialogNode(Vector2 position)
    {
        this._position = position;
        this._rect = new Rect(position, new Vector2(width, height));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:WindowDialogNode"/> class.
    /// </summary>
    /// <param name="position">Position.</param>
    /// <param name="content">Dialog node content.</param>
    public WindowDialogNode(Vector2 position, DialogNode content)
    {
        this._position = position;
        this.content = content;
        this._rect = new Rect(position, new Vector2(width, height));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:WindowDialogNode"/> class.
    /// Creates an instance of a DialogNode for you.
    /// </summary>
    /// <param name="position">Position.</param>
    /// <param name="sentence">Sentence.</param>
    /// <param name="responses">Responses.</param>
    public WindowDialogNode(Vector2 position, string sentence, List<DialogResponse> responses)
    {
        this._position = position;
        this.content = new DialogNode(sentence, responses);
        this._rect = new Rect(position, new Vector2(width, height));
    }

} // end WindowDialogNode
