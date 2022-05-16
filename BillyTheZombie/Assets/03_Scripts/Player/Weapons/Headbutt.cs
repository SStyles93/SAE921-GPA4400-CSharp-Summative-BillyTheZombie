using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;

namespace Player
{
    public class Headbutt : MonoBehaviour
    {
        [Tooltip("HeadDamage should NOT be changed (5.0f)")]
        [SerializeField] private float _headDamage = 5.0f;
        
        private float _pushPower;

        public float PushPower { get => _pushPower; set => _pushPower = value; }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.GetComponent<EnemyStats>())
            {
                //Send enemy in opposite direction from player
                Vector2 forceDirection = collision.gameObject.transform.position -
                    gameObject.transform.position;
                collision.gameObject.GetComponent<Rigidbody2D>().AddForce(forceDirection * (_pushPower * 100.0f), ForceMode2D.Impulse);

                collision.gameObject.GetComponent<EnemyStats>().TakeDamage(_headDamage);
            }
        }
    }
}
