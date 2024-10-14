using UnityEngine;
using UnityEngine.Serialization;

public class PlayerManager : MonoBehaviour
{
    [Header("Managers")] 
        public GameManager gameManager;
        
        [Header("Components")]
        public CharacterController controller;
        
        [Header("Controllers")]
        public InputController inputController;
        public PlayerAnimationController animationController;
        [Header("Visual")] 
        public Transform visual;

        [Header("Parameters")]
        public float xSpeed = 15f;
        public float zSpeed = 10f;

        public float minRotation = -15;
        public float maxRotation = 15;

        private float _minBorder;
        private float _maxBorder; 
    
        private void Start()
        {
            _minBorder = gameManager.targetA.position.x;
            _maxBorder = gameManager.targetB.position.x;
            
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
                animationController.SetPlayerIdle();
                return;
            }
            
            StartRunning();
            
            
            var moveX = controller.transform.right * (inputController.IsMouseX() * xSpeed);
            var moveZ = controller.transform.forward * zSpeed;
            var move = moveX + moveZ;

            controller.Move(move * Time.deltaTime);

 
            var newRot = Quaternion.Slerp(visual.rotation, controller.velocity.magnitude > 0.5f ?
                Quaternion.LookRotation(controller.velocity) : 
                Quaternion.identity, Time.deltaTime * controller.velocity.magnitude);
            
            newRot.x = Mathf.Clamp(visual.rotation.x, minRotation, maxRotation);
            visual.rotation = newRot;

            ClampPosition();
        }

        private void ClampPosition()
        {
            var clampedPosition = controller.transform.localPosition;
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, _minBorder, _maxBorder);
            controller.transform.localPosition = clampedPosition;
        }

        private void OnTriggerEnter(Collider other)
        {
            
        }

        public void ResetPlayer()
        {
            ResetPos();
            animationController.Reset();
            ResetInput();
        }
        private void ResetInput()
        {
            inputController.isMouseDown = false;
            inputController.canMove = false;
        }

        public void ResetPos()
        {
            var initPos = gameManager.playerInitialPosition;
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
        

}