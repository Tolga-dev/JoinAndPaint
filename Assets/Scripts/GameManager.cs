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

    [Header("Game States")]
    public GameState CurrentState;
    public MenuState menuState;
    public PlayingState playingState;
    
    [Header("Game Save")]
    public GamePropertiesInSave gamePropertiesInSave;


    public void Start()
    {
        menuState.Init(this);
        playingState.Init(this);

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

}
