using System.Collections;
using GameObjects.Road;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameObjects.Boss
{
    public class FighterBoss : Boss
    {
        public TextMeshProUGUI healthUI;
        public Slider slider;
        public int maxHealth;
        protected static readonly int IsFighting = Animator.StringToHash("IsFighting");
        protected static readonly int AttackMode = Animator.StringToHash("AttackMode");
        
        public override void PlayerArrived(BossRoad bossRoadInGame)
        {
            maxHealth = health;
            base.PlayerArrived(bossRoadInGame);

            SetDamageUI();
            GameManager.playerManager.TargetToATransform(this, true);

            StartCoroutine(StartBossMatch());

        }

        private IEnumerator StartBossMatch()
        {
            animator.SetBool(IsFighting, true);

            while (BossRoad.gameManager.playerManager.members.Count > 0)
            {
                if (health <= 0)
                    break;

                var target = SelectClosestRecruitment(); // Method to get the closest member to attack
                var isTargetAvailable = target != null;

                while (isTargetAvailable)
                {
                    float distance = Vector3.Distance(transform.position, target.transform.position);

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

                    if (health <= 0)
                        break;

                    yield return null; // Continue checking in the next frame
                }

                animator.SetBool(IsFighting, false); // running state
                animator.SetFloat(AttackMode, 0); // running state
            }
            

            if (BossRoad.gameManager.playerManager.members.Count == 0)
            {
                animator.SetFloat(AttackMode, 3); // running kick
                GameManager.playingState.isGameWon = false;
            }
            else
            {
                GameManager.playingState.isGameWon = true;
                animator.enabled = false;
            }

            yield return new WaitForSeconds(0.5f); // wait for punch to play

            GameManager.playingState.isGameFinished = true;

            BossRoad.GameFinished();
        }


        private Recruitment SelectClosestRecruitment()
        {
            Recruitment closest = null;
            float minDistance = float.MaxValue;

            foreach (var recruitment in BossRoad.gameManager.playerManager.members)
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
        public override void TakeDamage(int memberDamageAmount)
        {
            base.TakeDamage(memberDamageAmount);
            SetDamageUI();
        }
        private void SetDamageUI()
        {
            if (health < 0)
            {
                slider.value = 0;
                healthUI.text = 0.ToString();
                return;
            }
            
            slider.value = (float)health / maxHealth;
            healthUI.text = health.ToString();
        }
       
    }
}