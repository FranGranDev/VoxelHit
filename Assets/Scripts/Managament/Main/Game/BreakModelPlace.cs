using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxel;

namespace Managament
{
    public class BreakModelPlace : MonoBehaviour, IFinalEvent, Initializable<GameInfo>
    {
        [Header("Final Move")]
        [SerializeField] private float finalDelay;
        [SerializeField] private float confettiPlayDelay;
        [SerializeField] private float uiDelay;
        [Header("Boom Effect")]
        [SerializeField] private Vector3 direction;
        [SerializeField] private float boomPower;
        [SerializeField] private float boomAngular;
        [SerializeField] private float randomize;
        [Header("Components")]
        [SerializeField] private Transform modelPlace;
        [SerializeField] private List<ParticleSystem> particles;

        private ISoundPlayer soundPlayer;

        public void Initialize(GameInfo info)
        {
            soundPlayer = info.Components.SoundPlayer;
        }

        
        

        public void Execute(System.Action onDone)
        {
            StartFinalEvent(onDone);
        }
        public void StartFinalEvent(System.Action onDone)
        {
            StartCoroutine(FinalEventCour(onDone));
        }
        private IEnumerator FinalEventCour(System.Action onDone)
        {
            yield return new WaitForSeconds(finalDelay);

            soundPlayer.PlaySound("win");

            yield return new WaitForSeconds(confettiPlayDelay);

            BoomEffect();
            soundPlayer.PlaySound("confetti");
            foreach (ParticleSystem particleSystem in particles)
            {
                particleSystem.Play();
            }


            yield return new WaitForSeconds(uiDelay);

            onDone?.Invoke();
            yield break;
        }

        private void BoomEffect()
        {
            VoxelObject voxel = GetComponentInChildren<VoxelObject>();
            if(voxel)
            {
                voxel.BoomEffect(direction * boomPower, boomAngular, randomize);
            }
        }
    }
}