using GameObjects.Base;
using UnityEditor;
using UnityEngine;

namespace GameObjects.Road
{
    public class FinishLine : GameObjectBase
    {
        public BossRoad bossRoad;
        
        public override void OnTriggerEnter(Collider other)
        {
            gameManager = bossRoad.gameManager;
            base.OnTriggerEnter(other);

            if (isHitPlayer)
            {
                bossRoad.PlayerArrived();
            }
        }
    }
}