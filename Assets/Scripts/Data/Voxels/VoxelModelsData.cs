using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Data
{
    [CreateAssetMenu(fileName = "Models Data", menuName = "Game Data/Models Data")]
    public class VoxelModelsData : ScriptableObject
    {
        [Header("State")]
        [SerializeField] private bool validate = true;
        [SerializeField] private int nextIndexHind;
        [SerializeField, TextArea] private string errors;
        [Header("Models")]
        [SerializeField] private List<Item> items;
        [SerializeField] private List<Group> groups;


        public ModelId GetItem(int index)
        {
            return items.Where(x => !x.IsNull && x.Index == index).First().Model;
        }
        public List<ModelId> GetItems()
        {
            return items.Where(x => !x.IsNull).ToList().ConvertAll(x => x.Model);
        }
        public List<ModelId> GetItems(string group)
        {
            return items.Where(x => !x.IsNull && x.Group == group).ToList().ConvertAll(x => x.Model);
        }
        public List<Group> GetGroups()
        {
            return groups;
        }
        public Dictionary<string, List<ModelId>> GetDictionary()
        {
            Dictionary<string, List<ModelId>> dictionary = new Dictionary<string, List<ModelId>>();

            foreach(Item item in items)
            {
                if (item.IsNull)
                    continue;

                if(dictionary.ContainsKey(item.Group))
                {
                    dictionary[item.Group].Add(item.Model);
                }
                else
                {
                    dictionary.Add(item.Group, new List<ModelId>() { item.Model });
                }
            }

            return dictionary;
        }


        [Button("Apply Data to Prefabs")]
        private void ApplyData()
        {
#if UNITY_EDITOR
            foreach(Item item in items)
            {
                if (item.IsNull)
                    continue;

                if(item.Apply())
                {
                    UnityEditor.PrefabUtility.SavePrefabAsset(item.Model.GetModel);
                }
            }
#endif
        }

        private void OnValidate()
        {
            if (!validate)
                return;
            int next = 1;
            foreach (Item item in items)
            {
                item.OnValidate();
                if(!item.IsNull)
                {
                    next++;
                    if(groups.Count(x => x.Id == item.Group) == 0)
                    {
                        groups.Add(new Group(item.Group));
                    }
                }
            }
            nextIndexHind = next;
          

            errors = "All Good!";
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].IsNull)
                    continue;
                for(int j = 0; j < items.Count; j++)
                {
                    if (items[j].IsNull)
                        continue;
                    if (i == j)
                        continue;

                    if(items[i].Conflict(items[j]))
                    {
                        errors = $"Items conflict: {items[i].Name} and {items[j].Name}";
                    }
                }
            }

            foreach (Group group in groups)
            {
                if(items.Count(x => x.Group == group.Id) == 0)
                {
                    groups.Remove(group);
                }
            }
            groups = groups.Distinct().ToList();
        }


        [System.Serializable]
        private class Item
        {
            [SerializeField] private string name;
            [SerializeField] private bool ignore;
            [SerializeField] private ModelId model;
            [Space]
            [SerializeField] private int index;
            [SerializeField] private string group;


            public string Name => name;
            public int Index => index;
            public string Group => group;
            public bool IsNull => model == null;
            public ModelId Model
            {
                get => model;
            }

            public void OnValidate()
            {
                if (ignore || model == null)
                    return;

                name = model.name;
                group = group.ToLower();
            }
            public bool Conflict(Item other)
            {
                return index == other.index || model == other.model;
            }

            public bool Apply()
            {
                if (model.Index == index && model.Group == group)
                    return false;

                model.Index = index;
                model.Group = group;
                return true;
            }
        }

        [System.Serializable]
        public class Group
        {
            public Group(string groupId)
            {
                this.groupId = groupId;
            }


            [SerializeField] private string groupId;
            [SerializeField] private string groupName = "null";


            public string Id => groupId;
            public string Name => groupName;


        }
    }
}
