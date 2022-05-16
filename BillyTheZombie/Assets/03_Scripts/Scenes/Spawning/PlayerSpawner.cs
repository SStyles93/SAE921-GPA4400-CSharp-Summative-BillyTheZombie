using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

namespace Managers
{
    public class PlayerSpawner : MonoBehaviour
    {
        [Header("PlayerSpawn")]
        [Tooltip("The player prefab to spawn, leave empty if not a combat scene")]
        [SerializeField] private GameObject _playerPrefab;
        [Tooltip("The player Stats")]
        [SerializeField] private PlayerStatsSO _playerStatsSO;
        [Tooltip("The index of the spawn position at which the player will spawn")]
        [SerializeField] private PlayerSpawnPositionIndexSO _spawnIndexSO;
        [Tooltip("The transform on which to spawn the player")]
        [SerializeField] private Transform[] _playerSpawns;

        //Reference Scripts
        private EnemySpawner _enemySpawner;
        private SceneManagement _sceneManagement;
        
        //Reference GameObjects
        private GameObject _player;

        //Properties
        public GameObject Player { get => _player; set => _player = value; }
        public PlayerSpawnPositionIndexSO SpawnIndexSO { get => _spawnIndexSO; set => _spawnIndexSO = value; }

        private void Awake()
        {
            if (_playerPrefab != null)
            {
                _enemySpawner = GetComponent<EnemySpawner>();
                _sceneManagement = GetComponent<SceneManagement>();
            }
        }

        void Start()
        {

            if (_playerPrefab != null)
            {
                InitPlayer(_playerSpawns[_spawnIndexSO.positionIndex]);
                //Send the player ref to the EnemySpawner
                _enemySpawner.Player = _player;

                //Subscribes the "Pause" Methods to the player Controller
                _player.GetComponent<PlayerController>().GameState += _enemySpawner.PauseEnemies;
                _player.GetComponent<PlayerController>().GameState += _sceneManagement.PauseCanvas;

            }
        }

        /// <summary>
        /// Initializes the player 
        /// </summary>
        /// <param name="playerSpawn">The transform at which the player has to spawn</param>
        private void InitPlayer(Transform playerSpawn)
        {
            _player = Instantiate(_playerPrefab, playerSpawn.position, Quaternion.identity);
            PlayerStats playerStats = _player.GetComponent<PlayerStats>();
            playerStats.SceneManagement = _sceneManagement;
            playerStats.Health = _playerStatsSO.currentHealth;
            _player.GetComponentInChildren<EnemyIndicator>().EnemySpawner = _enemySpawner;
        }

        /// <summary>
        /// Sets the game state to "Play"
        /// </summary>
        public void ResumeGame()
        {
            _player.GetComponent<PlayerController>().GameState(true);
            _player.GetComponent<PlayerController>().play = true;
            Time.timeScale = 1.0f;
        }
    }
}
