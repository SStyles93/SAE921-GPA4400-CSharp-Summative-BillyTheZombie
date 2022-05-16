using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;

namespace Player
{
    public class EnemyIndicator : MonoBehaviour
    {
        [SerializeField] private EnemySpawner _enemySpawner;
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private SpriteRenderer _arrow;

        public EnemySpawner EnemySpawner { get => _enemySpawner; set => _enemySpawner = value; }

        // Update is called once per frame
        void Update()
        {
            if (_enemySpawner == null || _enemySpawner.EnemyTracked.Count == 0)
            {
                _arrow.gameObject.SetActive(false);
                return;
            }
            else
            {
                if (_enemySpawner.EnemyTracked[0] != null)
                {
                    _arrow.gameObject.SetActive(true);
                    Vector3 direction = _playerController.transform.position - _enemySpawner.EnemyTracked[0].transform.position;
                    Quaternion rotation = Quaternion.LookRotation(direction, Vector3.forward);
                    rotation.x = 0f;
                    rotation.y = 0f;
                    transform.rotation = rotation;
                }
            }
        }
    }
}
