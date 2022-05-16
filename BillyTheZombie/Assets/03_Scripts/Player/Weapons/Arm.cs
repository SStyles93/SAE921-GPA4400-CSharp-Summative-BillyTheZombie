using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;

namespace Player
{
    public class Arm : MonoBehaviour
    {
        //Reference GameObject
        private GameObject _player;

        //Reference Components
        private Rigidbody2D _rb;
        private ParticleSystem _particleSystem;
        private AudioSource _audioSource;

        //ArmType
        [SerializeField] private BODYPART armSide;
        private enum ARMTYPE
        {
            BASIC,
            BOOMERANG,
            LAWNMOWER,
            EXPLOSIVE
        }
        [SerializeField] private ARMTYPE armType;

        //Arm stats
        [SerializeField] private float _speed = 1.0f;
        [SerializeField] private float _damage = 1.0f;
        [SerializeField] private float _explosiveArmRadius = 0.5f;
        [SerializeField] private float _pushPower = 150.0f;

        //Damage & PickUp logic
        private float _pickUpTimer = 0.5f;
        private bool _checkForPickUp = false;
        private bool _canBePickedUp = false;
        [SerializeField] private float _damageTimer = 0.5f;
        [SerializeField] private bool _startDamageCountDown = false;
        [SerializeField] private bool _canDamage = true;

        //ArmThrow
        private Vector3 _armDirection;
        [Tooltip("Updates the phisical movement of the Arm")]
        private bool _canMove;

        //Vec3 Used to Boomerang
        private Vector3 throwPosition;
        //ParticleSystem
        private bool _particlewasPlayed = false;
        //AudioSource
        private bool _audioSourceWasPlayed = false;

        //Properties
        public float Damage { get => _damage; set => _damage = value; }
        public float PushPower { get => _pushPower; set => _pushPower = value; }
        public Vector3 ArmDirection { get => _armDirection; set => _armDirection = value; }
        public Vector3 ThrowPosition { get => throwPosition; set => throwPosition = value; }
        public GameObject Player { get => _player; set => _player = value; }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }
        private void Start()
        {
            //Subscribe the PauseArm Method to the player controler's GameState
            _player.GetComponent<PlayerController>().GameState += PauseArm;

            switch (armType)
            {
                case ARMTYPE.BASIC:
                    _rb.drag = 3.0f;
                    _rb.sharedMaterial.bounciness = 0.25f;
                    _pickUpTimer = 0.5f;
                    _damageTimer = 0.5f;
                    break;

                case ARMTYPE.BOOMERANG:
                    _rb.drag = 0.0f;
                    _rb.sharedMaterial.bounciness = 1.0f;
                    _pickUpTimer = 0.25f;
                    _damageTimer = 1.0f;
                    break;

                case ARMTYPE.LAWNMOWER:
                    _rb.drag = 0.1f;
                    _rb.sharedMaterial.bounciness = 0.0f;
                    _rb.mass = 10.0f;
                    _pickUpTimer = 2.0f;
                    break;

                case ARMTYPE.EXPLOSIVE:
                    _rb.drag = 10.0f;
                    _rb.sharedMaterial.bounciness = 0.0f;
                    _pickUpTimer = 1.0f;
                    _damageTimer = 0.5f;
                    GetComponent<CircleCollider2D>().radius = _explosiveArmRadius;
                    GetComponent<CircleCollider2D>().enabled = false;
                    Physics2D.IgnoreCollision(
                            transform.GetComponent<CircleCollider2D>(),
                            _player.gameObject.GetComponent<CapsuleCollider2D>());
                    _particleSystem = GetComponent<ParticleSystem>();
                    _particleSystem.Stop();
                    _audioSource = GetComponent<AudioSource>();
                    break;
            }

            _canMove = true;
            _canBePickedUp = false;
        }

