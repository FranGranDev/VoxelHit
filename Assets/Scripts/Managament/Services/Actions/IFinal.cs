using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    public interface IFinal
    {
        UniTask<Result> Execute(int money);
        UniTask ExtraMoney(int money);
        void StopWheel();


        public class Result
        {
            public Result(bool adsWatched)
            {
                WatchAds = adsWatched;
                RewardRatio = 0f;
            }
            public Result(bool adsWatched, float rewardRatio)
            {
                WatchAds = adsWatched;
                RewardRatio = rewardRatio;
            }

            public bool WatchAds { get; }
            public float RewardRatio { get; }
        }
    }
}
