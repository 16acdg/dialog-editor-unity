using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Class representing a node in a weightless, directed dialog graph
/// </summary>
[Serializable]
public class DialogNode
{
    /// <summary>
    /// The written content of the dialog node.
    /// </summary>
    public string sentence;

    /// <summary>
    /// List of responses to the dialog prompt.
    /// </summary>
    public List<DialogResponse> responses;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:DialogNode"/> class.
    /// </summary>
    /// <param name="sentence">The written content of the dialog node.</param>
    public DialogNode(string sentence)
    {
        this.sentence = sentence;
        this.responses = new List<DialogResponse>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:DialogNode"/> class.
    /// </summary>
    /// <param name="sentence">The written content of the dialog node.</param>
    /// <param name="responses">List of responses to the dialog prompt.</param>
    public DialogNode(string sentence, List<DialogResponse> responses)
    {
        this.sentence = sentence;
        this.responses = responses;
    }

    /// <summary>
    /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:DialogNode"/>.
    /// </summary>
    /// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:DialogNode"/>.</returns>
    public override string ToString()
    {
        string output = "Content: ";
        output += this.sentence + "\n";
        output += "Responses: [\n";
        for (int i = 0; i < this.responses.Count; i++)
        {
            output += "\t" + this.responses[i].ToString();
        }
        output += "]";
        return output;
    }

} // end DialogNode class
