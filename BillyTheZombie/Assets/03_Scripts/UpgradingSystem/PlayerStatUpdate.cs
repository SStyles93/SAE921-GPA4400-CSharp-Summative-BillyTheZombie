using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class PlayerStatUpdate : MonoBehaviour, IMoveHandler, IEndDragHandler
{
    [SerializeField] private PlayerStatsSO _playerStats;

    private Slider _slider;

    private enum STATTYPE
    {
        PUSHPOWER,
        ARMDAMAGE,
        HEALTH,
        SPEED
    }
    [SerializeField] private STATTYPE statType;

    [SerializeField] private float stat;

    public float Stat { get => stat; set => stat = value; }

    public void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    public void Start()
    {
        switch (statType)
        {
            case STATTYPE.PUSHPOWER:
                stat = _playerStats.pushPowerPercentage;
                break;
            case STATTYPE.ARMDAMAGE:
                stat = _playerStats.armDamagePercentage;
                break;
            case STATTYPE.HEALTH:
                stat = _playerStats.healthPercentage;
                break;
            case STATTYPE.SPEED:
                stat = _playerStats.speedPercentage;
                break;
        }
    }

    /// <summary>
    /// Updates the playerStats according the the sliders value 
    /// </summary>
    public void UpdateStat()
    {
        switch (statType)
        {
            case STATTYPE.PUSHPOWER:
                _playerStats.pushPowerPercentage = stat;
                if (_playerStats.pushPowerPercentage < 0.0f)
                {
                    _playerStats.pushPowerPercentage = 0.0f;
                }
                break;
            case STATTYPE.ARMDAMAGE:
                _playerStats.armDamagePercentage = stat;
                if (_playerStats.armDamagePercentage < 0.0f)
                {
                    _playerStats.armDamagePercentage = 0.0f;
                }
                break;
            case STATTYPE.HEALTH:
                _playerStats.healthPercentage = stat;
                if (_playerStats.healthPercentage < 0.0f)
                {
                    _playerStats.healthPercentage = 0.0f;
                }
                break;
            case STATTYPE.SPEED:
                _playerStats.speedPercentage = stat;
                if (_playerStats.speedPercentage < 0.0f)
                {
                    _playerStats.speedPercentage = 0.0f;
                }
                break;
        }
    }

    /// <summary>
    /// Updates the sliders according to the stat value
    /// </summary>
    public void UpdateSlider()
    {
        _slider.value = stat /100.0f;
    }

    /// <summary>
    /// Sets the behaviour of the slider OnMove
    /// </summary>
    /// <param name="eventData">The movement event</param>
    public void OnMove(AxisEventData eventData)
    {
        // override the slider value using our previousSliderValue and the desired step
        if (eventData.moveDir == MoveDirection.Left)
        {
            _slider.value = stat / 100.0f;
        }

        if (eventData.moveDir == MoveDirection.Right)
        {
            _slider.value = stat / 100.0f;
        }

        //// keep the slider value for future use
        //previousSliderValue = _slider.value;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //// keep the last slider value if the slider was dragged by mouse
        //previousSliderValue = _slider.value;
    }
}