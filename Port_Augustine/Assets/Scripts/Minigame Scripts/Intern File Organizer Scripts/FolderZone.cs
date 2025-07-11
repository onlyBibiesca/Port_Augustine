using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FolderZone : MonoBehaviour
{
    [Tooltip("The paper tag this folder accepts (e.g. PAPER_HR, PAPER_IT)")]
    public string acceptedTag;

    [Tooltip("Is this folder the shredder? Applies harsher penalty.")]
    public bool IsShredder = false;

    public bool AcceptsTag(string tag)
    {
        return tag == acceptedTag;
    }
}
