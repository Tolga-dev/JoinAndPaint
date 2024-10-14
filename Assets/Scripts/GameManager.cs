using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class GameManager : MonoBehaviour
{
    // player
    public PlayerManager playerManager;
    
    // spawn ground prefab
    public GameObject groundPrefab;
    public Vector3 offset;
    public int spawnAmount;
     
    
    public void Start()
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            Instantiate(groundPrefab, offset * i, Quaternion.identity);
        }
    }
    
 
}
