using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxel.Data;
using Services;


namespace Voxel
{
    public class Voxel : MonoBehaviour, IGameVoxel
    {
        [Header("Info")]
        [SerializeField] private int colorIndex;

        [Header("Components")]
        [SerializeField] private new Rigidbody rigidbody;
        [SerializeField] private new Collider collider;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private MeshFilter meshFilter;

        [Header("Others")]
        [SerializeField] private Mesh simpleMesh;


        private ISoundPlayer soundPlayer;
        private bool playSound = false;

        private bool cascadeUsed;
        private Vector3 startScale;
        private Vector3 scale;
        private Vector3 startPosition;
        private Dictionary<int, float> waveScales = new Dictionary<int, float>();

        private MonoBehaviour parent;
        private Coroutine actionCoroutine;

        public GameObject GameObject
        {
            get => gameObject;
        }
        public VoxelStates State
        {
            get; private set;
        } = VoxelStates.Repaired;
        public Vector3 Position
        {
            get
            {
                return new Vector3(Mathf.RoundToInt(transform.localPosition.x),
                                   Mathf.RoundToInt(transform.localPosition.y),
                                   Mathf.RoundToInt(transform.localPosition.z));
            }
            set
            {
                transform.localPosition = new Vector3(Mathf.RoundToInt(value.x),
                                                      Mathf.RoundToInt(value.y),
                                                      Mathf.RoundToInt(value.z));
            }
        }
        public Vector3 Scale
        {
            get => scale;
            set
            {
                scale = value;

                float addScale = 0;
                foreach (float i in waveScales.Values)
                {
                    addScale += i;
                }

                transform.localScale = scale + Vector3.forward * addScale;
            }
        }
        public int ColorIndex
        {
            get => colorIndex;
            set => colorIndex = value;
        }
        public float Grayscale
        {
            get
            {
                if (meshRenderer.material == null)
                    return 0;

                if (meshRenderer.material.HasProperty("_SColor"))
                {
                    return Mathf.Lerp(meshRenderer.material.color.grayscale, meshRenderer.material.GetColor("_SColor").grayscale, 0.5f);
                }

                return meshRenderer.material.color.grayscale;
            }
        }
        public Material Material
        {
            get => meshRenderer.sharedMaterial;
            set
            {
                if (value == null)
                    return;
                meshRenderer.sharedMaterial = value;
            }
        }

        public Rigidbody Rigidbody
        {
            get => rigidbody;
        }


        public void Initilize(MonoBehaviour parent, GameTypes type)
        {
            this.parent = parent;

            startScale = transform.localScale;
            scale = startScale;
            startPosition = transform.localPosition;

            switch(type)
            {
                case GameTypes.Paint:
                    collider.enabled = true;
                    break;
                case GameTypes.Game:
                    collider.enabled = false;
                    break;
                case GameTypes.ShelfWatch:
                    Destroy(collider);
                    Destroy(rigidbody);
                    meshFilter.sharedMesh = simpleMesh;
                    break;
                case GameTypes.Puzzle:
                    collider.enabled = true;
                    Destroy(rigidbody);
                    meshFilter.sharedMesh = simpleMesh;
                    break;
                case GameTypes.Break:
                    collider.enabled = true;
                    break;
                case GameTypes.EventRoad:
                    collider.enabled = false;
                    meshFilter.sharedMesh = simpleMesh;
                    break;
            }
        }
        public void Bind(ISoundPlayer soundPlayer)
        {
            this.soundPlayer = soundPlayer;
        }
        public void Initilize(VoxelData data)
        {
            ColorIndex = data.ColorIndex;
            Position = data.Position;

            gameObject.name = $"Voxel {Position.x}:{Position.y}";
        }

        public void SetWaveScale(float value, int index)
        {
            if(!waveScales.ContainsKey(index))
            {
                waveScales.Add(index, value);
            }
            else
            {
                waveScales[index] = value;
            }

            Scale = scale; //fix;
        }

        public void Repair(ActionParams parameter)
        {
            if (State != VoxelStates.Hidden)
                return;
            State = VoxelStates.Repaired;

            switch (parameter.ActionType)
            {
                case ActionParams.Types.Instantly:
                    gameObject.SetActive(true);
                    break;
                case ActionParams.Types.Animate:
                    if(actionCoroutine == null)
                    {
                        actionCoroutine = parent.StartCoroutine(RepairCour(parameter as RepairParams));
                    }
                    break;
            }
        }
        private IEnumerator RepairCour(RepairParams parameter)
        {
            float time = (Position - parameter.Center).magnitude / parameter.Speed;

            yield return new WaitForSeconds(time + parameter.Animation.Delay);

            Scale = Vector3.zero;
            gameObject.SetActive(true);

            var wait = new WaitForFixedUpdate();


            time = 0;
            while(time < parameter.Animation.Time)
            {
                float ratio = time / parameter.Animation.Time;
                Scale = Vector3.LerpUnclamped(Vector3.zero, startScale, parameter.Animation.AnimationCurves[0].Evaluate(ratio));

                time += Time.fixedDeltaTime;
                yield return wait;
            }
            Scale = startScale;


            yield break;
        }

