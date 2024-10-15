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
        
        public GameObject clickAvoid;

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
            GameManager.playerManager.StartRunning();
        }

        private void ResetPlayGameUI()
        {
            gamePanel.gameObject.SetActive(true);
            
            scoreText.text = "0";
            extraBonus.text = "";
            extraComboBonus.text = "";
            
            processSlider.value = 0;
            score = 0;
            isGameWon = false;
            
            processLeftText.text = GameManager.gamePropertiesInSave.currenLevel.ToString();
            processRightText.text = (GameManager.gamePropertiesInSave.currenLevel + 1).ToString();
            
            SetStarsTransform(false);
        }

        private void SetStarsTransform(bool b)
        {
            foreach (var star in stars)
            {
                star.gameObject.SetActive(b);
            }
        }

        public override void Update()
        {
            if (isGameWon) return;
            
            GameManager.playerManager.UpdatePlayer();

            UpdateSlider();
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

                GameManager.playerManager.zSpeed += p0 * 2; // i like this
            }
        }

        public override void Exit()
        {
            gamePanel.gameObject.SetActive(false);
            if (isGameWon)
            {
                GameManager.gamePropertiesInSave.totalMoney += score;
                GameManager.menuState.SetMenuStateUI();
            }
            
            isGameWon = false;
            
            extraBonus.text = "";
            extraComboBonus.text = "";
            
            SetStarsTransform(false);
            
            GameManager.StartCoroutine(GameManager.spawnerManager.ResetSpawners());
            Debug.Log("PlayingState Exit");
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