        private void Update()
        {
            Move();

            //Lowers the _pickUpTimer until 0 is reached
            if (_pickUpTimer > 0.0f)
            {
                _pickUpTimer -= Time.deltaTime;
            }
            else
            {
                _pickUpTimer = 0.0f;
            }
            //Checks if the arm can be picked up
            if (_checkForPickUp)
            {
                _canBePickedUp = _pickUpTimer <= 0.0f ? true : false;
            }
            
            if (_startDamageCountDown)
            {
                //Checks if the arm can deal damage
                _canDamage = _damageTimer <= 0.0f ? false : true;
                if (_damageTimer > 0.0f)
                {
                    _damageTimer -= Time.deltaTime;
                }
                else
                {
                    _damageTimer = 0.0f;
                }
            }
            
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            switch (armType)
            {
                case ARMTYPE.BOOMERANG:
                    if (collision.gameObject.CompareTag("Player"))
                    {
                        _checkForPickUp = true;
                    }
                    else
                    {
                        _startDamageCountDown = true;

                        //on collision stops the rb from moving
                        _rb.velocity = Vector2.zero;
                        //stops applying force to the object
                        _canMove = false;
                        //Deals damage only once 
                        if (_canDamage)
                        {
                            collision.gameObject.GetComponent<EnemyStats>()?.TakeDamage(_damage);
                        }
                        else
                        {
                            Physics2D.IgnoreCollision(
                                transform.GetComponent<BoxCollider2D>(),
                                collision.gameObject.GetComponent<BoxCollider2D>());
                        }
                        _canBePickedUp = true;
                    }
                    break;

                case ARMTYPE.LAWNMOWER:
                    //Keeps the arm in movement
                    _canMove = true;
                    if (collision.gameObject.CompareTag("Enemy"))
                    {
                        //Deals damage on every collision with an enemy
                        collision.gameObject.GetComponent<EnemyStats>()?.TakeDamage(_damage);
                        collision.gameObject.GetComponent<Rigidbody2D>().velocity = _armDirection * _speed;
                        _canBePickedUp = true;
                    }
                    else
                    {
                        //For other collisions, check if arm can be picked up
                        _canBePickedUp = _pickUpTimer <= 0.0f ? true : false;
                        if (!collision.gameObject.CompareTag("Player"))
                        {
                            //if the object collides with anything except the player, stop it from moving
                            _canMove = false;
                        }
                    }
                    break;

                case ARMTYPE.EXPLOSIVE:
                    if (collision.gameObject.CompareTag("Player"))
                    {
                        //Collision with PLAYER
                        //Starts the timer for the playe to pickup the arm
                        _checkForPickUp = true;
                        if(_canBePickedUp)
                            GetComponent<BoxCollider2D>().isTrigger = true;
                    }
                    else
                    {
                        //Every collision EXCEPT PLAYER
                        //on collision stops the rb from moving
                        _rb.constraints = RigidbodyConstraints2D.FreezeAll;
                        //stops applying force to the object
                        _canMove = false;
                        _checkForPickUp = true;
                        _startDamageCountDown = true;

                        //Collision with ENEMY
                        if (collision.gameObject.CompareTag("Enemy"))
                        {

                            if (_canDamage)
                            {
                                GetComponent<CircleCollider2D>().enabled = true;
                                GetComponent<CircleCollider2D>().isTrigger = false;

                                //Send enemy in opposite direction from player
                                Vector2 forceDirection = collision.gameObject.transform.position -
                                    gameObject.transform.position;
                                collision.gameObject.GetComponent<Rigidbody2D>()?.AddForce(forceDirection * (PushPower * 100f), ForceMode2D.Force);
                                //Damages enemy
                                collision.gameObject.GetComponent<EnemyStats>()?.TakeDamage(_damage);

                                //Plays the particle system
                                if (!_particlewasPlayed)
                                {
                                    _particleSystem.Play();
                                    _particlewasPlayed = true;
                                }
                                if (!_audioSourceWasPlayed)
                                {
                                    _audioSource.Play();
                                    _audioSourceWasPlayed = true;
                                }
                            }
                            else
                            {
                                GetComponent<CircleCollider2D>().enabled = false;
                                Physics2D.IgnoreCollision(
                                    transform.GetComponent<BoxCollider2D>(),
                                    collision.gameObject.GetComponent<BoxCollider2D>());
                            } 
                        }
                    }
                    break;

                default:
                    _checkForPickUp = true;
                    if (!collision.gameObject.CompareTag("Player"))
                    {
                        //stops applying force to the object
                        _canMove = false;
                        if (_canDamage)
                        {
                            collision.gameObject.GetComponent<EnemyStats>()?.TakeDamage(_damage);
                            _canDamage = false;
                        }
                        else
                        {
                            if(collision.gameObject.GetComponent<BoxCollider2D>() != null)
                            Physics2D.IgnoreCollision(
                                transform.GetComponent<BoxCollider2D>(),
                                collision.gameObject.GetComponent<BoxCollider2D>());
                        }
                    }
                    break;
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            if (armType == ARMTYPE.LAWNMOWER)
            {
                collision.gameObject.GetComponent<EnemyStats>()?.TakeDamage(_damage);
                collision.gameObject.GetComponent<Rigidbody2D>().velocity = _armDirection * _speed;
            }
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            //PickUp the Arm
            if (collision.gameObject.CompareTag("Player") && _canBePickedUp)
            {
                collision.gameObject.GetComponent<PlayerActions>()?.EnablePlayersArm(armSide, true);
                Destroy(gameObject);
            }
        }
        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player") && _canBePickedUp)
            {
                collision.gameObject.GetComponent<PlayerActions>()?.EnablePlayersArm(armSide, true);
                Destroy(gameObject);
            }
        }



        /// <summary>
        /// Updates the movement of the Arm
        /// </summary>
        private void Move()
        {
            if (_canMove)
            {
                _rb.constraints = RigidbodyConstraints2D.None;
                //Physic movement
                _rb.velocity = _armDirection * _speed;
                transform.Rotate(Vector3.back * Time.deltaTime * 1000f);
            }
            else
            {
                _rb.velocity = Vector2.zero;
            }
            //Boomerang return
            if (armType == ARMTYPE.BOOMERANG && !_canMove)
            {
                transform.position = Vector3.Lerp(transform.position, throwPosition, _speed / 10.0f * Time.deltaTime);
            }
            //Lawnmower rotation
            if (armType == ARMTYPE.LAWNMOWER)
            {
                transform.Rotate(Vector3.back * Time.deltaTime * 1000f);
            }
            //Explosive constraints
            if (armType == ARMTYPE.EXPLOSIVE && !_canMove)
            {
                _rb.constraints = RigidbodyConstraints2D.FreezeAll;
            }
        }

        /// <summary>
        /// Enables arm movement according to GameState (Play/Pause)
        /// </summary>
        /// <param name="state">The state of the game Play(true)/Pause(false)</param>
        private void PauseArm(bool state)
        {
            _canMove = state;
        }

    }

}
