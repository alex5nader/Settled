using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An object that the player can interact with when they approach.
/// </summary>
public abstract class InteractableObject : MonoBehaviour
{
    public string actionLabel;      // the label to display when the player approaches the object
    public abstract void Execute(); // the method to execute when the player interacts
}
