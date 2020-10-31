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
}
