
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Puzzles;
using Data;
using System;

namespace Factory
{
    public class PuzzleFactory : MonoBehaviour, IFactory<PuzzleId>
    {
        [Header("Data")]
        [SerializeField] private SavedData savedData;
        [SerializeField] private PuzzleModelsData puzzleData;
        [Header("Components")]
        [SerializeField] private Transform place;
        [Header("Test")]
        [SerializeField] private bool testMode;
        [SerializeField] private string testGroup;

        public PuzzleId Created { get; private set; }

        public void Create(PuzzleId puzzle)
        {
            PuzzleId other = place.GetComponentInChildren<PuzzleId>();
            if(other)
            {
                other.transform.parent = null;
                Destroy(other.gameObject);
            }

            try
            {
                if (testMode || puzzle == null)
                {
                    puzzle = puzzleData.GetModels.First(x => x.Group == testGroup).PuzzleId;
                    Created = Instantiate(puzzleData.GetPuzzle(puzzle).PuzzleId, place);
                    return;
                }
                Created = Instantiate(puzzle, place);
            }
            catch
            {
                Debug.LogError($"There is no PuzzleId with group {testGroup}");
            }
        }
    }
}
