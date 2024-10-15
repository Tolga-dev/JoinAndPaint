using System;
using GameStates.Base;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace GameStates
{
    [Serializable]
    public class MenuState : GameState
    { 
        // panel
        
        public Transform menuPanel;
        // game properties
        public TextMeshProUGUI paraAmount;
            
        // game spawn
        // prepare game
        // shop
            // no ads
        [Header("Shop UI")]
        public Button startMarket;
        public Button exitMarket;
        public Button buyNoAds;
        public TextMeshProUGUI description;
        public TextMeshProUGUI shopResult;
        public Transform marketPanel;
        
        // setting
        [Header("Setting UI")]
        public Button startSettings;
        public Button exitSettings;
        public Transform settingPanel;
        
        public Button changeStatusMusicButton;
        public Button changeStatusSoundButton;
        public TextMeshProUGUI clickToStart;

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
        public override void Exit()
        {
            menuPanel.gameObject.SetActive(false);
            Debug.Log("MenuState Exit");
        }
        
        private void SetUI()
        {
            SetSaveUI();

            var save = GameManager.gamePropertiesInSave;
            var soundManager = GameManager.soundManager;
            
            // settings
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
            
            // markets
            startMarket.onClick.AddListener(() =>
            {
                soundManager.ButtonClickSound();
                soundManager.GameMusic(save.onMarketSound);
                marketPanel.gameObject.SetActive(true);
                
            });

            exitMarket.onClick.AddListener(() =>
            {
                soundManager.ButtonClickSound();
                soundManager.GameMusic(save.onMenuStateSound);
                marketPanel.gameObject.SetActive(false);
                
                shopResult.text = "";
            });
            
            buyNoAds.onClick.AddListener(() =>
            {
                var success = new UnityAction<string>((string result) =>
                {
                    soundManager.ButtonClickSound();
                    soundManager.PlayASound(save.purchasedSound);
                    GameManager.gamePropertiesInSave.isNoAds = true;
                    
                    SetShopUI();
                    
                    GameManager.serviceManager.adsManager.CleanUp();
                    shopResult.text = result;
                });
                
                var failed = new UnityAction<string>((string result) =>
                {
                    soundManager.PlayASound(save.failedClickSound);
                    shopResult.text = result;
                });
                GameManager.serviceManager.inAppPurchase.BuyItem(GameManager.gamePropertiesInSave.noAdsProductId,success,failed);
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
            SetShopUI();
            SetMoney();
        }

        private void SetMoney()
        {
            paraAmount.text = GameManager.gamePropertiesInSave.totalMoney + "$";
        }

        private void SetShopUI()
        {
            var save = GameManager.gamePropertiesInSave;
            if (save.isNoAds)
            {
                buyNoAds.gameObject.SetActive(false);
                description.text = "You have bought No Ads!";
            }
            else
            {
                description.text = $"Remove ads with {GameManager.serviceManager.inAppPurchase.ReturnLocalizedPrice(GameManager.gamePropertiesInSave.noAdsProductId)}"; // money might be changed
            }   
        }

        public void SetNewMotivationString(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                clickToStart.text = message;
                return;
            }

            var gamePropertiesInSave = GameManager.gamePropertiesInSave;
            
            clickToStart.text = gamePropertiesInSave.winTexts[Random.Range(0, gamePropertiesInSave.winTexts.Length)];
        }

    }
}