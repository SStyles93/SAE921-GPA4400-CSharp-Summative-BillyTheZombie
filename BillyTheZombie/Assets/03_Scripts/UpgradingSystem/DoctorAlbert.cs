using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Player;

public class DoctorAlbert : Interactable
{
    //Reference GameObjects
    [Header("UI Objects")]
    [SerializeField] private Canvas _canvas;
    [SerializeField] private GameObject[] _sliders;
    [SerializeField] private GameObject[] _hightLights;
    [SerializeField] private GameObject[] _rightArmButtons;
    [SerializeField] private bool[] _rightArmBools = new bool[4];
    [SerializeField] private GameObject[] _leftArmButtons;
    [SerializeField] private bool[] _leftArmBools = new bool[4];

    //Reference Components
    [Header("Components")]
    [SerializeField] private EventSystem _eventSystem;
    [SerializeField] private AudioSource _audioSource;

    [Header("Audio")]
    [SerializeField] private AudioClip _albertSound;

    //Reference ScriptableObjects
    [Header("Scriptable Objects")]
    [SerializeField] private GameStatsSO _gameStatsSO;
    [SerializeField] private PlayerStatsSO _playerStatsSO;

    //Variables
    [Header("Variables")]
    [Tooltip("The coefficient of MutagenPoints, 1 => 1pt/%")]
    [SerializeField] private float _pointsCoef = 1.0f;
    [SerializeField] private float _firstArmLimit = 33.0f;
    private bool _activateFirst = false;
    [SerializeField] private float _secondArmLimit = 66.0f;
    private bool _activateSecond = false;
    [SerializeField] private float _thirdArmLimit = 99.0f;
    private bool _activateThird = false;

    public void Start()
    {
        _canvas.gameObject.SetActive(false);

        SetLeftArmPower(_playerStatsSO.leftArmType);
        SetRightArmPower(_playerStatsSO.rightArmType);

        foreach (GameObject slider in _sliders)
        {
            slider.GetComponentInChildren<Text>().text =
                $"{slider.name}";
        }
    }
    private void Update()
    {
        if (player != null)
        {
            UpdateUIButton();

            //Interact
            if (player.GetComponent<PlayerController>().Head && !hasInteracted)
            {
                Act();
                hasInteracted = true;
            }
        }
        if (_eventSystem.currentSelectedGameObject != null)
        {
            for (int i = 0; i < _sliders.Length; i++)
            {
                if (_eventSystem.currentSelectedGameObject == _sliders[i])
                {
                    HighlightSlider(_hightLights[i], true);
                    if (player.GetComponent<PlayerController>().ArmR || player.GetComponent<PlayerController>().Movement.x > 0.25f)
                    {
                        AddPoints(_sliders[i]);
                    }
                    else if(player.GetComponent<PlayerController>().ArmL || player.GetComponent<PlayerController>().Movement.x < -0.25f)
                    {
                        SubstractPoints(_sliders[i]);
                    }
                }
                else
                {
                    HighlightSlider(_hightLights[i], false);
                }
            }
        }
        UpdateSliders();
        UpdateArmButtons();
        SetRightArmPower(_playerStatsSO.rightArmType);
        SetLeftArmPower(_playerStatsSO.leftArmType);
    }

    /// <summary>
    /// Highlights the selected "body part" in UI
    /// </summary>
    /// <param name="highlightedImage"></param>
    /// <param name="enable"></param>
    public void HighlightSlider(GameObject highlightedImage, bool enable)
    {
        highlightedImage.SetActive(enable);
    }

    /// <summary>
    /// Stops the player from acting/moving and enables UI Navigation
    /// </summary>
    public override void Act()
    {
        _canvas.gameObject.SetActive(true);

        //Disable Player
        player.GetComponent<PlayerMovement>().CanMove = false;
        player.GetComponent<PlayerActions>().enabled = false;
        player.GetComponentInChildren<PlayerVisuals>().enabled = false;

        //Disables the "InfoBubble"
        _infoBubble.SetActive(false);

        //TODO : UI Nav
        _eventSystem.SetSelectedGameObject(_sliders[0]);

        _audioSource.clip = _albertSound;
        if(!_audioSource.isPlaying)
        _audioSource.Play();

    }

    /// <summary>
    /// Enables the players Actions/Movement
    /// </summary>
    public override void StopActing()
    {
        _canvas.gameObject.SetActive(false);

        //Enable Player
        player.GetComponent<PlayerMovement>().CanMove = true;
        player.GetComponent<PlayerActions>().enabled = true;
        player.GetComponentInChildren<PlayerVisuals>().enabled = true;
        
        //Stops the UI Navigation
        _eventSystem.SetSelectedGameObject(null);
    }

    /// <summary>
    /// Updates the text contained in the Sliders
    /// </summary>
    private void UpdateSliders()
    {
        foreach (GameObject slider in _sliders)
        {
            slider.GetComponentInChildren<Text>().text =
                $"{Mathf.Round(slider.GetComponent<Slider>().value * 100.0f)} %\n {slider.name}";

            slider.GetComponent<PlayerStatUpdate>()?.UpdateSlider();
        }
    }
    /// <summary>
    /// Adds MutagenPoints to a slider
    /// </summary>
    /// <param name="slider">The slider to add points to</param>
    private void AddPoints(GameObject slider)
    {
        if (_gameStatsSO.mutagenPoints > 0.0f 
            && slider.GetComponent<Slider>().value < 1.0f)
        {
            _gameStatsSO.mutagenPoints -= 0.1f * _pointsCoef;
            slider.GetComponent<PlayerStatUpdate>().Stat += 0.1f; 
            slider.GetComponent<Slider>().value += 0.1f/ 100.0f;
        }
    }

