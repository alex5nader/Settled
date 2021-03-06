﻿using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Controls player movement and interaction.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [HideInInspector] [SerializeField] private Rigidbody2D _rigidbody2d;
    [HideInInspector] [SerializeField] private Animator _animator;
    [SerializeField] private InteractionLabel label;            // preferably not use GameObject.Find or drag and drop but one use does not requite the creation of a UI manager
    
    [Range(1f, 10f)]
    [SerializeField] private int walkSpeed = 2;                 // the walk speed of the player
    
    private bool walking = false;                               // tells the animator if the player is walking
    private Vector2 direction;                                  // direction vector

    private readonly static int ppu = 32;                       // pixels per unit (NOTE: best to create CameraManager and reference from there but may not be necessary)
    private readonly static float pixelUnitSize = 1f / ppu;     // the size of one pixel
    // input axis names
    private readonly static string xAxisName = "Horizontal";
    private readonly static string yAxisName = "Vertical";
    private readonly static string submitButtonName = "Submit";

    private void Awake()
    {
        SetReferences();
    }

    private void Reset()
    {
        SetReferences();
    }

    private void SetReferences()
    {
        _rigidbody2d = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    // read input on Update
    private void Update()
    {
        direction = Vector2.zero;

        HandleInput();
    }

    /// <summary>
    /// Read input into direction.
    /// </summary>
    private void HandleInput()
    {
        direction = new Vector2(Input.GetAxis(xAxisName), Input.GetAxis(yAxisName));
    }

    // perform movement on physics intervals
    private void FixedUpdate()
    {
        PixelPerfectMove();
    }

    /// <summary>
    /// Moves the player at walkSpeed in pixel perfect movement
    /// </summary>
    private void PixelPerfectMove()
    {
        _rigidbody2d.velocity = walkSpeed * direction * Time.fixedDeltaTime * 2000 / ppu;

        walking = direction != Vector2.zero;

        if (walking)
        {
            _animator.SetFloat("PosX", direction.x);
            _animator.SetFloat("PosY", direction.y);
        }

        _animator.SetBool("walking", walking);
    }

    // perform snapping after the physics step (to remove jittering)
    private void OnCollisionStay2D(Collision2D collision)
    {
        SnapPositionToPixel();
    }

    /// <summary>
    /// Snaps the transform position to a pixel snapped rigidbody position
    /// </summary>
    private void SnapPositionToPixel()
    {
        transform.position = SnapVectorToPixel(_rigidbody2d.position, pixelUnitSize);
    }

    // the size of one pixel
    /// <summary>
    /// Snaps a Vector2 to the pixel unit size.
    /// </summary>
    /// <param name="vector">The vector to snap</param>
    /// <param name="unitSize">The pixel unit size to apply snapping with</param>
    /// <returns></returns>
    private Vector2 SnapVectorToPixel(Vector2 vector, float unitSize)
    {
        Vector2 remainder = new Vector2(vector.x % unitSize, vector.y % unitSize);

        // floor to pixel unit
        vector -= remainder;

        // round up to next unitSize based on remainder
        vector += new Vector2((remainder.x >= pixelUnitSize / 2) ? pixelUnitSize : 0, (remainder.y >= pixelUnitSize / 2) ? pixelUnitSize : 0);

        return vector;
    }

    InteractableObject currentInteractable = null;  // the interactable we are currently overlapping with

    private void OnTriggerExit2D(Collider2D collision)
    {
        // if we are leaving an interactable
        if (collision.CompareTag(LayerUtility.Tags.Interactable.ToString()))
        {
            // if the object marked as interactable actually has an interactable and it is the same as the one we entered
            if (collision.TryGetComponent<InteractableObject>(out InteractableObject otherInteractable)
                && otherInteractable == currentInteractable)
            {
                // clear the interactable
                currentInteractable = null;

                // clear the label
                label.ClearInteractableObject();
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // if we approach an interactable
        if (collision.CompareTag(LayerUtility.Tags.Interactable.ToString()))
        {
            // if the other object has an interactable
            if (collision.TryGetComponent<InteractableObject>(out InteractableObject otherInteractable))
            {
                // on first trigger overlap cycle
                if (currentInteractable == null)
                {
                    currentInteractable = otherInteractable;            // store the interactable we trigger
                    label.SetInteractableObject(currentInteractable);   // and set then display the interaction label
                }

                // if the player hits enter, start the action
                if (Input.GetButtonDown(submitButtonName))
                    otherInteractable.Execute();
            }
        }
    }
}
