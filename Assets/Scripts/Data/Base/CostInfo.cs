using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [System.Serializable]
    public class CostInfo
    {
        [SerializeField] private int cost = 0;
        [Space]
        [SerializeField] private bool openByAds = false;
        [SerializeField] private int maxAds = 0;
        [SerializeField] private string adsId = "null";

        public int Cost
        {
            get
            {
                if (!openByAds)
                    return cost;

                return Mathf.Max(Mathf.RoundToInt(cost * (1 - (float)AdsWatched / (float)maxAds)), 0);
            }
        }
        public int AdsWatched
        {
            get
            {
                return PlayerPrefs.GetInt(adsId, 0);
            }
            set
            {
                PlayerPrefs.SetInt(adsId, value);
            }
        }
        public int RemainingAds
        {
            get
            {
                return Mathf.Max(maxAds - AdsWatched, 0);
            }
        }
        public bool OpenByAds
        {
            get => openByAds;
        }


        public void Initialize(string id)
        {
            adsId = id;
        }
    }
}
