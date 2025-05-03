using Infrastructure.StateMachine;
using UnityEngine;

public class LevelReloaderTrigger : MonoBehaviour
{
    private const string PlayerTag = "Player";
    
    private GameManager _gameManager;

    public void Construct(GameManager gameManager)
    {
        _gameManager = gameManager;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PlayerTag))
        {
            _gameManager.ReloadGame();
        }
    }
}
