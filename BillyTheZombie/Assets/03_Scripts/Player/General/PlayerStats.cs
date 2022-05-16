using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;

namespace Player
{
    public class PlayerStats : MonoBehaviour
    {
        //Reference Scripts
        private PlayerController _playerController;
        private PlayerMovement _playerMovement;
        private PlayerActions _playerActions;
        private PlayerAudio _playerAudio;
        private PlayerVisuals _playerVisuals;
        
        //Reference GameObjects
        [SerializeField] private SceneManagement _sceneManagement;

        //statSO contains all the player stats
        [Header("Player Stats ScriptableObject")]
        [SerializeField] private PlayerStatsSO _statSO;
        [SerializeField] private GameStatsSO _gameStatsSO;

        [Header("Player's Stats")]
        [SerializeField] private float _pushPower = 10.0f;
        [SerializeField] private float _armDamage = 10.0f;
        [SerializeField] private float _health = 100.0f;
        [SerializeField] private float _maxHealth = 100.0f;
        [SerializeField] private float _speed = 2.0f;

        [SerializeField] private bool _isInvicible = false;

        //Properties
        public SceneManagement SceneManagement { get => _sceneManagement; set => _sceneManagement = value; }
        public float Health { get => _health; set => _health = value; }
        public float MaxHealth { get => _maxHealth; set => _maxHealth = value; }
        public float Speed { get => _speed; set => _speed = value; }
        public float PushPower { get => _pushPower; set => _pushPower = value; }
        public float ArmDamage { get => _armDamage; set => _armDamage = value; }
        public bool IsInvicible { get => _isInvicible; set => _isInvicible = value; }

        private void Awake()
        {
            _pushPower = _statSO.basicPushPower + (_statSO.basicPushPower * _statSO.pushPowerPercentage / 100.0f);
            _armDamage = _armDamage + (_statSO.basicArmDamage * _statSO.armDamagePercentage / 100.0f);
            _maxHealth = _statSO.basicHealth + (_statSO.basicHealth * _statSO.healthPercentage / 20.0f);
            _speed = _statSO.basicSpeed + (_statSO.basicSpeed * _statSO.speedPercentage / 100.0f);

            //Get player components
            _playerController = GetComponent<PlayerController>();
            _playerMovement = GetComponent<PlayerMovement>();
            _playerActions = GetComponent<PlayerActions>();
            _playerAudio = GetComponent<PlayerAudio>();
            //Get player's body components
            _playerVisuals = GetComponentInChildren<PlayerVisuals>();

            _playerController.GameState += PauseStats;

        }

        private void Update()
        {
            _health = _statSO.currentHealth;

            if (_statSO.currentHealth <= 0.0f)
            {
                Die();
            }
        }

        /// <summary>
        /// Applies death to the player
        /// </summary>
        private void Die()
        {
            DisablePlayersActions();
            ResetWaves();
            //Set and play SceneManager's FadeOut
            _sceneManagement.Player = gameObject;
            _sceneManagement.SceneIndex = 1;
            _sceneManagement.FadeOut = true;
            _sceneManagement.GetComponent<PlayerSpawner>().SpawnIndexSO.positionIndex = 0;
            ResetLife();
        }

        /// <summary>
        /// Pauses the player Stats when the game is not running
        /// </summary>
        /// <param name="state">State of game Play(true)/Pause(false)</param>
        private void PauseStats(bool state)
        {
            _isInvicible = !state;
            _playerActions.enabled = state;
        }

        /// <summary>
        /// Resets the Waves to zero
        /// </summary>
        private void ResetWaves()
        {
            _gameStatsSO.currentWaveCount = 0;
            _gameStatsSO.indexOfWaveToSpawn = 0;
        }

        /// <summary>
        /// Lowers health according to the damage
        /// </summary>
        /// <param name="damage">The damage to substract to health</param>
        public void TakeDamage(float damage)
        {
            if (!_isInvicible)
                _statSO.currentHealth -= damage;
            _playerVisuals.HitEffect();
            _playerAudio.PlayHit();
        }

        /// <summary>
        /// Disables all actions from player and makes him invulerable
        /// </summary>
        public void DisablePlayersActions()
        {
            IsInvicible = true;
            _playerActions.CanHit = false;
            _playerMovement.CanMove = false;
            _playerVisuals.Animator.speed = 0;
        }

        /// <summary>
        /// Resets the player's health
        /// </summary>
        public void ResetLife()
        {
            //Reset player's health
            _statSO.currentHealth = _statSO.maxHealth;
        }
    }
}
