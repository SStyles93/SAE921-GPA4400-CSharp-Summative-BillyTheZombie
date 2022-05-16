using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Managers
{
    public class MenuManager : MonoBehaviour
    {
        private PlayerInput _playerInput;
        private Animator _animator;
        private bool _passIntro;

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            _animator = GetComponent<Animator>();
        }
        private void Update()
        {
            if (_passIntro)
            {
                _animator.speed = 1000.0f;
            }
        }
        public void OnHead(InputValue value)
        {
            _passIntro = value.isPressed;
        }
        public void OnArmR(InputValue value)
        {
            _passIntro = value.isPressed;
        }
        public void OnArmL(InputValue value)
        {
            _passIntro = value.isPressed;
        }
    }
}
