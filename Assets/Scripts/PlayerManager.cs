using System.Collections;
using System.Collections.Generic;
using GameObjects.Boss;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("Managers")] public GameManager gameManager;

    [Header("Controllers")] public InputController inputController;
    public PlayerAnimationController animationController;

    [Header("Visual")] 
    public Recruitment recruitment;
    
    [Header("Parameters")] 
    public float xSpeed = 15f;
    public float zSpeed = 10f;
    public float rotationSpeed;

    public List<Recruitment> members = new List<Recruitment>();
    public Vector3 direction;
    public Transform prizeEffectSpawnPoint;
    public Transform canvasSpawnPoint;

    public bool memberEditMode = false;
    public float maxDistanceMove = 0.5f;
    public float maxMergeDistanceMove;
    public Transform mergePos;
    
    public float scaleBigFactor;

    private void Start()
    {
        members.Add(recruitment);
    }

    public void UpdatePlayer()
    {
        inputController.HandleMouseInput();

        if (memberEditMode)
            return;
        MovePlayer();
    }

    private void MovePlayer()
    {
        if (inputController.canMove == false)
        {
            foreach (var member in members)
            {
                member.rb.velocity = Vector3.zero;
                member.playerAnimationController.SetPlayerIdle();
            }
        }   
        else
        {
            SetDirection();
            SetRot();
            
            var velocity = new Vector3(direction.x * xSpeed, 0f, direction.z * zSpeed);
            foreach (var member in members)
            {
                member.rb.velocity = velocity;
                member.playerAnimationController.StartRunner();
            }
        }
    }

    private void SetRot()
    {
        foreach (var member in members)
        {
            
            member.transform.rotation = 
                Quaternion.Slerp(member.transform.rotation, member.rb.velocity.magnitude > 0.5f 
                    ? Quaternion.LookRotation(member.rb.velocity) : Quaternion.identity, Time.deltaTime * rotationSpeed);
        }
    }

    private void SetDirection()
    {
        direction.x = Mathf.Lerp(direction.x, inputController.IsMouseX(), Time.deltaTime * xSpeed);
        direction.z = 1;

        direction = Vector3.ClampMagnitude(direction, 1f);
    }
 

    public void ResetPlayer()
    {
        recruitment.UnFreeze();
        gameObject.SetActive(true);

        ResetPos();
        animationController.Reset();
        ResetInput();
        
        foreach (var member in members) 
        {
            if(member == recruitment)
                continue;
            Destroy(member);
        }
        members.Clear();
        
        members.Add(recruitment);
        recruitment.rb.velocity = Vector3.zero;
    }

    public void ResetInput()
    {
        inputController.isMouseDown = false;
        inputController.canMove = false;
    }

    public void ResetPos()
    {
        var initPos = gameManager.playingState.playerInitialPosition;
        transform.position = initPos.position;
        transform.rotation = Quaternion.Euler(Vector3.zero);
        transform.localScale = Vector3.one;
    }   
    
    public void SetWin()
    {
        recruitment.rb.velocity = Vector3.zero;
        gameManager.playingState.isGameFinished = true;

        if (gameManager.playingState.score > 0)
        {
            gameManager.gamePropertiesInSave.currenLevel++;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Recruitment"))
        {
            var rec = other.transform.GetComponent<Recruitment>();
            if (rec.isHitPlayer)
                return;
            gameManager.memberManager.AddNewMember(rec);
        }
    }
    public void GotHitReaction()
    {
        Debug.Log("die");
    }

    public void TargetToATransform(Boss target, bool isAttack) // hated ofc but, i was bored tbh
    {
        if (isAttack)
        {
            foreach (var member in members)
            {
                var rb = member.rb;
            
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.drag = 0f;
                rb.angularDrag = 0f;

                member.Attack(member, target);
            }
            
        }
        else
        {

           
            
            StartCoroutine(AttackToTarget(target));
        }
        
    }

    public IEnumerator AttackToTarget(Boss target)
    {
        yield return MergeMembers();
        
        if (members.Count > 1)
        {
            bool isAllMembersAreMerged = false;
            while (!isAllMembersAreMerged)
            {
                isAllMembersAreMerged = true;
                foreach (var member in members)
                {
                    if (member == recruitment)
                        continue;

                    if (member.merged == false)
                    {
                        isAllMembersAreMerged = false;
                        break; // Exit the loop once you find an unmerged member
                    }
                }

                // Optionally, add a small delay between each check to avoid continuous looping
                yield return null;
            }
        }

        // Now that all members are merged, perform the attack
        recruitment.Attack(recruitment, target);
    }

    
    public IEnumerator MergeMembers()
    {
        List<Coroutine> mergeCoroutines = new List<Coroutine>();

        foreach (var member in members)
        {
            var rb = member.rb;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.drag = 0f;
            rb.angularDrag = 0f;

            if (members.Count == 1)
                break;

            if (member == recruitment)
                continue;

            Coroutine mergeCoroutine = StartCoroutine(member.MergeCoroutine(member, transform));
            mergeCoroutines.Add(mergeCoroutine);
        }

        var incMe = members.Count / 20;
        maxMergeDistanceMove += incMe * 0.1f;
        
        foreach (var mergeCoroutine in mergeCoroutines)
        {
            yield return mergeCoroutine;
        }
        
        maxMergeDistanceMove -= incMe * 0.1f;
        
    }
    
}