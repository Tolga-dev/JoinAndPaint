using System;
using System.Collections;
using GameObjects.Base;
using GameObjects.Boss;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Recruitment : GameObjectBase
{
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public PlayerAnimationController playerAnimationController;
    public Rigidbody rb;
    public int health;
    public int damageAmount;
    
    public PlayerManager _playerManager;
    public float maxDistance = 0.3f;
    
    public bool merged = false;

    public ParticleSystem dieEffect;
    public ParticleSystem surfaceBlood;

    public Transform accessorLeftHand;
    public Transform accessorRightHand;
    public Transform accessorHead;
    
    public void StartPlayer(PlayerManager playerManager)
    {
        _playerManager = playerManager;
        skinnedMeshRenderer.material = playerManager.recruitment.skinnedMeshRenderer.material;
        playerAnimationController.StartRunner();    
    }
    
    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        
        if (other.CompareTag("Obstacle"))
        {
            gameManager.memberManager.DestroyNewMember(this);
        }
    }

    protected override void DisableGameObject()
    {
        
    }
    
    public void Freeze()
    {
       rb.constraints = RigidbodyConstraints.FreezeAll;
    }
    public void UnFreeze()
    {
        rb.constraints = 
            RigidbodyConstraints.None | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }
    public void Die(PlayerManager playerManager)
    {
        SetParticlePosition(dieEffect);
        SetParticlePosition(surfaceBlood);
        
        Destroy(gameObject);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health < 0)
        {
            gameManager.memberManager.DestroyNewMember(transform.GetComponent<Recruitment>());
        }
    }
    

    public bool IsDefeated()
    {
        return health < 0;
    }
    
    public void Merge(Recruitment member, Transform playerPos)
    {
        StartCoroutine(MergeCoroutine(member, playerPos));    
    }

    private IEnumerator MergeCoroutine(Recruitment member, Transform playerPos)
    {
        while (merged == false)
        {
            var position = playerPos.transform.position;
            var distance = Vector3.Distance(member.rb.position, position);
            var canMerge =distance < member.maxDistance;

            if (canMerge)
            {
                playerPos.localScale += new Vector3(0.1f, 0.1f, 0.1f);
                merged = true;
                yield break;
            }
            else
            {
                member.playerAnimationController.StartRunner();
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
        }
        
        gameObject.SetActive(false);
        yield break;
    }


    public void Attack(Recruitment member, Boss target)
    {
        StartCoroutine(MoveToTarget(member, target));   
    }
    
    private IEnumerator MoveToTarget(Recruitment member, Boss target)
    {
        var controller = member.playerAnimationController;
        controller.SetStartFight();

        var coolDown = false;
        var playingState = _playerManager.gameManager.playingState;
        while (!playingState.isGameFinished || !playingState.isGameWon) // Continue moving until the game is won
        {
            var distance = Vector3.Distance(member.rb.position, target.transform.position);

            var canAttack =distance < member.maxDistance;
            
            if (canAttack && coolDown == false)
            {
                coolDown = true;
                
                int attackType = Random.Range(0, 3); // 0 for kick, 1 for punch
                controller.SetFightMethod(attackType);
                DamageTarget(member,target);
                
                yield return new WaitForSeconds(0.5f); // wait for kick to play
                coolDown = false;
            }
            else
            {
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
