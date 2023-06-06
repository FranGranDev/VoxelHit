using Cysharp.Threading.Tasks;

namespace Services
{
    public interface IAdsController
    {
        UniTask TryShowInter();
        UniTask<bool> ShowRewarded();


        public float InterLoad { get; }
        public bool InterReady { get; }
        public bool RewardedReady { get; }
        


        event System.Action<bool> OnInterReadyChanged;
        event System.Action<bool> OnRewardedReadyChanged;
    }
}
