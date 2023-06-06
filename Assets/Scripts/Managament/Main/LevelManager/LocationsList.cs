using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif


namespace Managament.Levels
{
    [CreateAssetMenu(fileName = "Locations", menuName = "Game Data/Locations")]
    public class LocationsList : ScriptableObject
    {
        [SerializeField] private int levelsCount;
        [SerializeField] private List<Location> locations;

        public int LevelsCount => levelsCount;
        public List<Location> Locations => locations;

        //private void OnValidate()
        //{
        //    int count = 0;
        //    foreach(Location location in locations)
        //    {
        //        count += location.levels.Count;
        //    }

        //    levelsCount = count;
        //}
        #region Internal
#if UNITY_EDITOR

        private void OnValidate()
        {
            bool dirty = false;

            for (int i = 0; i < locations.Count; i++)
            {
                Location location = locations[i];
                for (int a = 0; a < location.levels.Count; a++)
                {
                    Level level = location.levels[a];

                    if (level.LevelPrefab == null)
                        continue;
                    level.Name = level.LevelPrefab.name;
                    string path = AssetDatabase.GetAssetPath(level.LevelPrefab).Replace("Assets/Resources/", "").Replace(".prefab", "");
                    if (level.PrefabPath == "" || level.PrefabPath != path)
                        dirty = true;
                    level.PrefabPath = path;
                    level.LevelPrefab = null;
                }
            }
            if (dirty)
            {
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
        }

#endif
        #endregion
    }
}
