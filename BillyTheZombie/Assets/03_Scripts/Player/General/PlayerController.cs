using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        //Reference Scripts
        private PlayerInput _playerInput;

        private string _controlScheme;

        //Movement Vectors
        private Vector2 _movement;
        private Vector2 _aim;

        //Action bools
        private bool _head;
        private bool _armR;
        private bool _armL;

        //Game State bools
        public bool play = true;
        public delegate void Pause(bool isActive);
        public Pause GameState;

        //Repeating logic
        private bool _canRepeateActions = false;
        private bool _repeatingHead = false;
        private bool _repeatingArmR = false;
        private bool _repeatingArmL = false;
        private float _repeatingTimer = 0.1f;
        private float _repeatTimerHead;
        private float _repeatTimerArmR;
        private float _repeatTimerArmL;

        //Properties
        public string ControlScheme { get => _controlScheme; set => _controlScheme = value; }
        public Vector2 Movement { get => _movement; set => _movement = value; }
        public Vector2 Aim { get => _aim; set => _aim = value; }
        public bool Head { get => _head; set => _head = value; }
        public bool ArmR { get => _armR; set => _armR = value; }
        public bool ArmL { get => _armL; set => _armL = value; }
        public bool CanRepeateActions { get => _canRepeateActions; set => _canRepeateActions = value; }

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
        }

        private void Update()
        {
            _controlScheme = _playerInput.currentControlScheme;

            if (!_canRepeateActions)
            {
                //Prevents from repeating input HEAD
                if (_repeatingHead)
                {
                    _repeatTimerHead += Time.deltaTime;
                }
                if (_repeatTimerHead >= _repeatingTimer)
                {
                    _head = true;
                    _repeatingHead = false;
                    _repeatTimerHead = 0.0f;
                }
                else
                {
                    _head = false;
                }

                //Prevents from repeating input ArmR
                if (_repeatingArmR)
                {
                    _repeatTimerArmR += Time.deltaTime;
                }
                if (_repeatTimerArmR >= _repeatingTimer)
                {
                    _armR = true;
                    _repeatingArmR = false;
                    _repeatTimerArmR = 0.0f;
                }
                else
                {
                    _armR = false;
                }

                //Prevents from repeating input ArmL
                if (_repeatingArmL)
                {
                    _repeatTimerArmL += Time.deltaTime;
                }
                if (_repeatTimerArmL >= _repeatingTimer)
                {
                    _armL = true;
                    _repeatingArmL = false;
                    _repeatTimerArmL = 0.0f;
                }
                else
                {
                    _armL = false;
                }
            }
        }

        public void OnMove(InputValue value)
        {
            _movement = value.Get<Vector2>();
        }
        public void OnAim(InputValue value)
        {
            switch (_controlScheme)
            {
                case "Keyboard":
                    break;
                case "Gamepad":
                    _aim = value.Get<Vector2>();
                    break;
                default:
                    break;
            }

        }
        public void OnHead(InputValue value)
        {
            if (!_canRepeateActions)
            {
                if (value.isPressed)
                    _repeatingHead = true;

            }
            else
            {
                _head = value.isPressed;
            }
        }
        public void OnArmR(InputValue value)
        {
            if (!_canRepeateActions)
            {
                if (value.isPressed)
                    _repeatingArmR = true;

            }
            else
            {
                _armR = value.isPressed;
            }
        }
        public void OnArmL(InputValue value)
        {
            if (!_canRepeateActions)
            {
                if (value.isPressed)
                    _repeatingArmL = true;

            }
            else
            {
                _armL = value.isPressed;
            }
        }

        public void OnPause(InputValue value)
        {
            play = !play;
            Time.timeScale = play ? 1.0f : 0.0f;
            GameState(play);
        }
    }
}
