using System.Collections.Generic;
using GameObjects.Prizes;
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
    public Chest foundChest;

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
        if (!inputController.canMove)
        {
            return;
        }

        direction.x = Mathf.Lerp(direction.x, inputController.IsMouseX(), Time.deltaTime * xSpeed);
        direction.z = 1;

        direction = Vector3.ClampMagnitude(direction, 1f);
 
        foreach (var member in members)
        {
            member.rb.rotation = 
                Quaternion.Slerp(member.rb.rotation, member.rb.velocity.magnitude > 0.5f 
                    ? Quaternion.LookRotation(member.rb.velocity) : Quaternion.identity, Time.deltaTime * rotationSpeed);
        }
    }

    private void FixedUpdate()
    {
        if (inputController.canMove)
        {
            var velocity = new Vector3(direction.x * xSpeed, 0f, direction.z * zSpeed);

            foreach (var member in members)
            {
                member.rb.velocity = velocity;
            }
        }
        else
        {
            foreach (var member in members)
                member.rb.velocity = Vector3.zero;
        }
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
    }

    private void ResetInput()
    {
        inputController.isMouseDown = false;
        inputController.canMove = false;
    }

    public void ResetPos()
    {
        var initPos = gameManager.playingState.playerInitialPosition;
        transform.position = initPos.position;
    }

    public void StartRunning()
    {
        animationController.StartRunner();
    }

    public void SetWin()
    {
        animationController.StartWinner();
        /*gameManager.SwitchToWinCam();
        gameManager.playingState.isGameWon = true;

        if (gameManager.playingState.score > 0)
        {
            gameManager.gamePropertiesInSave.currenLevel++;
        }*/
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.memberManager.AddNewMember(other.transform);
        }
    }


    public void GotHitReaction()
    {
        Debug.Log("die");
    }
}