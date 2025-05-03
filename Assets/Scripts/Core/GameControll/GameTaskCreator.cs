using System.Collections.Generic;
using Core.Other;
using Infrastructure.DI.Container;
using Infrastructure.DI.Injector;
using UnityEngine;

namespace Core.GameControll
{
    public class GameTaskCreator
    {
        //NOTE How many hypothetical rounds should the pool be reserved for?
        //NOTE If there is no progression in the game (there are no more opponents), it is recommended to bet 3
        private const int ACTIVE_TASK_COUNT = 2;
    
        private List<Transform> _waypoints = new();
        private Container _container;
        private List<(Transform, GameTask)> _gameTasks = new();
    
        [Inject]
        public void Construct(List<Transform> waypoints, Container container)
        {
            _container = container;
            _waypoints = waypoints;
        }

        public void GenerateGameTask()
        {
            for (int i = 0; i < _waypoints.Count; i++)
            {
                TagRadiusDetector detector;
                if (!_waypoints[i].gameObject.TryGetComponent(out detector))
                {
                    var spawnPointComponent = _waypoints[i].gameObject.GetComponent<TagRadiusDetector>();
                    UnityEngine.Object.Destroy(spawnPointComponent);
                    continue;
                }
            
                var waypointInfo = detector.Scan();
                List<Transform> enemySpawnerTransforms = new();

                for (int j = 0; j < waypointInfo.Count; j++)
                {
                    string tag = waypointInfo[j].tag;
                    Transform transform = waypointInfo[j].transform;
                    
                    if (tag == "EnemySpawner")
                    {
                        enemySpawnerTransforms.Add(transform);
                    }
                }
            
                if (enemySpawnerTransforms.Count > 0)
                {
                    GameTask task = new GameTask( enemySpawnerTransforms);
                
                    _gameTasks.Add((_waypoints[i],task));
                }

                _container.CacheType(_gameTasks.GetType(), _gameTasks);
                var detectorComponent = _waypoints[i].gameObject.GetComponent<TagRadiusDetector>();
                UnityEngine.Object.Destroy(detectorComponent);
            }
        }

        public int CalculateOptimalEnemyPool()
        {
            int taskCount = _gameTasks.Count;
        
            if (taskCount <= ACTIVE_TASK_COUNT)
            {
                int total = 0;
                for (int i = 0; i < taskCount; i++)
                {
                    total += _gameTasks[i].Item2.EnemyCount; 
                }
                return total;
            }
        
            int maxSum = 0;

            for (int start = 0; start <= taskCount - ACTIVE_TASK_COUNT; start++)
            {
                int windowSum = 0;
                for (int offset = 0; offset < ACTIVE_TASK_COUNT; offset++)
                {
                    int index = start + offset;
                    windowSum += _gameTasks[index].Item2.EnemyCount;  
                }

                if (windowSum > maxSum)
                    maxSum = windowSum;
            }

            return maxSum;
        }
    }
}