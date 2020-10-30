using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractablePuzzleLoader : InteractableObject
{
    [SerializeField] private Puzzle.Scriptable.Puzzle puzzle;

    public override void Execute()
    {
        if (puzzle != null)
            GameManager.GetPuzzleLoader().BeginPuzzle(puzzle);
        else 
            Debug.Log("Puzzle is null. Set a puzzle in " + gameObject + "'s inspector view.");
    }
}
