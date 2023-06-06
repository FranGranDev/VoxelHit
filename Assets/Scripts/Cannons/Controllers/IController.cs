namespace Cannons.Controllers
{
    public interface IController
    {
        public void Bind(CannonBase cannon);
        public void Fire();
        public void EndFire();
    }
}
