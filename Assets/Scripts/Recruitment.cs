using System;
using System.Collections;
using GameObjects.Boss;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Recruitment : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public PlayerAnimationController playerAnimationController;
    public Rigidbody rb;
    public int health;
    public int damageAmount;
    
    public PlayerManager _playerManager;
    public float maxDistance = 0.3f;

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

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health < 0)
        {
            _playerManager.gameManager.memberManager.DestroyNewMember(transform);
        }
    }
    

    public bool IsDefeated()
    {
        return health < 0;
    }

    public void Attack(Recruitment member, Boss target)
    {
        StartCoroutine(MoveToTarget(member, target));   
    }
    
    private IEnumerator MoveToTarget(Recruitment member, Boss target)
    {
        var playerAnimationController = member.playerAnimationController;
        playerAnimationController.SetStartFight();

        var coolDown = false;
        var playingState = _playerManager.gameManager.playingState;
        while (!playingState.isGameFinished || !playingState.isGameWon) // Continue moving until the game is won
        {
            var distance = Vector3.Distance(member.rb.position, target.transform.position);

            var canAttack =distance < member.maxDistance;
            
            if (canAttack && coolDown == false)
            {
                Debug.Log("Attack!");
                coolDown = true;
                
                int attackType = Random.Range(0, 3); // 0 for kick, 1 for punch
                playerAnimationController.SetFightMethod(attackType);
                DamageTarget(member,target);
                
                yield return new WaitForSeconds(0.5f); // wait for kick to play
                coolDown = false;
            }
            else
            {
                Debug.Log("Running!");
                member.playerAnimationController.StartRunner();
                var position = target.transform.position;
                var targetPosition = position;

                member.rb.MovePosition(Vector3.MoveTowards(member.rb.position, targetPosition,
                    _playerManager.zSpeed * Time.fixedDeltaTime));

                var lookDirection = (position - member.transform.position).normalized;
                
                if (lookDirection != Vector3.zero) // Avoid zero direction issues
                {
                    var targetRotation = Quaternion.LookRotation(lookDirection);
                    member.transform.rotation = Quaternion.Slerp(member.transform.rotation, targetRotation, 
                        Time.fixedDeltaTime * _playerManager.rotationSpeed);
                }
            }
            
            yield return new WaitForFixedUpdate(); // Ensure this runs in sync with the physics engine
        }
        
        // add animation
        member.playerAnimationController.SetFightMethod(3); // cheers 
        
        yield return new WaitForSeconds(0.5f); // wait for punch to play
    }
    private void DamageTarget(Recruitment member, Boss target)
    {
        target.TakeDamage(member.damageAmount); 
    }
}
