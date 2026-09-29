using UnityEngine;

public interface IState
{
    void Enter();
    void Execute();
    void Exit();
}