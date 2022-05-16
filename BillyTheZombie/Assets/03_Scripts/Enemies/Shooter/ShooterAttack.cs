using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class ShooterAttack : MonoBehaviour
    {
        //Reference Scripts
        [SerializeField] private EnemyStats _enemyStats;
        [SerializeField] private EnemyRayCaster _enemyRayCaster;
        
        //Reference Prefab
        [SerializeField] private GameObject _ribPrefab;
        
        //Variables
        [SerializeField] private float _damage = 1.0f;
        [SerializeField] private float _ribSpeed = 0.5f;

        private void Awake()
        {
            _enemyStats = GetComponentInParent<EnemyStats>();
        }
        
        void Start()
        {
            _damage = _enemyStats.Damage;
        }

        public void LaunchRib()
        {
            GameObject currentRib = Instantiate(_ribPrefab, transform.position, Quaternion.identity);
            Rib rib = currentRib.GetComponent<Rib>();
            rib.RibDirection = _enemyRayCaster.Target.position - transform.position;
            rib.Damage = _damage;
            rib.Speed = _ribSpeed;

        }
    }
}
