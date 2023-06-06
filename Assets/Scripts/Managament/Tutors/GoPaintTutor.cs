using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
using Services;
using UI;

namespace Tutors
{
    public class GoPaintTutor : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool isEnabled;
        [SerializeField] private int startLevel;
        [SerializeField] private string savedId;
        [Header("Components")]
        [SerializeField] private UIPanel background;

        private ICursor cursor;


        private void Start()
        {
            cursor = GetComponentInChildren<ICursor>(true);

            TryStart(GetComponentInParent<ITutor<ShelfTutorValue>>());
        }

        public bool IsActive()
        {
            return isEnabled && SavedData.LevelsDone >= startLevel && PlayerPrefs.GetInt(savedId, 0) == 0;
        }
        public void TryStart(ITutor<ShelfTutorValue> obj)
        {
            if (!IsActive())
                return;

            Debug.Log("TUTOR ACTIVATED | GO PAINT");

            ShelfTutorValue value = obj.Start(null);

            if(value.Activated)
            {
                StartCoroutine(TutorCour(value));

                PlayerPrefs.SetInt(savedId, 1);
            }
        }

        private IEnumerator TutorCour(ShelfTutorValue value)
        {
            background.IsShown = true;

            cursor.Position = Camera.main.WorldToScreenPoint(value.ItemPosition);

            cursor.Hidden = false;

            while (true)
            {
                yield return new WaitForSeconds(1f);

                cursor.Down();

                yield return new WaitForSeconds(0.33f);

                cursor.Up();
            }
        }
    }
}
