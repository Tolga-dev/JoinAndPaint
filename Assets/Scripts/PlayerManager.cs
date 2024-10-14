using System.Collections.Generic;
using OuterAssets.Assets.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerManager : MonoBehaviour
{
    public InputController inputController;
    public GameManager gameManager;
    
    
    public List<Rigidbody>  rbList = new List<Rigidbody>();
    
    public bool MoveByTouch, gameState, attackToTheBoss;
    private Vector3 Direction;
    [SerializeField] private float runSpeed, velocity, swipeSpeed, roadSpeed;
  //  public Transform road;
    public static PlayerManager PlayerManagerCls;
    
    private void Start()
    {
        PlayerManagerCls = this;
        rbList.Add(transform.GetChild(0).GetComponent<Rigidbody>());
    }
    
    private void Update()
    {
        inputController.HandleMouseInput();
        
        if (gameState)
        {
            if (Input.GetMouseButtonDown(0))
            {
                MoveByTouch = true;
            }
        
            if (Input.GetMouseButtonUp(0))
            {
                MoveByTouch = false;
            }
        
            if (MoveByTouch)
            { 
            
                Direction.x = Mathf.Lerp(Direction.x,Input.GetAxis("Mouse X"), Time.deltaTime * runSpeed);
           
                Direction = Vector3.ClampMagnitude(Direction,1f);
           
              //  road.position = new Vector3(0f,0f,Mathf.SmoothStep(road.position.z,-100f,Time.deltaTime * roadSpeed));

                foreach (var stickman_Anim in rbList)
                    stickman_Anim.GetComponent<Animator>().SetFloat("run",1f);
            }
        
            else
            {
                foreach (var stickman_Anim in rbList)
                    stickman_Anim.GetComponent<Animator>().SetFloat("run",0f);
            }

            foreach (var stickManRb in rbList)
            {
                if (stickManRb.velocity.magnitude > 0.5f)
                {
                    stickManRb.rotation = Quaternion.Slerp(stickManRb.rotation,Quaternion.LookRotation(stickManRb.velocity), Time.deltaTime * velocity );
                }
                else
                {
                    stickManRb.rotation = Quaternion.Slerp(stickManRb.rotation,Quaternion.identity, Time.deltaTime * velocity );
                }
            }
        }
        else
        {
            /*if (!bossManager.BossManagerCls.BossIsAlive)
            {
                foreach (var stickMan in rbList)
                {
                    stickMan.GetComponent<Animator>().SetFloat("attackmode",4);
                }
            }*/
        }
        
           
        
    }
    
    private void FixedUpdate()
    {
        if (gameState)
        {
            if (MoveByTouch)
            {
                Vector3 displacement = new Vector3(Direction.x,0f,0f) * Time.fixedDeltaTime;
            
                foreach (var stickman_rb in rbList)
                    stickman_rb.velocity = new Vector3(Direction.x * Time.fixedDeltaTime * swipeSpeed,0f,0f) + displacement;
            }
            else
            {
                foreach (var stickman_rb in rbList)
                    stickman_rb.velocity = Vector3.zero;
            }
        }
        
    }
}
