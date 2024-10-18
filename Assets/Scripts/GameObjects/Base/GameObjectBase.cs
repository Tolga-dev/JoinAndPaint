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
        public AudioClip playerHitSound;
        public bool isHitPlayer = false;
        private static readonly int PlayerHit = Animator.StringToHash("HitPlayer");
        
        
        public virtual void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                CallPlayerGotHit();
            }
            
        }
        public void CallPlayerGotHit()
        {
            CloseCollider();
            if(playerHitSound != null)
                gameManager.soundManager.PlayASound(playerHitSound);
    
            if(animator != null)
                animator.SetBool(PlayerHit, true);
            
            if (hitPlayerEffect != null)
            {
                var playerController = gameManager.playerManager;
                SetParticlePosition(hitPlayerEffect, hitPlayerEffect.transform);
                PlayAdditionalEffects(playerController);
            }

            isHitPlayer = true;
            
            DisableGameObject();
        }

        private void CloseCollider()
        {
            var component = GetComponent<Collider>();
            component.enabled = false;
        }

        protected virtual void PlayAdditionalEffects(PlayerManager playerController)
        {
            
        }

        protected virtual void DisableGameObject()
        {
            enabled = false;
        }

        protected void SetParticlePosition(ParticleSystem currentParticle, Transform parent = null)
        {
            if (parent == null)
            {
                currentParticle.transform.parent = null;
            }
            
            currentParticle.Play(); // Start particle system
            var main = currentParticle.main;
            Destroy(currentParticle.gameObject, main.duration + main.startLifetime.constantMax);
            
        }


    }
}