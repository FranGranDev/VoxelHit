using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cannons.Controllers
{
    [RequireComponent(typeof(CannonBase))]
    public class TestController : MonoBehaviour, IController
    {
        [SerializeField] private KeyCode fireKey = KeyCode.Space;
        private CannonBase cannon;

        private bool fire;

        private void Awake()
        {
            Bind(GetComponent<CannonBase>());
        }

        private void Update()
        {
            if(Input.GetKey(fireKey))
            {
                Fire();
            }
            if(Input.GetKeyUp(fireKey))
            {
                EndFire();
            }
        }
        private void FixedUpdate()
        {
            if(fire)
            {
                cannon.Fire();
            }
        }

        public void Bind(CannonBase cannon)
        {
            this.cannon = cannon;
        }
        public void Fire()
        {
            fire = true;
        }

        public void EndFire()
        {
            fire = false;
            cannon.EndFire();
        }
    }
}
