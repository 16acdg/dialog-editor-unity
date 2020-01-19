using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Class representing a dialog response.
/// </summary>
[Serializable]
public class DialogResponse
{
    /// <summary>
    /// The acutal content of the response.
    /// </summary>
    public string response;

    /// <summary>
    /// The name of the function this response will trigger.
    /// </summary>
    public string triggerFunctionName = "";

    /// <summary>
    /// Index of the next dialog node to go to next if this response was selected
    /// </summary>
    public int nextSentence;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:DialogResponse"/> class.
    /// </summary>
    /// <param name="response">Response's written content.</param>
    public DialogResponse(string response)
    {
        this.response = response;
        this.nextSentence = -1;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:DialogResponse"/> class.
    /// </summary>
    /// <param name="response">Response's written content.</param>
    /// <param name="nextSentence">Index of the next dialog node to go to next if this response was selected.</param>
    public DialogResponse(string response, int nextSentence)
    {
        this.response = response;
        this.nextSentence = nextSentence;
    }

    /// <summary>
    /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:DialogResponse"/>.
    /// </summary>
    /// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:DialogResponse"/>.</returns>
    public override string ToString()
    {
        return "Goes to " + this.nextSentence + ". Response: " + this.response + ". Invokes method " + this.triggerFunctionName + "\n";
    }

} // end DialogResponse class
