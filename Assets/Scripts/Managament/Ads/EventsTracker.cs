using UnityEngine;
using Services;


namespace Ads
{
    public class EventsTracker : ITrackEvents
    {
        //GA Here

        public void OnLevelStarted(int level)
        {
            Debug.Log($"Level started: {level}");
        }
        public void OnLevelCompleted(int level, int score = 0)
        {
            Debug.Log($"Level Compleated: {level}");
        }
        public void OnLevelFailed(int level, int score = 0)
        {
            Debug.Log($"Level failed: {level}");
        }

    }
}
