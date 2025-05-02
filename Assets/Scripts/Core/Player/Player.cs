using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Infrastructure.DI.Injector;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    static readonly int IsRun = Animator.StringToHash( "IsRun" );
    
    [SerializeField] private CinemachineFreeLook _freeLockCamera;

    private List<Transform> _waypoints;
    private NavMeshAgent _navMeshAgent;
    private Animator _animator;
    private int _currentIndex;

    private bool _canMove;
    private bool _isMoving = true;
    
    [Inject]
    public void Construct(CinemachineFreeLook virtualCamera, List<Transform> waypoints)
    {
        _freeLockCamera = virtualCamera;
        _freeLockCamera.Follow = transform;
        _freeLockCamera.LookAt = transform;
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _waypoints = waypoints;
        _canMove = true;
        _isMoving = false;
    }
    
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && _canMove && !_isMoving)
        {
            TryMoveToNextWaypoint();
        }

        if (Input.GetKeyDown(KeyCode.G) && !_canMove && !_isMoving)
        {
            _canMove = true;
        }
    }
    
    private void TryMoveToNextWaypoint()
    {
        if (_currentIndex >= _waypoints.Count)
            return;

        _isMoving = true;
        _canMove = false;
        _animator.SetBool( IsRun, true );

        Transform target = _waypoints[_currentIndex];
        _navMeshAgent.SetDestination(target.position);

        StartCoroutine(WaitForArrival());
    }

    private IEnumerator WaitForArrival()
    {
        while (_navMeshAgent.pathPending || _navMeshAgent.remainingDistance > 0.1f)
        {
            yield return null;
        }

        _isMoving = false;
        _animator.SetBool( IsRun, false );
        _currentIndex++;
    }
}
