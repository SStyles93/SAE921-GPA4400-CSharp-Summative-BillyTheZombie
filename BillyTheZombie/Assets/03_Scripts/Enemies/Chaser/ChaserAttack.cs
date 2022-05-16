using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

namespace Enemy
{
    public class ChaserAttack : MonoBehaviour
    {
        [SerializeField] private EnemyStats _enemyStats;

        private void Awake()
        {
            _enemyStats.GetComponentInParent<EnemyStats>();
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                //Uses the Trigger to hit
                collision.GetComponent<PlayerStats>()?.TakeDamage(_enemyStats.Damage);
                collision.gameObject.GetComponent<Rigidbody2D>().AddForce((collision.gameObject.transform.position -
                    gameObject.transform.position) * 100.0f, ForceMode2D.Impulse);
            }
            
        }
    }
}
