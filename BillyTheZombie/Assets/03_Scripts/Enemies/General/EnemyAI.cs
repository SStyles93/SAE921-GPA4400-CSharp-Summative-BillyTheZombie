using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace Enemy
{
    public class EnemyAI : MonoBehaviour
    {
        //Reference Scripts
        private AIPath _aIPath;
        private EnemyRayCaster _rayCaster;
        private AIDestinationSetter _destinationSetter;
        private EnemyStats _enemyStats;
        private EnemyVisuals _enemyVisuals;

        //Variables
        private float recoveryTimer = 0.5f;
        private float recoveryTime;
        private float attackTime;
        private bool _stopMoving = false;

        private float _detectionRadius = 0.0f;

        //Reference Component
        private CircleCollider2D _colliderTrigger;
        private BoxCollider2D _boxCollider;

        private void Awake()
        {
            _aIPath = GetComponent<AIPath>();
            _rayCaster = GetComponentInChildren<EnemyRayCaster>();
            _destinationSetter = GetComponent<AIDestinationSetter>();
            _enemyStats = GetComponent<EnemyStats>();
            _enemyVisuals = GetComponentInChildren<EnemyVisuals>();

            _colliderTrigger = GetComponent<CircleCollider2D>();
            _boxCollider = GetComponent<BoxCollider2D>();
        }
        private void Start()
        {
            recoveryTime = recoveryTimer;
            attackTime = _enemyStats.AttackFrequency;

            _aIPath.maxSpeed = _enemyStats.Speed;
            _destinationSetter.target = _rayCaster.Target;

            _detectionRadius = GetComponent<CircleCollider2D>().radius * 3f;
            _colliderTrigger.enabled = false;
        }

        private void Update()
        {
            if (_stopMoving == true)
            {
                _aIPath.canMove = false;
                recoveryTime -= Time.deltaTime;
            }
            if (recoveryTime <= 0.0f)
            {
                recoveryTime = recoveryTimer;
                _aIPath.canMove = true;
                _stopMoving = false;
            }

            float targetDistance = (_destinationSetter.target.position - transform.position).magnitude;
            if (_rayCaster.PlayerInSight)
            {
                _colliderTrigger.enabled = targetDistance < _detectionRadius ? true : false;
            }
            else
            {
                _colliderTrigger.enabled = false;
            }
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Arm") || collision.gameObject.CompareTag("Player"))
            {
                _stopMoving = true;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                if (_rayCaster.PlayerInSight)
                {
                    _aIPath.canMove = false;
                }
            }
        }
        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                if (_rayCaster.PlayerInSight)
                {
                    _aIPath.canMove = false;
                    attackTime -= Time.deltaTime;
                }
                else
                {
                    _aIPath.canMove = true;
                }
                if (attackTime < 0.0f)
                {
                    attackTime = _enemyStats.AttackFrequency;
                    //Launches the Attack of the enemy
                    _enemyVisuals.Attack = true;
                }

            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                _aIPath.canMove = true;
                _enemyVisuals.Attack = false;
            }

        }

        public void PauseEnemy(bool isRunning)
        {
            GetComponentInChildren<Animator>().enabled = isRunning;
            _aIPath.enabled = isRunning;
            _rayCaster.enabled = isRunning;
            _destinationSetter.enabled = isRunning;
            _enemyStats.enabled = isRunning;
            _enemyVisuals.enabled = isRunning;
        }
    }
}
