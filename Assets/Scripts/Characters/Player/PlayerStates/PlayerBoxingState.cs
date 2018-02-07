using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoxingState : IPlayerState
{
    private PlayerManager m_playerManager;

    public PlayerBoxingState(PlayerManager playerManager)
    {
        m_playerManager = playerManager;
    }

    public void OnStateEnter()
    {
        throw new NotImplementedException();
    }

    public void OnStateExit()
    {
        throw new NotImplementedException();
    }

    public void UpdateState()
    {
        throw new NotImplementedException();
    }

    public void OnAnimationMove()
    {

    }

    public void OnAnimatorIK()
    {

    }
}
