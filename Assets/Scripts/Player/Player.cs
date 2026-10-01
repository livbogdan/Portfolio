using System;
using UnityEngine;


[RequireComponent(typeof(PlayerHealth), typeof(PlayerXP))]
public class Player : MonoBehaviour
{
    public static Player instance;
    [Header("Player Stats")]
    [SerializeField] private Collider2D _playerCollider;
    [SerializeField] private MeshFilter _playerMesh;
    [SerializeField] private MeshRenderer _playerMeshRenderer;
    

    private PlayerHealth _playerHealth;
    private PlayerXP _playerXP;
    private PlayerController _playerController;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        _playerHealth = GetComponent<PlayerHealth>();
        _playerXP = GetComponent<PlayerXP>();
        _playerController = GetComponent<PlayerController>();

        CharacterSelectionManager._onCharacterSelected += CharacterSelectedCallback;
    }



    private void OnDestroy() => CharacterSelectionManager._onCharacterSelected -= CharacterSelectedCallback;

    public void TakeDamage(int damage)
    {
        _playerHealth.TakeDamage(damage);
    }

    public Vector2 GetCenter()
    {
        return (Vector2)transform.position + _playerCollider.offset;
    }

    public bool HasLeveledUp()
    {
        return _playerXP.HasLeveledUp();
    }

    private void CharacterSelectedCallback(CharterDataSO charterData)
    {
        if (charterData == null)
        {
            Debug.LogError("CharterData is null");
            return;
        }

        // Set main character mesh
        if (_playerMesh != null && charterData.CharacterMesh != null)
        {
            _playerMesh.sharedMesh = charterData.CharacterMesh.sharedMesh;
        }

        // Set main character material
        if (_playerMeshRenderer != null && charterData.CharacterMaterial != null)
        {
            _playerMeshRenderer.sharedMaterial = charterData.CharacterMaterial;
        }

        //TODO: Change Material Rendering Mode
    }

    /// <summary>
    /// Receive joystick input from VR controllers and apply to player movement
    /// Connect JoystickControl.onJoystickVectorChange event to this method
    /// </summary>
    public void OnJoystickInput(Vector2 joystickDirection)
    {
        if (_playerController != null)
        {
            _playerController.SetExternalMoveInput(joystickDirection);
        }
    }
}
