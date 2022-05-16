using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemyAudio : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip[] _enemyHit;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void HitEffect()
        {
            if (_enemyHit.Length == 0) return;

            int randomIndex = Random.Range(0, _enemyHit.Length);
            _audioSource.clip = _enemyHit[randomIndex];
            if (!_audioSource.isPlaying)
                _audioSource.Play();
        }
    }
}
