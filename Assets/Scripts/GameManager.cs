using System.Collections;
using Cinemachine;
using Controller;
using Core;
using GameStates;
using GameStates.Base;
using Save;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    // player
    public PlayerManager playerManager;
    public MemberManager memberManager;
    public SoundManager soundManager;
    public ServiceManager serviceManager;
    public SaveManager saveManager;
    public SpawnManager spawnerManager;
    
    [Header("Controllers")]
    public CameraController cameraController;
    
    [Header("Game States")]
    public GameState CurrentState;
    public MenuState menuState;
    public PlayingState playingState;
    
    [Header("Game Save")]
    public GamePropertiesInSave gamePropertiesInSave;

    public Transform targetA;
    public Transform targetB;

    public void Start()
    {
        saveManager.Load();

        menuState.Init(this);
        playingState.Init(this);
        spawnerManager.Starter();
        
        ChangeState(menuState);
    }

    public void Update()
    {
        CurrentState.Update();
    }

    public void ChangeState(GameState newState)
    {
        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

    public void OnCameraSwitch(ICinemachineCamera toCam, ICinemachineCamera fromCam) // event function
    {
        var firstCam = (CinemachineVirtualCamera)fromCam;
        var secondCam = (CinemachineVirtualCamera)toCam;
        if (firstCam == cameraController.bossCam && secondCam == cameraController.menuStateCam)
        {
            menuState.SetNewMotivationString(null);
            playingState.ClickAvoid(true);
            
            serviceManager.adsManager.PlaySceneTransitionAds();
            StartCoroutine(WaitForCameraBlendToFinish());
        }
    }

    private IEnumerator WaitForCameraBlendToFinish()
    {
        while (cameraController.cinemaMachineBrain.IsBlending)
        {
            yield return null;  // Wait until the next frame
        }
        StartCoroutine(saveManager.Save());
        playingState.ClickAvoid(false);
        menuState.SetNewMotivationString("Tap To Play!");
        Debug.Log("Arrived at menu state!");
    }
    
    private void OnApplicationQuit()
    {
        gamePropertiesInSave.lastTimeNextLevelAdWatched = 0;
        gamePropertiesInSave.lastTimeComboAdWatched = 0;
            
        StartCoroutine(saveManager.Save());
    }
    
}