        public void Break(ActionParams parameter)
        {
            if (State != VoxelStates.Repaired)
                return;

            switch (parameter.ActionType)
            {
                case ActionParams.Types.Instantly:
                    State = VoxelStates.Hidden;
                    gameObject.SetActive(false);
                    break;
                case ActionParams.Types.Animate:
                    State = VoxelStates.Broken;
                    gameObject.SetActive(true);
                    if (actionCoroutine != null)
                    {
                        parent.StopCoroutine(actionCoroutine);
                    }
                    actionCoroutine = parent.StartCoroutine(BreakCour(parameter as BreakParams));
                    break;
            }
        }
        private IEnumerator BreakCour(BreakParams parameter)
        {
            yield return new WaitForSeconds(Random.Range(0, 0.1f));

            Vector3 direction = (Position - parameter.Center).normalized * parameter.CenterRatio;
            Vector3 randomize = Random.onUnitSphere * parameter.Randomize;
            Vector3 zRatio = Vector3.forward * (Random.Range(0, 2) == 1 ? 1 : -1) * parameter.ZRatio;
            Vector3 upRatio = Vector3.up * parameter.YRatio;

            direction = (direction + randomize + upRatio + zRatio).normalized;

            collider.enabled = true;
            rigidbody.isKinematic = false;

            float impulse = parameter.Impulse * Random.Range(0.75f, 1.25f);

            rigidbody.AddTorque(Random.onUnitSphere * impulse * 36, ForceMode.VelocityChange);
            rigidbody.AddForce(direction * impulse, ForceMode.VelocityChange);

            var wait = new WaitForFixedUpdate();
            Vector3 tempScale = scale;
            for(float time = 0; time < 0.25f; time += Time.fixedDeltaTime)
            {
                Scale = Vector3.Lerp(tempScale, Vector3.one, time / 0.25f);
                yield return wait;
            }

            actionCoroutine = null;

            playSound = soundPlayer != null && Random.Range(0, 1f) > 0.6f;

            yield break;
        }
        private void TryCascadeBreak(GameObject other)
        {
            if (cascadeUsed || gameObject.layer != other.layer)
                return;
            if (Random.Range(0, 1f) < 1f && other.TryGetComponent(out IGameVoxel voxel))
            {
                voxel.Break(new BreakParams(transform.position, 3, 0, 0, 0, 1));
                cascadeUsed = true;
            }
        }

        public void AddImpulse(Vector3 force, Vector3 angular)
        {
            rigidbody.AddForce(force, ForceMode.VelocityChange);
            rigidbody.AddTorque(angular, ForceMode.VelocityChange);
        }

        public void Restart(ActionParams parameter)
        {
            if (State != VoxelStates.Broken)
                return;

            switch (parameter.ActionType)
            {
                case ActionParams.Types.Instantly:
                    gameObject.SetActive(true);
                    State = VoxelStates.Repaired;
                    transform.localPosition = startPosition;
                    transform.localRotation = Quaternion.identity;
                    transform.localScale = startScale;

                    rigidbody.isKinematic = true;
                    collider.enabled = false;
                    break;
                case ActionParams.Types.Animate:
                    gameObject.SetActive(true);
                    State = VoxelStates.Repaired;
                    if (actionCoroutine != null)
                    {
                        StopCoroutine(actionCoroutine);
                    }
                    RestartParams restartParams = parameter as RestartParams;

                    switch(restartParams.AnimationType)
                    {
                        case RestartParams.ActionTypes.None:
                            actionCoroutine = StartCoroutine(RestartCour(restartParams));
                            break;
                        case RestartParams.ActionTypes.Jump:
                            actionCoroutine = StartCoroutine(RestartJumpCour(restartParams));
                            break;
                    }

                    break;
            }
        }
        private IEnumerator RestartJumpCour(RestartParams parameter)
        {
            float positionY = transform.localPosition.y;

            Vector3 direciton = (Vector3.up + Random.onUnitSphere * parameter.JumpRandomize).normalized;

            rigidbody.AddForce(direciton * parameter.JumpPower, ForceMode.VelocityChange);
            rigidbody.AddTorque(Random.onUnitSphere * (parameter.JumpPower * 30 + 180));


            yield return new WaitForSeconds(0.75f);

            yield return StartCoroutine(RestartCour(parameter));
        }
        private IEnumerator RestartCour(RestartParams parameter)
        {
            yield return new WaitForSeconds(Random.Range(0, parameter.Animation.Delay));

            rigidbody.isKinematic = true;
            collider.enabled = false;

            Vector3 position = transform.localPosition;
            Quaternion rotation = transform.localRotation;
            Vector3 scale = transform.localScale;


            var wait = new WaitForFixedUpdate();
            for (float time = 0; time < parameter.Animation.Time; time += Time.fixedDeltaTime)
            {
                float ratio = time / parameter.Animation.Time;

                transform.localPosition = Vector3.Lerp(position, startPosition, parameter.Animation.AnimationCurves[0].Evaluate(ratio));
                transform.localRotation = Quaternion.Lerp(rotation, Quaternion.identity, parameter.Animation.AnimationCurves[1].Evaluate(ratio));
                transform.localScale = Vector3.Lerp(scale, startScale, parameter.Animation.AnimationCurves[2].Evaluate(ratio));

                yield return wait;
            }

            transform.localPosition = startPosition;
            transform.localRotation = Quaternion.identity;
            transform.localScale = startScale;

            Scale = startScale;
            waveScales.Clear();

            actionCoroutine = null;

            yield break;
        }


        private void OnCollisionEnter(Collision collision)
        {
            if (State == VoxelStates.Broken)
            {
                TryCascadeBreak(collider.gameObject);

                if (playSound && collision.gameObject.layer != gameObject.layer)
                {
                    soundPlayer.PlaySound("voxel_break_hit");
                    playSound = false;
                }
            }
        }
    }
}