    /// <summary>
    /// Substracts MutagenPoints to a slider
    /// </summary>
    /// <param name="slider"></param>
    private void SubstractPoints(GameObject slider)
    {
        if (slider.GetComponent<Slider>().value > 0.0f)
        {
            _gameStatsSO.mutagenPoints += 0.1f * _pointsCoef;
            slider.GetComponent<PlayerStatUpdate>().Stat -= 0.1f;
            slider.GetComponent<Slider>().value -= 0.1f/ 100.0f;
        }
    }

    /// <summary>
    /// Activates or disactivates the button UIs
    /// </summary>
    private void UpdateArmButtons()
    {
        //Activates the First Arm
        _activateFirst = (_sliders[1].GetComponent<Slider>().value * 100) >= _firstArmLimit;
        _rightArmBools[1] = _activateFirst;
        _leftArmBools[1] = _activateFirst;
        _rightArmButtons[0].GetComponentInChildren<Text>().color = _activateFirst ? Color.white : Color.gray;
        _leftArmButtons[0].GetComponentInChildren<Text>().color = _activateFirst ? Color.white : Color.gray;
        _rightArmButtons[0].GetComponent<Image>().color = _activateFirst ? Color.white : Color.gray;
        _leftArmButtons[0].GetComponent<Image>().color = _activateFirst ? Color.white : Color.gray;

        //Activates the Second Arm
        _activateSecond = (_sliders[1].GetComponent<Slider>().value * 100) >= _secondArmLimit;
        _rightArmBools[2] = _activateSecond;
        _leftArmBools[2] = _activateSecond;
        _rightArmButtons[1].GetComponentInChildren<Text>().color = _activateSecond ? Color.white : Color.gray;
        _leftArmButtons[1].GetComponentInChildren<Text>().color = _activateSecond ? Color.white : Color.gray;
        _rightArmButtons[1].GetComponent<Image>().color = _activateSecond ? Color.white : Color.gray;
        _leftArmButtons[1].GetComponent<Image>().color = _activateSecond ? Color.white : Color.gray;

        //Activates the Third Arm
        _activateThird = (_sliders[1].GetComponent<Slider>().value * 100) >= _thirdArmLimit;
        _rightArmBools[3] = _activateThird;
        _leftArmBools[3] = _activateThird;
        _rightArmButtons[2].GetComponentInChildren<Text>().color = _activateThird ? Color.white : Color.gray;
        _leftArmButtons[2].GetComponentInChildren<Text>().color = _activateThird ? Color.white : Color.gray;
        _rightArmButtons[2].GetComponent<Image>().color = _activateThird ? Color.white : Color.gray;
        _leftArmButtons[2].GetComponent<Image>().color = _activateThird ? Color.white : Color.gray;
    }

    /// <summary>
    /// Sets the right arm power
    /// </summary>
    /// <param name="armIndex">Index of the desired arm power</param>
    public void SetRightArmPower(int armIndex)
    {
        if (_rightArmBools[armIndex] == false) return;
        _playerStatsSO.rightArmType = armIndex;
        if (armIndex <= 0) return;
        

        foreach(GameObject button in _rightArmButtons)
        {
            button.GetComponent<Image>().color = Color.gray;
        }
        
        _rightArmButtons[armIndex-1].GetComponent<Image>().color = Color.green;
    }

    /// <summary>
    /// Sets the left arm power
    /// </summary>
    /// <param name="armIndex">Index of the desired arm power</param>
    public void SetLeftArmPower(int armIndex)
    {
        if (_leftArmBools[armIndex] == false) return;
        _playerStatsSO.leftArmType = armIndex;
        if (armIndex <= 0) return;
        
        foreach (GameObject button in _leftArmButtons)
        {
            button.GetComponent<Image>().color = Color.gray;
        }
        _leftArmButtons[armIndex-1].GetComponent<Image>().color = Color.green;
    }

    /// <summary>
    /// Reset all slider values & ArmPower index
    /// </summary>
    public void ResetPoints()
    {
        for (int i = 0; i < _sliders.Length; i++)
        {
            _gameStatsSO.mutagenPoints += _sliders[i].GetComponent<Slider>().value * (100.0f * _pointsCoef);
            _sliders[i].GetComponent<PlayerStatUpdate>().Stat = 0.0f;
            _sliders[i].GetComponent<Slider>().value = 0.0f;
            _playerStatsSO.rightArmType = 0;
            _playerStatsSO.leftArmType = 0;
        }
    }

    /// <summary>
    /// Saves the state of the StatSliders & ArmPower in the _playerStats ScriptableObject
    /// </summary>
    public void SavePoints()
    {
        for (int i = 0; i < _sliders.Length; i++)
        {
            _sliders[i].GetComponent<PlayerStatUpdate>()?.UpdateStat();
        }
        _playerStatsSO.maxHealth = _playerStatsSO.basicHealth + (_playerStatsSO.basicHealth * _playerStatsSO.healthPercentage / 20.0f);
        _playerStatsSO.currentHealth = _playerStatsSO.maxHealth;
        if(_playerStatsSO.armDamagePercentage <= _firstArmLimit)
        {
            _playerStatsSO.rightArmType = 0;
            _playerStatsSO.leftArmType = 0;
        }
    }

}
