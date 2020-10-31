using UnityEngine;

public class InteractablePuzzleLoader : InteractableObject
{
    [SerializeField] private Puzzle.Scriptable.Puzzle puzzle;
    [SerializeField] private GameObject gate;

    public static Puzzle.OnPuzzleComplete openGate;

    public override void Execute()
    {
        if (puzzle != null)
            GameManager.GetPuzzleLoader().BeginPuzzle(puzzle, OpenGate);
        else 
            Debug.Log("Puzzle is null. Set a puzzle in " + gameObject + "'s inspector view.");
    }

    private void OpenGate()
    {
        if (gate != null)
            gate.SetActive(false);
        else
            Debug.Log("Cannot open gate for puzzle " + puzzle + " because gate is null.");
    }
}
