using GameObjects.Base;
using GameObjects.Prizes;
using UnityEngine;

namespace GameObjects.Obstacle
{
    public class Obstacle : GameObjectBase
    {
        public override void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);
 
            if (isHitPlayer)
            {
                var playerController = gameManager.playerManager;
                playerController.GotHitReaction();
            }
        }
 
        protected override void DisableGameObject()
        {
            
        }
     
    }
}