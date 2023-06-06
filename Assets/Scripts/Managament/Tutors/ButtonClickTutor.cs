using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Services;
using Data;
using UI;

namespace Tutors
{
    /// <summary>
    /// Use only for scene transition tutorial
    /// </summary>
    public class ButtonClickTutor : MonoBehaviour, Initializable<GameInfo>
    {
        [Header("Activate Settings")]
        [SerializeField] private bool isEnabled = true;
        [SerializeField] private string savedId;
        [SerializeField] private int startLevel;
        [Header("Settings")]
        [SerializeField] private Transform clickTarget;
        [SerializeField] private Transform targetButton;
        [Space]
        [SerializeField] private UIPanel background;
        [SerializeField] private Transform parent;

        private ICursor cursor;

        public void Initialize(GameInfo info)
        {
            background.gameObject.SetActive(false);

            this.Delayed(Time.fixedDeltaTime, () =>
            {
                if (!isEnabled)
                    return;
                if (SavedData.LevelsDone < startLevel)
                    return;
                if (PlayerPrefs.GetInt(savedId, 0) == 1)
                    return;

                cursor = GetComponentInChildren<ICursor>(true);

                background.Initilize();

                StartCoroutine(TutorCour());

                PlayerPrefs.SetInt(savedId, 1);
            });
        }

        private IEnumerator TutorCour()
        {
            background.IsShown = true;

            yield return new WaitForSeconds(0.5f);

            targetButton.SetParent(parent);
            targetButton.SetAsLastSibling();


            cursor.Hidden = false;

            while(true)
            {
                cursor.Position = clickTarget.position; 

                yield return new WaitForSeconds(1f);

                cursor.Down();

                yield return new WaitForSeconds(0.33f);

                cursor.Up();
            }
        }
    }
}
