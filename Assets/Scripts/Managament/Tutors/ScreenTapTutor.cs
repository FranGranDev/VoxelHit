using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cannons;
using Services;

namespace Tutors
{
    public class ScreenTapTutor : MonoBehaviour, Initializable<GameInfo>, IBindable<IGameEventsHandler>
    {
        [Header("Settings")]
        [SerializeField] private bool isEnabled = true;
        [SerializeField] private float idleTime = 2f;

        private Coroutine helpCoroutine;
        private ICursor cursor;
        private CannonBase cannon;
        private bool activated;

        public void Initialize(GameInfo info)
        {
            cursor = GetComponentInChildren<ICursor>(true);
            cannon = info.SceneContext.LevelTransform.GetComponentInChildren<CannonBase>(true);

            cannon.OnFire += Stop;

            activated = false;
        }
        public void Bind(IGameEventsHandler eventsHandler)
        {
            eventsHandler.OnStarted += OnStarted;
            eventsHandler.OnClearScene += OnClearScene;
        }

        private void OnClearScene(IGameEventsHandler obj)
        {
            Stop();
            activated = false;
        }

        private void OnStarted()
        {
            if (activated)
                return;
            helpCoroutine = StartCoroutine(HelpCour());
            activated = true;
        }

        private void Stop()
        {
            if (helpCoroutine != null)
            {
                StopCoroutine(helpCoroutine);
                helpCoroutine = null;
            }
            if (!cursor.Hidden)
            {
                cursor.Hidden = true;
            }
        }

        private IEnumerator HelpCour()
        {
            yield return new WaitForSeconds(idleTime);

            cursor.Hidden = false;

            while(!cursor.Hidden)
            {
                yield return new WaitForSeconds(1f);

                cursor.Down();

                yield return new WaitForSeconds(1f);

                cursor.Up();
            }
        }
    }
}
