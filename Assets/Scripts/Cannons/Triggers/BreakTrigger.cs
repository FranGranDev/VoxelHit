using UnityEngine;


namespace Cannons.Bullets
{
    public class BreakTrigger : MonoBehaviour
    {
        public event System.Action<BreakBulletInfo> OnBulletEnter;

        private bool entered;

        private void OnTriggerEnter(Collider other)
        {
            if (entered)
                return;
            if(other.TryGetComponent(out BreakBullet bullet))
            {
                OnBulletEnter?.Invoke(bullet.Info);
                entered = true;
            }
        }
    }
}
