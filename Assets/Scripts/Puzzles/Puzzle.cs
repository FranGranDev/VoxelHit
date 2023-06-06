using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Data;
using Services;

namespace Puzzles
{
    public class Puzzle : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] PuzzleId puzzleId;

        [Header("Components")]
        [SerializeField] private List<PuzzlePlace> items;
        [SerializeField] private Transform background;

        private GameInfo gameInfo;

        public PuzzleId PuzzleId => puzzleId;
        public List<PuzzlePlace> Items => items;


        public void Initialize(GameInfo gameInfo)
        {
            this.gameInfo = gameInfo;

            foreach(PuzzlePlace item in items)
            {
                item.Initialize(gameInfo);
            }
        }
        public void SetItems(PuzzleModelsData.Collection collection, PuzzleItem.InitTypes type = PuzzleItem.InitTypes.Static)
        {
            foreach (PuzzleModelsData.Item item in collection.Items)
            {
                SavedData.PuzzleItemInfo info = new SavedData.PuzzleItemInfo(item.Model);
                if (info.Opened && info.Placed)
                {
                    PuzzlePlace place = items.First(x => x.Index == item.Index);

                    PuzzleItem puzzle = Instantiate(item.Model.GetModel).AddComponent<PuzzleItem>();

                    puzzle.Initialize(gameInfo, type);

                    place.SetItem(puzzle);
                }
            }
        }
        public void HideBackground()
        {
            background.gameObject.SetActive(false);
        }
    }
}
