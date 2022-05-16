using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    //Reference Scripts
    [SerializeField] private Enemy.EnemyStats _enemyStats;
    //Reference Components
    [SerializeField] private Image _healthBar;
    //Components
    [SerializeField] private Gradient _gradient; 
    //Variables
    private float _maxHealth;
    private float _currentHealth;
    private float displayTimer = 5.0f;

    private void Awake()
    {
        _enemyStats = GetComponentInParent<Enemy.EnemyStats>();
    }

    void Start()
    {
        _currentHealth = _maxHealth = _enemyStats.Health;
        _healthBar.fillAmount = _enemyStats.Health / _maxHealth;
    }
    void Update()
    {
        _healthBar.fillAmount = _enemyStats.Health / _maxHealth;
        _healthBar.color = _gradient.Evaluate(_enemyStats.Health / _maxHealth);
        if (_enemyStats.Health == _maxHealth)
        {
            _healthBar.gameObject.SetActive(false);
        }
        else if (_enemyStats.Health == _currentHealth)
        {
            _healthBar.gameObject.SetActive(false);
        }
        else if(_enemyStats.Health != _currentHealth)
        {
            if(displayTimer > 0.0f)
            {
                displayTimer -= Time.deltaTime;
            }
            else
            {
                displayTimer = 5.0f;
                _currentHealth = _enemyStats.Health;
            }
            _healthBar.gameObject.SetActive(true);
        }
    }
}
