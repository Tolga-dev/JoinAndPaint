using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Recruitment : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public PlayerAnimationController playerAnimationController;
    public Rigidbody rb;
    public int health;

    public PlayerManager _playerManager;
    public void StartPlayer(PlayerManager playerManager)
    {
        _playerManager = playerManager;
        skinnedMeshRenderer.material = playerManager.recruitment.skinnedMeshRenderer.material;
        playerAnimationController.StartRunner();    
    }

    public void Die(PlayerManager playerManager)
    {
        gameObject.SetActive(false);
    }

    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;
        if (health < 0)
        {
            _playerManager.gameManager.memberManager.DestroyNewMember(transform);
        }
    }

    public bool IsDefeated()
    {
        return health < 0;
    }
}
