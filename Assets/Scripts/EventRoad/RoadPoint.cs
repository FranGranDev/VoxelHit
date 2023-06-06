using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Voxel;

namespace EventRoad
{
    public class RoadPoint : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int index;
        [SerializeField] private States state;
        [SerializeField] private Types type;
        [Header("Components")]
        [SerializeField] private Transform topPoint;
        [SerializeField] private Transform voxelPlace;
        [SerializeField] private MeshRenderer meshRenderer;
        [Header("Materials")]
        [SerializeField] private Material currantMaterial;
        [SerializeField] private Material closedMaterial;
        [SerializeField] private Material doneMaterial;
        [Header("Effects")]
        [SerializeField] private List<ParticleSystem> particles;



        private IHaptic haptic;
        private ISoundPlayer soundPlayer;


        private Dictionary<States, Material> materialStates;
        private VoxelObject voxelObject;


        public int Index 
        { 
            get => index;
        }
        public RoadPoint NextPoint
        {
            get;
            private set;
        }
        public RoadPoint PrevPoint
        {
            get;
            private set;
        }
        public Vector3 position
        {
            get => topPoint.position;
        }
        public States State
        {
            get => state;
            set
            {
                if (state == value)
                    return;
                state = value;
                OnStateChanged(state);

                if(type == Types.First && value != States.Currant)
                {
                    State = States.Done;
                }
            }
        }
        public Types Type
        {
            get => type;
        }


        public void Initialize(GameInfo gameInfo, RoadPoint prevPoint, RoadPoint nextPoint, States state)
        {
            soundPlayer = gameInfo.Components.SoundPlayer;
            haptic = gameInfo.Components.Haptic;

            materialStates = new Dictionary<States, Material>()
            {
                {States.Closed, closedMaterial },
                {States.Currant, currantMaterial },
                {States.Done, doneMaterial },
                {States.None, closedMaterial },
            };

            State = state;
            PrevPoint = prevPoint;
            NextPoint = nextPoint;

            switch(Type)
            {
                case Types.Model:
                    voxelObject = voxelPlace.GetComponentInChildren<VoxelObject>();
                    voxelObject.InitializeRoad();

                    if (State != States.Closed)
                    {
                        voxelPlace.gameObject.SetActive(false);
                    }
                    break;
            }
        }
        private void OnStateChanged(States state)
        {
            meshRenderer.material = materialStates[state];
        }

        public void OnStartEnter()
        {
            switch(Type)
            {
                case Types.Model:
                    voxelObject.gameObject.SetActive(true);
                    voxelObject.Demolish();

                    haptic.VibrateHaptic();
                    break;
                case Types.Final:
                    voxelPlace.DOScale(Vector3.zero, 0.5f)
                        .SetEase(Ease.InOutSine)
                        .SetDelay(0.25f);
                    voxelPlace.DORotate(Vector3.up * 180, 0.5f, RotateMode.WorldAxisAdd)
                        .SetEase(Ease.InOutSine)
                        .SetDelay(0.25f);

                    haptic.VibrateHaptic();
                    break;
            }
        }
        public void OnEnter()
        {
            switch (Type)
            {
                case Types.Model:
                    soundPlayer.PlaySound("model_opened");
                    break;
                case Types.Final:
                    StartCoroutine(FinalCour());
                    break;
            }
        }


        public void Hide()
        {
            voxelPlace.gameObject.SetActive(false);
        }

        private IEnumerator FinalCour()
        {
            soundPlayer.PlaySound("win");

            VoxelSpawner spawner = GetComponentInChildren<VoxelSpawner>();
            if(spawner)
            {
                spawner.StartSpawn();
            }

            yield return new WaitForSeconds(0.25f);

            soundPlayer.PlaySound("confetti");
            foreach (ParticleSystem particle in particles)
            {
                particle.Play();
                yield return new WaitForSeconds(0.05f);
            }
            yield break;
        }


        public enum States { None, Closed, Done, Currant}
        public enum Types { First, Model, Final}
    }
}
