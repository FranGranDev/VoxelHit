using Services;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Voxel.Waves;
using DG.Tweening;
using Animations;
using System;
using Cysharp.Threading.Tasks;

namespace Managament
{
    public class VoxelModelPlace : MonoBehaviour, IFinalEvent, Initializable<GameInfo>
    {
        [Header("Final Move")]
        [SerializeField] private float waveMakeDelay;
        [SerializeField] private float confettiPlayDelay;
        [SerializeField] private float uiShowDelay;
        [Header("Components")]
        [SerializeField] private ScaleReturnAnimation radialShine;
        [SerializeField] private List<ParticleSystem> particles;
        [SerializeField] private RepairEffect repairEffect;
        [Header("Link")]
        [SerializeField] private MovingEnviroment movingEnviroment;

        private WaveMaker waveMaker;
        private ISoundPlayer soundPlayer;

        public void Initialize(GameInfo info)
        {
            soundPlayer = info.Components.SoundPlayer;

            repairEffect.Initilalize(GetComponentInChildren<IFillEvent>(), info.MainColor);
        }


        public async UniTask Execute()
        {
            await movingEnviroment.MoveToPoint(GameStates.Game, GameStates.Done);


            waveMaker = GetComponentInChildren<WaveMaker>();
            radialShine.Play();
            soundPlayer.PlaySound("win");

            await UniTask.Delay(waveMakeDelay);

            waveMaker.MakeWave(new WaveParams(WaveParams.Types.Double));

            await UniTask.Delay(confettiPlayDelay);

            soundPlayer.PlaySound("confetti");
            foreach (ParticleSystem particleSystem in particles)
            {
                particleSystem.Play();
            }

            await UniTask.Delay(uiShowDelay);
        }
    }
}
