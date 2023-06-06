using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
using Cannons;
using UI;
using Services;

namespace Tutors
{
    public class ScreenHoldTutor : MonoBehaviour, Initializable<GameInfo>, IBindable<IGameEventsHandler>
    {
        [Header("Settings")]
        [SerializeField] private bool isEnabled = true;
        [Space]
        [SerializeField] private string saveId = "fill_tutor";
        [SerializeField, Range(0, 1f)] private float targetValue = 0.5f;
        [SerializeField] protected UIPanel textPanel;


        private Coroutine helpCoroutine;
        private IFillable fillable;


        public void Initialize(GameInfo info)
        {
            fillable = info.SceneContext.LevelTransform.GetComponentInChildren<CannonBase>(true);
            textPanel.Initilize();
        }
        public void Bind(IGameEventsHandler eventsHandler)
        {
            if (PlayerPrefs.GetInt(saveId, 0) == 1)
                return;

            eventsHandler.OnStarted += OnStarted;
            eventsHandler.OnClearScene += OnClearScene;
        }

        private void OnClearScene(IGameEventsHandler obj)
        {
            Stop();
        }

        private void OnStarted()
        {
            helpCoroutine = StartCoroutine(WaitHoldCour());
        }

        private void Stop()
        {
            if (helpCoroutine != null)
            {
                StopCoroutine(helpCoroutine);
                helpCoroutine = null;
            }
        }

        private IEnumerator WaitHoldCour()
        {
            textPanel.IsShown = true;

            yield return new WaitUntil(() => fillable.Fill >= targetValue);

            textPanel.IsShown = false;
            PlayerPrefs.SetInt(saveId, 1);

            yield break;
        }
    }
}
