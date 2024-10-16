using System;
using UnityEngine;

[Serializable]
public class PlayerAnimationController
{
    public Animator animator;
        
    public static readonly int IsRunning = Animator.StringToHash("isRunning");
    public static readonly int IsFinished = Animator.StringToHash("isFinished");
    
    private static readonly int IsFighting = Animator.StringToHash("IsFighting");
    private static readonly int AttackMode = Animator.StringToHash("AttackMode");
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

   
    public void SetPlayerIdle()
    {
        SetAnimation(IsRunning, 0);
    }
    public void StartWinner()
    {
        SetAnimation(IsFinished, true);
    }

    public void SetStartFight()
    {
        SetAnimation(IsFighting, true);
    }

    public void SetFightMethod(int fightMode)
    {
        SetAnimation(AttackMode, fightMode);

    }
}