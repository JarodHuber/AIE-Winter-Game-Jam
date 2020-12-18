using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DialogeInfo
{
    public enum PersonTalking
    {
        Scrooge, Other
    }

    public PersonTalking personTalking;
    [TextArea]
    public string text;
}
