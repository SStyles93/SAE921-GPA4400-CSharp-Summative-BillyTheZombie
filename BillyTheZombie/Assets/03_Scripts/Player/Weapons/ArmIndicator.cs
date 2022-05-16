using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class ArmIndicator : MonoBehaviour
    {
        [SerializeField] private PlayerActions _playerActions;
        [Tooltip("Arrows used to indicate arms positions [0]-Right ||[1]-Left")]
        [SerializeField] private GameObject[] _arrows;

        private void Awake()
        {
            _playerActions = GetComponentInParent<PlayerActions>();
        }

        // Update is called once per frame
        void Update()
        {
            //Disable arrow if arm is not instantiated
            if (_playerActions.CurrentLeftArm == null)
            {
                _arrows[(int)BODYPART.LEFTARM].gameObject.SetActive(false);
            }
            else
            {
                IndicateArm((int)BODYPART.LEFTARM, _playerActions.CurrentLeftArm);
            }
            if (_playerActions.CurrentRightArm == null)
            {
                _arrows[(int)BODYPART.RIGHTARM].gameObject.SetActive(false);
            }
            else
            {
                IndicateArm((int)BODYPART.RIGHTARM, _playerActions.CurrentRightArm);
            }
        }

        /// <summary>
        /// Indicates the direction of a given arm  
        /// </summary>
        /// <param name="arrowSide">Which arrow renderer to activate</param>
        /// <param name="currentArm">The Arm (GameObject) to show</param>
        private void IndicateArm(int arrowSide, GameObject currentArm)
        {
            _arrows[arrowSide].gameObject.SetActive(true);
            Vector3 direction = _playerActions.transform.position - currentArm.transform.position;
            Quaternion rotation = Quaternion.LookRotation(direction, Vector3.forward);
            rotation.x = 0f;
            rotation.y = 0f;
            _arrows[arrowSide].transform.rotation = rotation;
        }
    }

}
