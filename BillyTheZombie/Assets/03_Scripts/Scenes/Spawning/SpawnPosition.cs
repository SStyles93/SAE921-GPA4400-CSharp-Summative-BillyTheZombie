using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class SpawnPosition : MonoBehaviour
    {
        [SerializeField] float _spawnRange = 1.0f;

        public float SpawnRange { get => _spawnRange; set => _spawnRange = value; }


        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _spawnRange);
        }
    }
}
