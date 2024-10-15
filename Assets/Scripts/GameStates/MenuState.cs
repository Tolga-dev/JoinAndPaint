using System;
using GameStates.Base;
using UnityEngine;
using UnityEngine.UI;

namespace GameStates
{
    [Serializable]
    public class MenuState : GameState
    { 
        // game spawn
        // prepare game
        // shop
        
        // setting
        [Header("Setting UI")]
        public Button startSettings;
        public Button exitSettings;
        public Transform settingPanel;
        
        public Button changeStatusMusicButton;
        public Button changeStatusSoundButton;

        public override void Init(GameManager gameManager)
        {
            base.Init(gameManager);
            SetUI();
        }

        public override void Enter()
        {
            // start music
            // start cam
            // start ui
            var save = GameManager.gamePropertiesInSave;
            var sound = GameManager.soundManager;
            
            sound.GameMusic(save.onMenuStateSound);
            
        }
        public override void Update()
        {
            GameManager.playerManager.inputController.HandleMouseInput();
            if (GameManager.playerManager.inputController.isMouseDown)
            { 
                GameManager.ChangeState(GameManager.playingState);
            }
        }
        private void SetUI()
        {
            SetSaveUI();

            var save = GameManager.gamePropertiesInSave;
            var soundManager = GameManager.soundManager;
            
            changeStatusMusicButton.onClick.AddListener(() =>
            {
                soundManager.ButtonClickSound();
                
                if (save.isGameMusicOn)
                {
                    soundManager.GameMusic(null);
                    save.isGameMusicOn = false;
                }
                else
                {
                    save.isGameMusicOn = true;
                    soundManager.GameMusic(save.onMenuStateSound);
                }
                
                ToggleButtonPosition(changeStatusMusicButton, save.isGameMusicOn);
            });
            
            changeStatusSoundButton.onClick.AddListener(() =>
            {
                soundManager.ButtonClickSound();
                save.isGameSoundOn = !save.isGameSoundOn;
                ToggleButtonPosition(changeStatusSoundButton, save.isGameSoundOn);
            });
            
            startSettings.onClick.AddListener(() =>
            {
                settingPanel.gameObject.SetActive(true);
                soundManager.ButtonClickSound();
            }); 
            exitSettings.onClick.AddListener(() =>
            {
                settingPanel.gameObject.SetActive(false);
                soundManager.ButtonClickSound();
            });
            
        }

        private void ToggleButtonPosition(Button button, bool isOn)
        {
            var rectTransform = button.GetComponent<RectTransform>();
            var width = rectTransform.rect.width;
            var vector2 = rectTransform.anchoredPosition;
            
            if (isOn)
            {
                vector2.x = 0;
            }
            else
            {
                vector2.x = width;
            }
            rectTransform.anchoredPosition = vector2;
        }
        private void SetSaveUI()
        {
            var save = GameManager.gamePropertiesInSave;
            
            // settings
            ToggleButtonPosition(changeStatusMusicButton, save.isGameMusicOn);
            ToggleButtonPosition(changeStatusSoundButton, save.isGameSoundOn);
            
            
        }
    }
}