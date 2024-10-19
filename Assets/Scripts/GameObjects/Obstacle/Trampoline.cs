using System.Collections;
using UnityEngine;

namespace GameObjects.Obstacle
{
    public class Trampoline : Obstacle
    {
        public float force;
        public Vector3 targetDirection;
        public override void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);

            if (other.CompareTag("Player"))
            {
                var recruitment = other.GetComponentInChildren<Recruitment>();
                StartCoroutine(recruitment.MoveUpwards(recruitment.transform.parent, force, targetDirection));
            }
            else if (other.CompareTag("Recruitment"))
            {
                var recruitment = other.GetComponent<Recruitment>();
                StartCoroutine(recruitment.MoveUpwards(recruitment.transform, force, targetDirection));
            }
        }

        
        public override void CloseCollider()
        {
            
        }
        protected override void DisableGameObject()
        {
            
        }
    }
}