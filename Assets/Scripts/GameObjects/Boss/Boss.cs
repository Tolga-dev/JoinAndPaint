using System;
using System.Collections;
using GameObjects.Road;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace GameObjects.Boss
{
    public class Boss : MonoBehaviour
    {
        public Animator animator;

        public GameManager gameManager;
        protected BossRoad BossRoad;

        public int health;
        public int damageAmount;
        public float zSpeed;
        public float maxFarToRunOnPlayer;
        

        public Transform accessorLeftHand;
        public Transform accessorRightHand;
        public Transform accessorHead;

        public virtual void PlayerArrived(BossRoad bossRoadInGame)
        {
            BossRoad = bossRoadInGame;
            gameManager = BossRoad.gameManager;

            Debug.Log("Player is arrived");
        }

        public void DamageTarget(Recruitment target)
        {
            target.TakeDamage(damageAmount);
        }

        public virtual void TakeDamage(int memberDamageAmount)
        {
            Debug.Log("damage");
            health -= memberDamageAmount;
        }
    }
}