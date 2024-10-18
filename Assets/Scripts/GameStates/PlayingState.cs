using System;
using System.Collections.Generic;
using GameStates.Base;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace GameStates
{
    [Serializable]
    public class PlayingState : GameState
    {
        // static
        [Header("Player Pos")] 
        public Transform playerInitialPosition;
        public float startPosZ;
        
        // parameters
        [Header("Player Parameters")] 
        public int score = 0;
        public bool isGameFinished = false;
        public bool isOnFinish = false;
        public bool isGameWon = false;
        
        [Header("Game UI")] 
        public Transform gamePanel;
        
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI processLeftText;
        public TextMeshProUGUI processRightText;
        
        public List<Transform> stars = new List<Transform>();
        public Slider processSlider;
        public Button reloadButton;

        public TextMeshProUGUI extraBonus;
        public TextMeshProUGUI extraComboBonus; 

        public override void Init(GameManager gameManager)
        {
            base.Init(gameManager);

            SetProperties();
        }
        public override void Enter()
        {
            GameManager.cameraController.SwitchToPlayerCam();
            
            ResetPlayGameUI();
            
            GameManager.soundManager.GameMusic(
                GameManager.gamePropertiesInSave.gameMusic
                    [Random.Range(0, GameManager.gamePropertiesInSave.gameMusic.Count)]);
            
            GameManager.playerManager.ResetPlayer();
        }


        public override void Update()
        {
            if (isGameFinished) return;

            UpdateUI();
            
            if (isOnFinish)
                return;
            
            GameManager.playerManager.UpdatePlayer();
        }
        public void UpdateUI()
        {
            scoreText.text = score.ToString();
            UpdateSlider();
        }
        public override void Exit()
        {
            gamePanel.gameObject.SetActive(false);
            
            if (isGameFinished)
            {
                GameManager.gamePropertiesInSave.totalMoney += score;
            }
            isGameFinished = false;
            isOnFinish = false;
            isGameWon = false;
            
            GameManager.menuState.SetMenuStateUI();
            
            extraBonus.text = "";
            extraComboBonus.text = "";
            
            SetStarsTransform(false);
            
            GameManager.playerManager.ResetPlayer();
            
            GameManager.StartCoroutine(GameManager.spawnerManager.ResetSpawners());
            Debug.Log("PlayingState Exit");
            
        }
        private void ResetPlayGameUI()
        {
            gamePanel.gameObject.SetActive(true);
            
            scoreText.text = "0";
            extraBonus.text = "";
            extraComboBonus.text = "";
            
            processSlider.value = 0;
            score = 0;
            isGameFinished = false;
            isGameWon = false;
            isOnFinish = false;
            
            processLeftText.text = GameManager.gamePropertiesInSave.currenLevel.ToString();
            processRightText.text = (GameManager.gamePropertiesInSave.currenLevel + 1).ToString();
            
            GameManager.playerManager.recruitment.rb.velocity = Vector3.zero;
            SetStarsTransform(false);
        }

        private void SetStarsTransform(bool b)
        {
            foreach (var star in stars)
            {
                star.gameObject.SetActive(b);
            }
        }


        private void UpdateSlider()
        {
            float endPosZ = GameManager.spawnerManager.roadSpawner.createdBossRoad.transform.position.z;
            float currentPosZ = GameManager.playerManager.transform.position.z;

            if (Math.Abs(endPosZ - startPosZ) > 0.1f) // Prevent division by zero
            {
                var normalizedValue = Mathf.Clamp01((currentPosZ - startPosZ) / (endPosZ - startPosZ));
                processSlider.value = normalizedValue;
            }
            else
            {
                processSlider.value = 0;
            }

            switch (processSlider.value)
            {
                case > 0.9f:
                    SetActiveStars(2);
                    break;
                case > 0.5f:
                    SetActiveStars(1);
                    break;
                case > 0.1f:
                    SetActiveStars(0);
                    break;
            }
        }

        private void SetActiveStars(int p0)
        {
            if (stars[p0].gameObject.activeSelf == false)
            {
                stars[p0].gameObject.SetActive(true);
                GameManager.soundManager.PlayASound(GameManager.gamePropertiesInSave.starSound);
            }
        }


        public void ClickAvoid(bool b)
        {
            GameManager.menuState.clickAvoid.SetActive(b);
        }
        
        private void SetProperties()
        {
            // upper ui
            reloadButton.onClick.AddListener(() =>
            {
                GameManager.serviceManager.adsManager.PlaySceneTransitionAds();
                GameManager.ChangeState(GameManager.playingState);
            });
            
            startPosZ = playerInitialPosition.transform.position.z;
        }

    }
}