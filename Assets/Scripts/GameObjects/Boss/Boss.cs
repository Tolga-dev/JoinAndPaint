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

        protected GameManager GameManager;
        protected BossRoad BossRoad;

        public int health;
        public int damageAmount;
        public float zSpeed;
        public float maxFarToRunOnPlayer;

        public virtual void PlayerArrived(BossRoad bossRoadInGame)
        {
            BossRoad = bossRoadInGame;
            GameManager = BossRoad.gameManager;

            Debug.Log("Player is arrived");
        }

        public void DamageTarget(Recruitment target)
        {
            target.TakeDamage(damageAmount);
        }

        public void TakeDamage(int memberDamageAmount)
        {
            Debug.Log("damage");
            health -= memberDamageAmount;
        }
    }
}