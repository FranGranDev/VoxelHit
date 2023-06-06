using UnityEngine;
using Services;
using Animations;

namespace Voxel
{
    public interface IVoxel
    {
        public GameObject GameObject { get; }
        public Vector3 Position { get; set; }
        public Vector3 Scale { get; set; }
        public int ColorIndex { get; set; }
        public float Grayscale { get; }


        public Material Material { get; set; }
    }


    public interface IGameVoxel : IVoxel
    {
        public void Initilize(MonoBehaviour parent, GameTypes gameType);
        public void Bind(ISoundPlayer soundPlayer);

        public VoxelStates State { get; }
        public void Repair(ActionParams parameter);
        public void Break(ActionParams parameter);
        public void Restart(ActionParams parameter);

        public void AddImpulse(Vector3 force, Vector3 angular);

        public void SetWaveScale(float value, int index = 0);
    }
    public enum VoxelStates
    {
        Hidden, Repaired, Broken
    }

    public class RepairParams : ActionParams
    {
        public RepairParams(Vector3 center, float speed, AnimationData animation) : base(Types.Animate)
        {
            Center = center;
            Speed = speed;
            Animation = animation;
        }

        public Vector3 Center { get; }
        public float Speed { get; }
        public AnimationData Animation { get; }
    }
    public class BreakParams : ActionParams
    {
        public BreakParams(Vector3 center, float impulse, float upRatio, float zRatio, float centerRatio, float randomize) : base(Types.Animate)
        {
            Center = center;
            Impulse = impulse;
            Randomize = randomize;
            YRatio = upRatio;
            ZRatio = zRatio;
            CenterRatio = centerRatio;
        }

        public Vector3 Center { get; }
        public float Impulse { get; }
        public float Randomize { get; }
        public float YRatio { get; }
        public float ZRatio { get; }
        public float CenterRatio { get; }
    }
    public class RestartParams : ActionParams
    {
        public RestartParams(AnimationData animation, ActionTypes actionType, float jumpPower, float jumpRandomize) : base(Types.Animate)
        {
            Animation = animation;
            AnimationType = actionType;
            JumpPower = jumpPower;
            JumpRandomize = jumpRandomize;
        }

        public AnimationData Animation { get; }
        public ActionTypes AnimationType { get; }
        public float JumpPower { get; }
        public float JumpRandomize { get; }


        public enum ActionTypes { None, Jump };

    }


    public class ActionParams
    {
        protected ActionParams(Types actionType)
        {
            ActionType = actionType;
        }
        public ActionParams()
        {
            ActionType = Types.Instantly;
        }

        public Types ActionType { get; }

        public enum Types { Animate, Instantly }
    }
}
