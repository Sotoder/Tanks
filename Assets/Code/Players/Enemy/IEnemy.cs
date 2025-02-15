using UnityEngine;

namespace MVC
{
    public interface IEnemy : IGamer, IPlayerTarget
    {
        void Fire(Transform target);
        public Transform transform { get; }
        public Enemy SetPool(BulletPool pool);
        public void IncreaceForce(float forceModifier);
        public void Reset();
        public int Id { get; }
        public float ForceModifer { get; }
        public float MaxHP { get; set; }
        public void UpdateHelthView();
        public void UpdateTurretMaterialFromLoad();
    }
}