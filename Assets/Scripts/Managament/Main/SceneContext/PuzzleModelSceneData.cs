using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Managament.SceneContext;

namespace Managament
{
    public class PuzzleModelSceneData : PuzzleSceneData
    {
        public PuzzleModelSceneData(PuzzleId puzzleId, ModelId modelId) : base(puzzleId)
        {
            ModelId = modelId;
        }
        public PuzzleModelSceneData(PuzzleId puzzleId, ModelId modelId, bool super) : base(puzzleId)
        {
            ModelId = modelId;
            Super = super;
        }

        public ModelId ModelId { get; }
        public bool Super { get; }
    }
    public class PuzzleSceneData : SceneData
    {
        public PuzzleSceneData(PuzzleId puzzle)
        {
            PuzzleId = puzzle;
        }

        public PuzzleId PuzzleId { get; set; }
    }
}
