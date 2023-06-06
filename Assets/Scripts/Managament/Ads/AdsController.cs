using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Services;
using Cysharp.Threading.Tasks;

namespace Ads
{
    public class AdsController : MonoBehaviour, IAdsController
    {
        [Header("States")]
        [SerializeField] private float currantReloadTime = 0f;
        [SerializeField] private float interReloadTime = 20f;
        [SerializeField] private bool interReloaded = false;
        [Header("Debug")]
        [SerializeField] private bool debugMode;


        private bool interReady;
        private bool rewardedReady;



        public bool InterReady
        {
            get
            {
                return true; //interReady
            }
            private set
            {
                if(interReady == value)
                {
                    return;
                }

                interReady = value;
                OnInterReadyChanged?.Invoke(value);
            }
        }
        public bool RewardedReady
        {
            get
            {
                return true; //rewardedReady;
            }
            private set
            {
                if (rewardedReady == value)
                {
                    return;
                }

                rewardedReady = value;
                OnRewardedReadyChanged?.Invoke(value);
            }
        }
        public float InterLoad
        {
            get => currantReloadTime / interReloadTime;
        }


        public event System.Action<bool> OnInterReadyChanged;
        public event System.Action<bool> OnRewardedReadyChanged;


        public void Initialize(float interReloadTime)
        {
            this.interReloadTime = interReloadTime;

            InterReloadCour();
        }
        public void ClearEvents()
        {
            OnInterReadyChanged = null;
            OnRewardedReadyChanged = null;
        }


        public async UniTask TryShowInter()
        {
            //implementation
            await UniTask.Delay(10);

            RestartInter();
        }
        private void RestartInter()
        {
            interReloaded = false;
            currantReloadTime = 0f;
        }


        public async UniTask<bool> ShowRewarded()
        {
            //implementation
            await UniTask.Delay(10);

            return true;
        }

        private async void InterReloadCour()
        {
            while (true)
            {

                while (currantReloadTime < interReloadTime)
                {
                    currantReloadTime += Time.fixedDeltaTime;
                    await UniTask.WaitForFixedUpdate();
                }
                interReloaded = true;
                currantReloadTime = 0f;

                await UniTask.WaitWhile(() => interReloaded);
            }
        }

    }
}
