using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class RightArmIndicator : MonoBehaviour
    {
        [SerializeField] private PlayerActions _playerActions;
        [SerializeField] private SpriteRenderer _arrow;
        [SerializeField] private SpriteRenderer[] _arrows;
        
        private void Awake()
        {
            _playerActions = GetComponentInParent<PlayerActions>();
            _arrow = GetComponentInChildren<SpriteRenderer>();
        }

        void Update()
        {
            //Disable arrow if arm is not instantiated
            if (_playerActions.CurrentRightArm == null)
            {
                _arrow.gameObject.SetActive(false);
            }
            else
            {
                _arrow.gameObject.SetActive(true);
                Vector3 direction = _playerActions.transform.position - _playerActions.CurrentRightArm.transform.position;
                Quaternion rotation = Quaternion.LookRotation(direction, Vector3.forward);
                rotation.x = 0f;
                rotation.y = 0f;
                transform.rotation = rotation;
            }
        }
    }
}
