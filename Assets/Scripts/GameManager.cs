using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class GameManager : MonoBehaviour
{
    // player
    public PlayerManager playerManager;
    public MemberManager memberManager;
    // spawn ground prefab
    public GameObject groundPrefab;
    public Vector3 offset;
    public int spawnAmount;
    public Transform targetA;
    public Transform targetB;
    public Transform playerInitialPosition;
    // spawn boss ground
    public GameObject bossPrefab;
    

    public void Start()
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            Instantiate(groundPrefab, offset * i, Quaternion.identity);
        }
        Instantiate(bossPrefab, offset * spawnAmount, Quaternion.identity);
    }

    private void Update()
    {
        playerManager.UpdatePlayer();
    }
}
