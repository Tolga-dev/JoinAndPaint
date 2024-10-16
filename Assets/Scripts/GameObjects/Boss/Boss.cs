using System;
using System.Collections;
using GameObjects.Road;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameObjects.Boss
{
    public class Boss : MonoBehaviour
    {
        public Animator animator;

        private GameManager _gameManager;
        private BossRoad _bossRoad;

        public float speed;
        public int health;
        public int damageAmount;
        public float maxFarToRunOnPlayer;
        public float zSpeed;

        private static readonly int IsFighting = Animator.StringToHash("IsFighting");
        private static readonly int AttackMode = Animator.StringToHash("AttackMode");

        public virtual void PlayerArrived(BossRoad bossRoadInGame)
        {
            _bossRoad = bossRoadInGame;
            _gameManager = _bossRoad.gameManager;

            _gameManager.playerManager.TargetToATransform(transform);
            Debug.Log("Player is arrived");
            StartCoroutine(StartBossMatch());
        }

        private IEnumerator StartBossMatch()
        {
            animator.SetBool(IsFighting, true);

            while (_bossRoad.gameManager.playerManager.members.Count > 0)
            {
                if(health <= 0)
                    break;
                
                var target = SelectClosestRecruitment(); // Method to get the closest member to attack
                var isTargetAvailable = target != null;
                
                while (isTargetAvailable)
                {
                    float distance = Vector3.Distance(transform.position, target.transform.position);
                    Debug.Log(distance);
                    
                    if (distance > maxFarToRunOnPlayer) // If far, run towards target
                    {
                        animator.SetFloat(AttackMode, 0); // running state
                        var direction = transform.position - target.transform.position;
                        direction.y = 0;
                        
                        transform.Translate(direction * (zSpeed * Time.deltaTime));
                    }
                    else 
                    {
                        animator.SetBool(IsFighting, true);
                        int attackType = Random.Range(1, 3); // 0 for kick, 1 for punch

                        animator.SetFloat(AttackMode, attackType); // running kick
                        yield return new WaitForSeconds(0.5f); // wait for kick to play

                        DamageTarget(target);

                       
                        
                        if (target.IsDefeated()) // Implement `IsDefeated()` to check the target's health
                        {
                            isTargetAvailable = false;
                        }
                    }
                    
                    if(health <= 0)
                        break;
                    
                    yield return null; // Continue checking in the next frame
                }
                animator.SetBool(IsFighting, false); // running state
                animator.SetFloat(AttackMode, 0); // running state
            }

            if (_bossRoad.gameManager.playerManager.members.Count == 0)
            {
                Debug.Log("YES");
                animator.SetFloat(AttackMode, 3); // running kick
            }
            else
            {
                Debug.Log("SEX");
            }
            
            yield return new WaitForSeconds(1f); // wait for punch to play
            
            _gameManager.playingState.isGameFinished = true;
            
            _bossRoad.GameFinished();
        }
        
        
        private Recruitment SelectClosestRecruitment()
        {
            Recruitment closest = null;
            float minDistance = float.MaxValue;

            foreach (var recruitment in _bossRoad.gameManager.playerManager.members)
            {
                float distance = Vector3.Distance(transform.position, recruitment.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closest = recruitment;
                }
            }

            return closest;
        }

        private void DamageTarget(Recruitment target)
        {
            target.TakeDamage(damageAmount); 
        }
        
    }
}