﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConversationPart
{
    public List<string> ConversationIDs { get; set; } = new List<string>();
    public string Speaker { get; set; } = ""; 
    public List<string> Contents { get; set; } = new List<string>(); 

    public ConversationNodeType Type { get; set; }

    public ConversationPart()
    {
        ConversationIDs = new List<string>();
        ConversationIDs.Add("");
        Speaker = "Speaker"; 
        Contents = new List<string>();
        Contents.Add(""); 
    }

}

public enum ConversationNodeType
{
    Basic,
    MultipleChoices
}