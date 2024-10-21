using System.Collections;
using GameObjects.Base;
using GameObjects.Road;
using UnityEngine;

namespace GameObjects.Boss
{
    public class ChestBoss : Boss
    {
        
        protected static readonly int AttackMode = Animator.StringToHash("HitPlayer");

        public override void PlayerArrived(BossRoad bossRoadInGame)
        {
            base.PlayerArrived(bossRoadInGame);
            
            gameManager.playingState.score +=
                gameManager.playerManager.members.Count * gameManager.gamePropertiesInSave.currenLevel;
            
            gameManager.playerManager.TargetToATransform(this, false);
            
        }

        private IEnumerator StartBossMatch()
        {
            animator.SetBool(AttackMode, true);

            gameManager.playingState.isGameWon = true;

            yield return new WaitForSeconds(0.5f); // wait for punch to play
            
            gameManager.playingState.isGameFinished = true;

            var bonus = gameManager.gamePropertiesInSave.currenLevel * 100;
            gameManager.gamePropertiesInSave.totalMoney += bonus;
            
            BossRoad.GameFinished();

        }
        public override void TakeDamage(int memberDamageAmount)
        {
            Debug.Log("damage");
            health -= memberDamageAmount;
            StartCoroutine(StartBossMatch());
        }
    }
}