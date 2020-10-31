using Puzzle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static PuzzleLoader puzzleLoader = null;

    public static PuzzleLoader GetPuzzleLoader()
    {
        if (puzzleLoader == null)
            puzzleLoader = GameObject.Find("Puzzle Manager").GetComponent<PuzzleLoader>();

        return puzzleLoader;
    }

    [SerializeField] private Puzzle.Scriptable.Puzzle[] puzzles;

    public static void MarkPuzzleComplete(Puzzle.Scriptable.Puzzle puzzle)
    {
        if (!puzzle)
            Debug.Log("Puzzle is null. Please pass a valid puzzle.");


    }
}
