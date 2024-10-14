using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Recruitment : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public PlayerAnimationController playerAnimationController;
    public Rigidbody rb;
    public void StartPlayer(PlayerManager playerManager)
    {
        skinnedMeshRenderer.material = playerManager.recruitment.skinnedMeshRenderer.material;
        playerAnimationController.StartRunner();
    }

    public void OnTriggerEnter(Collider other)
    {
        
    }
}
