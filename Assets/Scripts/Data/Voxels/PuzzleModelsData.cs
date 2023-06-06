using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Data
{
    [CreateAssetMenu(fileName = "Special Models Data", menuName = "Game Data/Puzzles Data")]
    public class PuzzleModelsData : ScriptableObject
    {
        [Header("Models")]
        [SerializeField] private List<Collection> puzzles;
        [SerializeField] private List<string> groups;

        public Collection GetPuzzle(PuzzleId puzzleId)
        {
            return puzzles.FirstOrDefault(x => x.PuzzleId == puzzleId);
        }
        public List<Collection> GetModels => puzzles;
        public List<ModelId> GetItems(PuzzleId puzzleId)
        {
            return puzzles.Where(x => x.PuzzleId.Group == puzzleId.Group)
                .First()
                .Items
                .ConvertAll(x => x.Model);
        }
        public List<ModelId> GetAllItems()
        {
            List<ModelId> all = new List<ModelId>();
            foreach(Collection puzzle in puzzles)
            {
                all.AddRange(puzzle.Items.ConvertAll(x => x.Model));
            }
            return all;
        }
        public List<string> Groups => groups;


        [Button("Apply Data to Prefabs")]
        private void ApplyData()
        {
#if UNITY_EDITOR
            foreach (Collection puzzle in puzzles)
            {
                if(puzzle.Apply())
                {
                    UnityEditor.PrefabUtility.SavePrefabAsset(puzzle.PuzzleId.GetModel);
                }

                foreach (Item item in puzzle.Items)
                {
                    if (item.Apply(puzzle.Group))
                    {
                        UnityEditor.PrefabUtility.SavePrefabAsset(item.Model.GetModel);
                    }
                }
            }
#endif
        }
        private void OnValidate()
        {
            groups = new List<string>();
            foreach (Collection puzzle in puzzles)
            {
                puzzle.OnValidate();
                groups.Add(puzzle.Group);

                foreach (Item item in puzzle.Items)
                {
                    item.OnValidate();
                }
            }
        }


        [System.Serializable]
        public class Collection
        {
            [SerializeField] private string name;
            [Space]
            [SerializeField] private string group;
            [SerializeField] private PuzzleId puzzle;
            [SerializeField] private List<Item> items;
            [Space]
            [SerializeField] private int minLevelDone;
            [Space]
            [SerializeField] private Notflication notflication;

            public string Name
            {
                get => name;
            }
            public string Group
            {
                get => group;
                set => group = value;
            }
            public int MinLevel
            {
                get => minLevelDone;
            }
            public Notflication Notflication
            {
                get => notflication;
            }
            public PuzzleId PuzzleId
            {
                get => puzzle;
            }
            public List<Item> Items
            {
                get => items;
            }

            public bool Apply()
            {
                if (puzzle == null)
                    return false;
                if (puzzle.Group == group)
                    return false;

                puzzle.Group = group;
                return true;
            }
            public void OnValidate()
            {
                puzzle.Group = puzzle.Group.ToLower();
            }
        }
        [System.Serializable]
        public class Item
        {
            [SerializeField] private string name;
            [SerializeField] private int index;
            [SerializeField] private ModelId model;
            [Space]
            [SerializeField] private int reward;

            public int Index => index;
            public ModelId Model => model;
            public int Reward => reward;

            public bool Apply(string group)
            {
                if (model.Index == index && model.Group == group)
                    return false;

                model.Index = index;
                model.Group = group;
                return true;
            }

            public void OnValidate()
            {
                if (model == null)
                    return;
                name = model.name;
            }
        }
    }

    [System.Serializable]
    public class Notflication
    {
        [SerializeField] private bool enabled;
        [Space]
        [SerializeField] private string name;
        [Space]
        [SerializeField] private Sprite image;


        public bool Enabled => enabled;
        public string Name => name;
    }
}
