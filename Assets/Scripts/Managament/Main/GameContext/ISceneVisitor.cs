using Managament.Shelfs;

namespace Managament
{
    public interface ISceneVisitor
    {
        public void Visited(RepairSceneContext scene);
        public void Visited(PuzzleSceneContext scene);
        public void Visited(InfinitySceneContext scene);
        public void Visited(PaintSceneContext scene);
        public void Visited(ShelfSceneContext scene);
        public void Visited(PaintedSceneContext scene);
        public void Visited(ShopSceneContext scene);
        public void Visited(PuzzleShelfSceneContext scene);
        public void Visited(PuzzleRepairSceneContext scene);
        public void Visited(EventRoadSceneContext scene);
        public void Visited(BreakSceneContext scene);
        public void Visited(LoadingSceneContext scene);
    }
}
