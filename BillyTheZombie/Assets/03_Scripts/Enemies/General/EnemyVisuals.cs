using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace Enemy
{
    public class EnemyVisuals : MonoBehaviour
    {
        //Reference Scripts
        private AIPath _aIPath;

        //Reference Components
        [SerializeField] private SpriteRenderer[] _spriteRenders;
        [SerializeField] private GameObject _rayCaster;
        private Animator _animator;
        private ParticleSystem _bloodEffect;

        //Animator Hashes
        private int _xPositionHash;
        private int _yPositionHash;
        private int _movementHash;
        private int _attackHash;

        //Variables
        private Color _currentColor;
        private float _damageCooldown = 2f;

        private bool _attack = false;

        public bool Attack { get => _attack; set => _attack = value; }

        private void Awake()
        {
            _aIPath = GetComponentInParent<AIPath>();

            _animator = GetComponent<Animator>();
            _bloodEffect = GetComponent<ParticleSystem>();

            _xPositionHash = Animator.StringToHash("xPosition");
            _yPositionHash = Animator.StringToHash("yPosition");
            _movementHash = Animator.StringToHash("Movement");
            _attackHash = Animator.StringToHash("Attack");
        }

        void Update()
        {
            _animator.SetFloat(_xPositionHash, _rayCaster.transform.rotation.y);
            if (_aIPath.canMove)
            {
                if (_aIPath.reachedEndOfPath)
                {
                    _animator.SetFloat(_movementHash, 0.0f);
                    return;
                }
                _animator.SetFloat(_movementHash, 1.0f);
            }
            else
            {
                _animator.SetFloat(_movementHash, 0.0f);
            }

            if (_attack)
            {
                _animator.SetBool(_attackHash, true);
            }
            else
            {
                _animator.SetBool(_attackHash, false);
            }

            RetriveNormalColor();

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
            foreach (SpriteRenderer renderer in _spriteRenders)
            {
                renderer.color = _currentColor;
            }

        }

        /// <summary>
        /// Visual feedack for when an enemy gets hit
        /// </summary>
        public void HitEffect()
        {
            _currentColor = Color.red;
            _damageCooldown = 0.0f;

            if (!_bloodEffect.isPlaying)
            {
                _bloodEffect.Play();
            }
        }

        /// <summary>
        /// HasAttacked is used by the animator to signal the end of attack
        /// </summary>
        public void HasAttacked()
        {
            _attack = false;
        }
    }
}
