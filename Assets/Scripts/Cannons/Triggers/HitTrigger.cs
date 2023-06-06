using UnityEngine;


namespace Cannons.Bullets
{
    public sealed class HitTrigger : MonoBehaviour
    {
        [SerializeField] private RepairHitInfo.Types type;

        public event System.Action<RepairHitInfo> OnBulletEnter;
        public RepairHitInfo.Types HitType => type;

        private void OnTriggerEnter(Collider other)
        {
            if(other.TryGetComponent(out RepairBulletBase bullet))
            {
                OnBulletEnter?.Invoke(new RepairHitInfo(bullet, type));
            }
        }
    }
}
