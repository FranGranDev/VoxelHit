using Cysharp.Threading.Tasks;
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


        public async UniTask Execute()
        {
            await UniTask.Delay(finalDelay);

            soundPlayer.PlaySound("win");

            await UniTask.Delay(confettiPlayDelay);

            BoomEffect();
            soundPlayer.PlaySound("confetti");
            foreach (ParticleSystem particleSystem in particles)
            {
                particleSystem.Play();
            }


            await UniTask.Delay(uiDelay);
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