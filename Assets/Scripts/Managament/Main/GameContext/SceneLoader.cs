using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Managament
{
    public class SceneLoader : MonoBehaviour
    {
        [Header("Debug")]
        [SerializeField] private int preloadedScene;
        [SerializeField] private int loadedScene;

        private Dictionary<SceneContext.SceneTypes, int> scenesIndex;
        private Loading currantLoading;

        public int CurrantIndex
        {
            get => SceneManager.GetActiveScene().buildIndex;
        }
        public SceneContext.SceneTypes CurrantScene
        {
            get
            {
                int index = CurrantIndex;
                return scenesIndex.Where(x => x.Value == index).First().Key;
            }
        }

        public void Initialize(Dictionary<SceneContext.SceneTypes, int> scenesIndex)
        {
            this.scenesIndex = scenesIndex;

            SceneManager.activeSceneChanged += OnSceneChanged;
        }

        private void OnSceneChanged(Scene arg0, Scene arg1)
        {
            currantLoading = null;
        }

        public void ActivateOnLoad()
        {
            if (currantLoading == null)
                return;

            currantLoading.ActivateOnLoad = true;
        }
        public void PreloadScene(SceneContext.SceneTypes sceneType, ThreadPriority priority = ThreadPriority.Normal)
        {
            Application.backgroundLoadingPriority = priority;

            if (currantLoading == null)
            {
                currantLoading = new Loading(scenesIndex[sceneType], sceneType);
            }
            else if(currantLoading.SceneType != sceneType)
            {
                currantLoading = new Loading(scenesIndex[sceneType], sceneType);
            }

            preloadedScene = scenesIndex[sceneType];
        }
        public void GoScene(SceneContext.SceneTypes sceneType)
        {
            if (currantLoading == null)
            {
                currantLoading = new Loading(scenesIndex[sceneType], sceneType);
                currantLoading.GoScene();
            }
            else if (currantLoading.SceneType == sceneType)
            {
                currantLoading.GoScene();
            }
            else
            {
                SceneManager.LoadScene(scenesIndex[sceneType]);
            }

            currantLoading = null;

            loadedScene = scenesIndex[sceneType];
        }


        private class Loading
        {
            private AsyncOperation operation;
            private UniTask loading;
            
            public int Index { get; }
            public SceneContext.SceneTypes SceneType { get; }
            public bool ActivateOnLoad
            {
                set => operation.allowSceneActivation = true;
            }

            public Loading(int index, SceneContext.SceneTypes sceneType)
            {
                Index = index;
                SceneType = sceneType;

                StartLoading();
            }

            private void StartLoading()
            {
                operation = SceneManager.LoadSceneAsync(Index);
                operation.allowSceneActivation = false;
            }

            public void GoScene()
            {
                operation.allowSceneActivation = true;
            }
        }
    }
}
