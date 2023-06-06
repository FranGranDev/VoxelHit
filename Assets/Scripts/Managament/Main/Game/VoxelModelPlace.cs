using Services;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Voxel.Waves;
using DG.Tweening;
using Animations;
using System;

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


        public void Execute(Action onDone)
        {
            movingEnviroment.MoveToPoint(GameStates.Game, GameStates.Done,
                () => StartFinalEvent(onDone));
        }
        public void StartFinalEvent(Action onDone)
        {
            waveMaker = GetComponentInChildren<WaveMaker>();

            StartCoroutine(FinalEventCour(onDone));
        }
        private IEnumerator FinalEventCour(Action onDone)
        {
            radialShine.Play();

            soundPlayer.PlaySound("win");

            yield return new WaitForSeconds(waveMakeDelay);

            waveMaker.MakeWave(new WaveParams(WaveParams.Types.Double));

            yield return new WaitForSeconds(confettiPlayDelay);

            soundPlayer.PlaySound("confetti"); 
            foreach (ParticleSystem particleSystem in particles)
            {
                particleSystem.Play();
            }

            yield return new WaitForSeconds(uiShowDelay);

            onDone?.Invoke();
            yield break;
        }
    }
}
