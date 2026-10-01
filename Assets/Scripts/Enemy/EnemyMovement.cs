using System;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("References")]
    private Player _player;

    [Header("Preferences")]
    [SerializeField] private float _movementSpeed = 1f;

    private bool _shouldMove = true;

    /* Old Movement Code
    Update is called once per frame
    void Update()
    {
        if (_player != null)
            FollowPlayer();
    }

    public void StorePlayer(Player player)
    {
        this._player = player;
    }

    public void FollowPlayer()
    {
        Vector2 direction = (_player.transform.position - transform.position).normalized;

        Vector2 targetPosition = transform.position + (Vector3)direction * _movementSpeed * Time.deltaTime;

        transform.position = targetPosition;
    }
    */

    public void StorePlayer(Player player)
    {
        this._player = player;
    }

    public void FollowPlayer()
    {
        if(_player == null || !_shouldMove) return;

        _shouldMove = true;
        
        Vector2 direction = (_player.transform.position - transform.position).normalized;
        MoveInDirection(direction);
    }
    
    public void MoveAwayFromPlayer()
    {
        if(_player == null || !_shouldMove) return;
        
        Vector2 direction = (transform.position - _player.transform.position).normalized;
        MoveInDirection(direction);
    }
    
    public void StopMovement()
    {
        _shouldMove = false;
    }
    
    public void ResumeMovement()
    {
        _shouldMove = true;
    }
    
    private void MoveInDirection(Vector2 direction)
    {
        Vector2 targetPosition = transform.position + (Vector3)direction * _movementSpeed * Time.deltaTime;
        transform.position = targetPosition;
    }
}
