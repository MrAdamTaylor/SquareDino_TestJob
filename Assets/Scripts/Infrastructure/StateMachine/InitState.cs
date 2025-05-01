using System;
using System.Collections.Generic;
using Infrastructure.DI.Container;
using UnityEngine;

public class InitState : IState
{
    private IFactory _enemyFactory;
    private IFactory _playerFactory;
    
    public InitState(GameStateMachine gameStateMachine, Container container)
    {
        var scope = container.CreateScope();
        _enemyFactory = (IFactory)scope.Resolve(typeof(EnemyFactory));
        _playerFactory = (IFactory)scope.Resolve(typeof(PlayerFactory));
        container.Construct(_enemyFactory);
        container.Construct(_playerFactory);
       
    }

    public void Enter()
    {
        GameObject parent = GameObject.Find("[GameObjects]");
        GameObject enemiesSpawnPointParent = GameObject.Find("[EnemiesSpawnPoints]");
        var spawnPoints = new List<Transform>();
        if (enemiesSpawnPointParent != null)
            spawnPoints = enemiesSpawnPointParent.transform.GetAllChildren();

        GameObject startPoint = GameObject.Find("StartWaypoint");

        _playerFactory.Create(startPoint.transform.position, parent.transform);
        
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            _enemyFactory.Create(spawnPoints[i].position, parent.transform);
        }
    }

    public void Exit()
    {
        throw new NotImplementedException();
    }
}