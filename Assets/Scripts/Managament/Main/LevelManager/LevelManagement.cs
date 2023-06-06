using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Data;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace Managament.Levels
{
    public class LevelManagement : MonoBehaviour
    {
        public LevelManagement()
        {
            levelBehaviors = new Dictionary<LevelTypes, LevelManagerBehavior>()
            {
                {LevelTypes.Base, new GameLevelManagerBehavior(this) },
                {LevelTypes.Infinity, new InfinityLevelManagerBehavior(this) },
                {LevelTypes.Static, new StaticLevelManagerBehavior(this) },
            };
        }


        [HideInInspector] public bool editorMode;
        [HideInInspector] public bool testMode;

        [HideInInspector] public int editorLevelIndex;
        [HideInInspector] public int editorLocationIndex;

        [HideInInspector] public int editorMetaLevelIndex;

        [SerializeField] private LevelTypes levelTypes;
        [Space]
        [SerializeField] private LocationsList locationsList;
        [SerializeField] private List<Level> currantLevels = new List<Level>();
        [Space]
        [SerializeField] private Transform levelContainer;


        private LevelManagerBehavior CurrantBehavior
        {
            get
            {
                return levelBehaviors[levelTypes];
            }
        }
        private Dictionary<LevelTypes, LevelManagerBehavior> levelBehaviors;


        public int CurrantLocationIndex
        {
            get => CurrantBehavior.CurrantLocationIndex;
            set => CurrantBehavior.CurrantLocationIndex = value;
        }
        public int CurrantLevelIndex
        {
            get => CurrantBehavior.CurrantLevelIndex;
            set => CurrantBehavior.CurrantLevelIndex = value;
        }
        public LevelTypes LevelType 
        { 
            get => levelTypes;
        }

        public Transform LevelTransform => levelContainer;

        public Location CurrantLocation
        {
            get
            {
                return Locations[CurrantLocationIndex];
            }
        }
        public List<Location> Locations
        {
            get
            {
                if (locationsList == null)
                    return null;
                return locationsList.Locations;
            }
        }
        public List<Level> CurrantLevels
        {
            get => currantLevels;
            set
            {
                currantLevels.Clear();
                foreach (Level level in value)
                {
                    currantLevels.Add(new Level(level));
                }
            }
        }
        public Level CurrantLevel
        {
            get
            {
                try
                {
                    return currantLevels[CurrantLevelIndex];
                }
                catch
                {
                    Debug.LogError("<color=red>Error while loading level, check null reference, or zero list size!</color>");
                    return null;
                }
            }
        }


        public int LevelsDone
        {
            get => CurrantBehavior.LevelsDone;
            set => CurrantBehavior.LevelsDone = value;
        }
        public int LocationsDone
        {
            get => CurrantBehavior.LocationsDone;
            set => CurrantBehavior.LocationsDone = value;
        }
        public AudioClip LevelMusic
        {
            get
            {
                try
                {
                    return Locations[CurrantLocationIndex].music;
                }
                catch
                {
                    Debug.LogError("<color=red>Error getting prefab, check null reference, or zero list size!</color>");
                    return null;
                }
            }
        }


        public event System.Action OnLevelLoaded;
        public event System.Action OnDestroyLevel; 


        public void Initialize()
        {
#if !UNITY_EDITOR
            editorMode = false;
            testMode = false;
#endif
            if (LocationsDone >= Locations.Count)
            {
                CurrantLevels = GenerateRandomLevels();
            }
            else
            {
                CurrantLevels = Locations[CurrantLocationIndex].levels;
            }

            if (testMode)
            {
                OnLevelLoaded?.Invoke();
            }
            else
            {
                LoadLevel(CurrantLevel);
            }
        }
        public void Initialize(ModelId modelId)
        {
#if !UNITY_EDITOR
            editorMode = false;
            testMode = false;
#endif

            CurrantBehavior.Accept(modelId);

            if (LocationsDone >= Locations.Count)
            {
                CurrantLevels = GenerateRandomLevels();
            }
            else
            {
                CurrantLevels = Locations[CurrantLocationIndex].levels;
            }

            if (testMode)
            {
                OnLevelLoaded?.Invoke();
            }
            else
            {
                LoadLevel(CurrantLevel);
            }
        }
        public void Initialize(PuzzleId puzzleId)
        {
#if !UNITY_EDITOR
            editorMode = false;
            testMode = false;
#endif

            CurrantBehavior.Accept(puzzleId);

            if (LocationsDone >= Locations.Count)
            {
                CurrantLevels = GenerateRandomLevels();
            }
            else
            {
                CurrantLevels = Locations[CurrantLocationIndex].levels;
            }

            if (testMode)
            {
                OnLevelLoaded?.Invoke();
            }
            else
            {
                LoadLevel(CurrantLevel);
            }
        }

        public enum LevelTypes { Base, Infinity, Static}

        #region LevelSelect
        public void SelectLevel(int level, int location)
        {
            CurrantLocationIndex = location;
            CurrantLevelIndex = level;

            LoadLevel(CurrantLevel);
        }
        public void LoadLevel(Level level)
        {
            if (level.PrefabPath != "")
            {
                ClearPrevLevel();
                if (Application.isEditor)
                {
                    if (Application.isPlaying)
                    {
                        GameObject levelObj = GetLevelPrefab(level);
                        Instantiate(levelObj, levelContainer);

                        OnLevelLoaded?.Invoke();
                    }
                    else
                    {
#if UNITY_EDITOR
                        GameObject levelObj = GetLevelPrefab(level);
                        PrefabUtility.InstantiatePrefab(levelObj, levelContainer);
                        OnLevelLoaded?.Invoke();
#endif
                    }
                }
                else
                {
                    GameObject levelObj = GetLevelPrefab(level);
                    Instantiate(levelObj, levelContainer);

                    OnLevelLoaded?.Invoke();
                }
            }
        }

        public void FirstLevel()
        {
            LoadLevel(CurrantLevel);
        }
        public void NextLevel()
        {
            CurrantLevelIndex++;
            LevelsDone++;

            LoadLevel(CurrantLevel);
        }
        public void LevelDone()
        {
            CurrantLevelIndex++;
            LevelsDone++;

        }

        public void RestartLevel()
        {
            LoadLevel(CurrantLevel);
        }


        public List<Level> GenerateRandomLevels()
        {
            System.Random random = new System.Random(SavedData.Seed);
            List<Level> randomLevel = new List<Level>();

            Debug.Log(CurrantLocationIndex);
            randomLevel.AddRange(Locations[CurrantLocationIndex].levels);
            random.Shuffle(randomLevel);

            return randomLevel;
        }
        public List<Level> GenerateRandomLevels(int location)
        {
            System.Random random = new System.Random(SavedData.Seed);
            List<Level> randomLevel = new List<Level>();

            randomLevel.AddRange(Locations[CurrantLocationIndex].levels);
            random.Shuffle(randomLevel);

            return randomLevel;
        }


        private GameObject GetLevelPrefab(Level level)
        {
            return Resources.Load(level.PrefabPath, typeof(GameObject)) as GameObject;
        }
        private void ClearPrevLevel()
        {
            DOTween.Clear();

            for (int i = 0; i < levelContainer.childCount; i++)
            {
                GameObject destroyObject = levelContainer.GetChild(i).gameObject;
                destroyObject.transform.parent = null;

                DestroyImmediate(destroyObject);
            }

            OnDestroyLevel?.Invoke();
        }
        #endregion

        #region Internal
#if UNITY_EDITOR
        private void OnValidate()
        {
            Background background = GetComponentInChildren<Background>();
            if (background)
            {
                background.Initialize(CurrantLocation.background);
            }
        }

#endif
        #endregion
    }


    [System.Serializable]
    public class Level
    {
        public string Name;
        public GameObject LevelPrefab;
        [Space]
        public string PrefabPath;


        public Level(Level other)
        {
            Name = other.Name;
            LevelPrefab = other.LevelPrefab;
            PrefabPath = other.PrefabPath;
        }

    }

    [System.Serializable]
    public class Location
    {
        public string name;
        public List<Level> levels;
        [Space]
        public AudioClip music;
        public Background.Settings background;
        public Color mainColor;
        [Space]
        public bool useInRandom = true;
    }

}