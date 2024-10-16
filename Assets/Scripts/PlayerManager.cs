using System.Collections;
using System.Collections.Generic;
using GameObjects.Prizes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

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

    private void Start()
    {
        members.Add(recruitment);
    }

    public void UpdatePlayer()
    {
        inputController.HandleMouseInput();
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
            gameManager.memberManager.AddNewMember(other.transform);
        }
    }
    public void GotHitReaction()
    {
        Debug.Log("die");
    }

    public void TargetToATransform(Transform target)
    {
        foreach (var member in members)
        {
            var rb = member.rb;
            
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.drag = 0f;
            rb.angularDrag = 0f;
            
            StartCoroutine(MoveToTarget(member, target));
        }
    }

    private IEnumerator MoveToTarget(Recruitment member, Transform target)
    {
        while (!gameManager.playingState.isGameFinished) // Continue moving until the game is won
        {
            var position = target.position;
            var targetPosition = position;

            member.rb.MovePosition(Vector3.MoveTowards(member.rb.position, targetPosition, zSpeed * Time.fixedDeltaTime));

            var lookDirection = (position - member.transform.position).normalized;
            
            if (lookDirection != Vector3.zero) // Avoid zero direction issues
            {
                var targetRotation = Quaternion.LookRotation(lookDirection);
                member.transform.rotation = Quaternion.Slerp(member.transform.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);
            }

            yield return new WaitForFixedUpdate(); // Ensure this runs in sync with the physics engine
        }
        yield break;
    }

}