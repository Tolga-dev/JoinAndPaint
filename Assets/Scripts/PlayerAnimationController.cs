using System;
using UnityEngine;

[Serializable]
public class PlayerAnimationController
{
    public Animator animator;
        
    public static readonly int IsRunning = Animator.StringToHash("isRunning");
    public static readonly int IsCarrying = Animator.StringToHash("isCarrying");
    public static readonly int IsDeath = Animator.StringToHash("isDeath");
    public static readonly int IsFinished = Animator.StringToHash("isFinished");

    public void SetAnimation(int animId, bool status = true)
    {
        animator.SetBool(animId, status);
    }
    public void SetAnimation(int animId, float status = 0)
    {
        animator.SetFloat(animId, status);
    }

    public void Reset()
    {
        animator.gameObject.SetActive(false);
        animator.gameObject.SetActive(true);
    }

    public void StartRunner()
    {
        SetAnimation(IsRunning, 1);
    }

    public void SetPlayerHolding()
    {
        SetAnimation(IsCarrying, true);
    }
    public void SetPlayerIdle()
    {
        SetAnimation(IsRunning, 0);
    }
    public void StartWinner()
    {
        SetAnimation(IsFinished, true);
    }
    
}