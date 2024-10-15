using UnityEngine;

namespace GameObjects.Base
{
    public class GameObjectBase : MonoBehaviour
    {
        public GameManager gameManager;
        [Header("Components")]
        public Animator animator;
        public ParticleSystem hitPlayerEffect;

        [Header("Parameters")]
        public bool isHitPlayer = false;
        private static readonly int PlayerHit = Animator.StringToHash("HitPlayer");
        
        public virtual void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                CallPlayerGotHit(other.gameObject);
            }
        }
        private void CallPlayerGotHit(GameObject player)
        {
            gameManager.soundManager.PlayASound(gameManager.gamePropertiesInSave.playerHitSound);
    
            if(animator != null)
                animator.SetBool(PlayerHit, true);
            
            if (hitPlayerEffect != null)
            {
                var playerController = gameManager.playerManager;
                SetParticlePosition(hitPlayerEffect, playerController.prizeEffectSpawnPoint.transform);
                PlayAdditionalEffects(playerController);
            }

            isHitPlayer = true;
            
            DisableGameObject();
        }

        protected virtual void PlayAdditionalEffects(PlayerManager playerController)
        {
            
        }

        protected virtual void DisableGameObject()
        {
            enabled = false;
        }

        protected void SetParticlePosition(ParticleSystem currentParticle, Transform playerPos)
        {
            var particleTransform = currentParticle.transform;
            particleTransform.parent = playerPos;
            particleTransform.transform.localPosition = Vector3.zero;
    
            currentParticle.Play(); // Start particle system

            var main = currentParticle.main;
            Destroy(currentParticle.gameObject, main.duration + main.startLifetime.constantMax);
        }


    }
}