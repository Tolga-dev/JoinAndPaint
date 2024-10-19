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

    public bool merged = false;

    public ParticleSystem dieEffect;
    public ParticleSystem surfaceBlood;

    public Transform accessorLeftHand;
    public Transform accessorRightHand;
    public Transform accessorHead;

    public bool imPlayer;

    public void StartPlayer(PlayerManager playerManager)
    {
        _playerManager = playerManager;
        skinnedMeshRenderer.material = playerManager.recruitment.skinnedMeshRenderer.material;
        playerAnimationController.StartRunner();
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (isHitPlayer == false)
            base.OnTriggerEnter(other);

        else if (other.CompareTag("Obstacle"))
        {
            gameManager.memberManager.DestroyNewMember(this);
        }
    }

    public override void CloseCollider()
    {

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
            RigidbodyConstraints.None | RigidbodyConstraints.FreezeRotation;
    }

    public void Die(PlayerManager playerManager)
    {
        if (imPlayer)
        {
            playerManager.gameObject.SetActive(false);
            return;
        }

        SetParticlePosition(dieEffect);
        SetParticlePosition(surfaceBlood);


        Destroy(gameObject);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health < 0)
        {
            gameManager.memberManager.DestroyNewMember(this);
        }
    }


    public bool IsDefeated()
    {
        return health < 0;
    }


    public IEnumerator MergeCoroutine(Recruitment member, Transform playerPos)
    {
        _playerManager.recruitment.Freeze();
        member.playerAnimationController.StartRunner();

        while (!merged)
        {
            var position = playerPos.transform.position;
            var distance = Vector3.Distance(member.rb.position, position);
            var canMerge = distance < (_playerManager.maxMergeDistanceMove);

            Debug.Log(distance);

            if (canMerge)
            {
                Freeze(); // Assume this stops NPC motion
                merged = true;
                member.transform.position = _playerManager.mergePos.position;
                playerPos.localScale += new Vector3(_playerManager.scaleBigFactor, _playerManager.scaleBigFactor,
                    _playerManager.scaleBigFactor);
                yield break;
            }
            else
            {
                member.rb.MovePosition(Vector3.MoveTowards(member.rb.position, position,
                    Time.fixedDeltaTime));

                var lookDirection = (position - member.transform.position).normalized;

                if (lookDirection != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                    member.transform.rotation = Quaternion.Slerp(member.transform.rotation,
                        targetRotation, Time.fixedDeltaTime * _playerManager.rotationSpeed);
                }
            }

            yield return new WaitForFixedUpdate(); // Ensure the coroutine runs with physics updates
        }

        _playerManager.recruitment.UnFreeze();

    }


    public void Attack(Recruitment member, Boss target)
    {
        StartCoroutine(MoveToTarget(member, target));
    }

    private IEnumerator MoveToTarget(Recruitment member, Boss target)
    {
        var controller = member.playerAnimationController;
        controller.SetStartFight();

        var playingState = _playerManager.gameManager.playingState;
        while (!playingState.isGameFinished || !playingState.isGameWon) // Continue moving until the game is won
        {
            var distance = Vector3.Distance(member.rb.position, target.transform.position);

            var canMove = distance > _playerManager.maxDistanceMove;

            if (canMove)
            {
                Freeze();
                UnFreeze();
                // close rb 
                Debug.Log(name);
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
            else
            {
                Freeze();
                int attackType = Random.Range(0, 3); // 0 for kick, 1 for punch
                controller.SetFightMethod(attackType);
                DamageTarget(member, target);

                yield return new WaitForSeconds(0.5f); // wait for kick to play
            }

            yield return new WaitForFixedUpdate(); // Ensure this runs in sync with the physics engine
        }

        gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
        member.playerAnimationController.SetFightMethod(3); // cheers 

        yield return new WaitForSeconds(0.5f); // wait for punch to play
    }

    private void DamageTarget(Recruitment member, Boss target)
    {
        target.TakeDamage(member.damageAmount);
    }
    public IEnumerator MoveUpwards(Transform currentTransform, float force, Vector3 targetDirection)
    {
        Vector3 startPosition = currentTransform.position;
        Vector3 targetPosition = startPosition + targetDirection * force;

        float elapsedTime = 0f;
        float duration = 1f; // Adjust duration as needed for smoothness

        // Move upwards
        while (elapsedTime < duration)
        {
            currentTransform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        currentTransform.position = targetPosition; // Ensure it reaches the final position

        // Start moving downwards after reaching the top
        StartCoroutine(MoveDownwards(currentTransform, startPosition));
    }

    public IEnumerator MoveDownwards(Transform currentTransform, Vector3 originalPosition)
    {
        Vector3 startPosition = currentTransform.position; // Current position after upward movement
        Vector3 targetPosition = originalPosition; // Back to the original starting point
        targetPosition.z += (startPosition.z - targetPosition.z) * 2;
        targetPosition.y = 0;

        float elapsedTime = 0f;
        float duration = 1f; // Adjust duration as needed for smoothness

        // Move downwards
        while (elapsedTime < duration)
        {
            currentTransform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        currentTransform.position = targetPosition; // Ensure it reaches the final position (original position)
    }



}
