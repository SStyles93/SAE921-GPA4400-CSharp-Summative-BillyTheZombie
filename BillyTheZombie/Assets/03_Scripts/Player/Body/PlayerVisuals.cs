using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerVisuals : MonoBehaviour
    {
        //Reference Scripts
        private PlayerController _playerController;
        private PlayerMovement _playerMovement;
        private PlayerActions _playerActions;
        private PlayerStats _playerStats;

        //Reference Components
        private Animator _animator;
        //Animator Hashes
        private int _xPositionHash;
        private int _yPositionHash;
        private int _movementHash;
        private int _headbuttHash;

        private bool _updateVisuals = true;

        //Color Change
        [SerializeField] private SpriteRenderer[] _spriteRenders;
        private Color _currentColor;
        private float _damageCooldown = 2f;

        public Animator Animator { get => _animator; private set => _animator = value; }

        private void Awake()
        {
            _playerController = GetComponentInParent<PlayerController>();
            _playerMovement = GetComponentInParent<PlayerMovement>();
            _playerActions = GetComponentInParent<PlayerActions>();
            _playerStats = GetComponentInParent<PlayerStats>();
            _animator = GetComponent<Animator>();

            _xPositionHash = Animator.StringToHash("xPosition");
            _yPositionHash = Animator.StringToHash("yPosition");
            _movementHash = Animator.StringToHash("Movement");
            _headbuttHash = Animator.StringToHash("Headbutt");

            _playerController.GameState += PauseVisuals;

        }
        private void Start()
        {
            _animator.speed = _playerStats.Speed / 4.0f;
        }

        private void Update()
        {
            if (!_updateVisuals) return;
            Look();
            Move();

            RetriveNormalColor();
        }

        /// <summary>
        /// Method used to update Animator Positions
        /// </summary>
        private void Look()
        {
            if (_playerController.Aim != Vector2.zero)
            {
                _animator.SetFloat(_xPositionHash, _playerController.Aim.x);
                _animator.SetFloat(_yPositionHash, _playerController.Aim.y);
            }
            else
            {
                _animator.SetFloat(_xPositionHash, _playerActions.Aim.transform.localPosition.x);
                _animator.SetFloat(_yPositionHash, _playerActions.Aim.transform.localPosition.y);
            }
        }
        /// <summary>
        /// Method used to update Animator Movement
        /// </summary>
        private void Move()
        {
            if (_playerController.Movement != Vector2.zero)
            {
                _animator.SetFloat(_movementHash, 1.0f);
            }
            else
            {
                _animator.SetFloat(_movementHash, 0.0f);
            }
        }

        /// <summary>
        /// Slowly sets the color of SpriteRenderers back to white (normal) color
        /// </summary>
        private void RetriveNormalColor()
        {
            if (_currentColor != Color.white)
            {
                _damageCooldown += Time.deltaTime;
                _currentColor = Color.Lerp(_currentColor, Color.white, _damageCooldown);
            }
            else
            {
                _damageCooldown = 0.0f;
            }
            //Color Update
            foreach (SpriteRenderer renderer in _spriteRenders)
            {
                renderer.color = _currentColor;
            }
        }

        /// <summary>
        /// Pauses the player visuals when the game is not running
        /// </summary>
        /// <param name="isRunning">State in which to place the player visuals</param>
        private void PauseVisuals(bool isRunning)
        {
            _updateVisuals = isRunning;
        }

        /// <summary>
        /// Launch headbutt
        /// </summary>
        public void StartHeadbutt()
        {
            _animator.SetBool(_headbuttHash, true);
            _playerMovement.CanMove = false;

        }
        /// <summary>
        /// Method used to disable Headbutt bool in animator
        /// </summary>
        public void EndHeadbutt()
        {
            _animator.SetBool(_headbuttHash, false);
            _playerMovement.CanMove = true;
        }

        /// <summary>
        /// Visual feedack hits taken
        /// </summary>
        public void HitEffect()
        {
            _currentColor = Color.red;
            _damageCooldown = 0.0f;
        }
    }
}
