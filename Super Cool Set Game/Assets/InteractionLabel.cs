using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

/// <summary>
/// Displays the label on the UI when the player approaches an InteractableObject.
/// </summary>
public class InteractionLabel : MonoBehaviour
{
    [SerializeField] private TMP_Text label;        // the text to show
    private InteractableObject objectLabel = null;  // the interactable we are next to

    /// <summary>
    /// Sets the interactable object to display the label with.
    /// </summary>
    /// <param name="triggeredinteractableObject">The interactable we approached.</param>
    public void SetInteractableObject(InteractableObject triggeredinteractableObject)
    {
        objectLabel = triggeredinteractableObject;
        label.text = objectLabel.actionLabel;

        ShowLabel();
    }

    /// <summary>
    /// Clears the current interactable and clears the label.
    /// </summary>
    public void ClearInteractableObject()
    {
        objectLabel = null;
        label.text = "";

        HideLabel();
    }

    /// <summary>
    /// Displays the label
    /// </summary>
    private void ShowLabel()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Hides the label
    /// </summary>
    private void HideLabel()
    {
        gameObject.SetActive(false);
    }
}
