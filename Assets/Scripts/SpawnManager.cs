using System.Collections;
using Controller.Spawners;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private GameManager _gameManager;
        
    [Header("Road Spawner")]
    public RoadSpawner roadSpawner;
        
    [Header("Obstacle Spawner")]
    public ObstacleSpawner obstacleSpawner;
        
    [Header("Prize Spawner")]
    public PrizeSpawner prizeSpawner;
        
    public void Starter()
    {
        _gameManager = GameManager.Instance;
        roadSpawner.Init(_gameManager);
        SpawnObjects(); 
    } 

    private void SpawnObjects()
    {
        SpawnRoads();
        //SpawnObstacles();
        //SpawnerPrizes();
    }

    private void SpawnerPrizes()
    {
        prizeSpawner.SpawnObject(this);
    }

    private void SpawnObstacles()
    {
        obstacleSpawner.SpawnObject(this);
    }

    private void SpawnRoads()
    {
        for (int i = 0; i < roadSpawner.GetNumberOfRoad(); i++)
        {
            roadSpawner.SpawnNormalRoad();
        }

        roadSpawner.SpawnBossObject();
    }

    public IEnumerator ResetSpawners()
    {
            
        roadSpawner.ResetRoads();
        obstacleSpawner.ResetObstacle();
        prizeSpawner.ResetPrize();

        SpawnObjects();

        yield return null;
    }
    public GameManager GameManager
    {
        get => _gameManager;
        set => _gameManager = value;
    }
    

}