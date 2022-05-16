using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class PlayerUI : MonoBehaviour
    {
        //Reference Scripts
        [Header("Player Scripts")]
        //[SerializeField] private PlayerInput _playerInput;
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private PlayerActions _playerActions;
        [SerializeField] private PlayerStats _playerStats;
        [SerializeField] private EnemyIndicator _playerSpawnIndicator;

        //Reference Components
        [Header("Player Health UI")]
        [SerializeField] private Slider _healthSlider;
        [SerializeField] private Gradient _healthGradient;
        [SerializeField] private Image _healthFill;
        [SerializeField] private Image _headImage;
        [SerializeField] private List<Sprite> _headSprites;

        [Tooltip("Game Stats UI")]
        [SerializeField] private Text _waveNumberText;
        [SerializeField] private Text _mutagenPointsText;
        private Color _currentColor;
        private float _colorCooldown;
        [SerializeField] private ParticleSystem _particleSystem;


        [Header("Player Actions UI")]
        [SerializeField] private Color _blockedColor = Color.gray;
        private Color _normalColor = Color.white;
        [Tooltip("List of ArmImages: [0]-Right || [1]-Left || [2]-Head")]
        [SerializeField] private Image[] _bodyImages;
        [Tooltip("List of ButtonImages: [0]-Right || [1]-Left || [2]-Head")]
        [SerializeField] private Image[] _buttonImages;

        //ScriptableObjects
        [SerializeField] private UIButtonsSO _uIButtonsSO;
        [SerializeField] private UIActionsSO _uIActionsSO;
        [SerializeField] private PlayerStatsSO _playerStatsSO;
        [SerializeField] private GameStatsSO _gameStatsSO;

        public EnemyIndicator PlayerSpawnIndicator { get => _playerSpawnIndicator; private set => _playerSpawnIndicator = value; }

        private void Awake()
        {
            _playerStats = GetComponentInParent<PlayerStats>();
            _playerActions = GetComponentInParent<PlayerActions>();
            _playerController = GetComponentInParent<PlayerController>();
            _playerSpawnIndicator = GetComponentInChildren<EnemyIndicator>();

            _healthSlider = GetComponentInChildren<Slider>();
        }
        private void Start()
        {
            _healthSlider.maxValue = _playerStats.MaxHealth;
            _healthSlider.value = _playerStats.Health;
            //Set Arm UIActions sprites 0-Right || 1- Left
            _bodyImages[0].sprite = _uIActionsSO.actionSprites[_playerStatsSO.rightArmType];
            _bodyImages[1].sprite = _uIActionsSO.actionSprites[_playerStatsSO.leftArmType];
        }

        // Update is called once per frame
        void Update()
        {
            UpdateHealthBar();
            UpdateActionsUI();
            UpdateButtonUi();
            UpdateButtonsUiColor();
            UpdateTextUi();
        }

        /// <summary>
        /// Updates the player's healthbar UI
        /// </summary>
        private void UpdateHealthBar()
        {
            _healthSlider.value = _playerStats.Health;
            _healthFill.color = _healthGradient.Evaluate(_healthSlider.value / _healthSlider.maxValue);
            if (_playerStats.Health <= (_playerStats.MaxHealth * 0.5f))
            {
                _headImage.sprite = _headSprites[1];
            }
            else
            {
                _headImage.sprite = _headSprites[0];
            }
        }

        /// <summary>
        /// Updates the player's actions UI according to PlayerActions
        /// </summary>
        private void UpdateActionsUI()
        {
            //If CanThrow => normalColor

            //Right arm
            _bodyImages[(int)BODYPART.RIGHTARM].color =
                _playerActions.CanThrow[(int)BODYPART.RIGHTARM] ? _normalColor : _blockedColor;

            //Left arm
            _bodyImages[(int)BODYPART.LEFTARM].color =
                _playerActions.CanThrow[(int)BODYPART.LEFTARM] ? _normalColor : _blockedColor;

            //Head
            _bodyImages[2].color =
                _playerActions.CanHeadbutt ? _normalColor : _blockedColor;
        }

        /// <summary>
        /// Updated the button UI according to PlayerController
        /// </summary>
        private void UpdateButtonsUiColor()
        {
            // If button pressed => blockedColor

            //Right arm trigger
            _buttonImages[(int)BODYPART.RIGHTARM].color =
                _playerController.ArmR ? _blockedColor : _normalColor;

            //Left arm trigger
            _buttonImages[(int)BODYPART.LEFTARM].color =
                _playerController.ArmL ? _blockedColor : _normalColor;

            //Head button
            _buttonImages[2].color =
                _playerController.Head ? _blockedColor : _normalColor;
        }

        /// <summary>
        /// Updated the buttons UI to match the Controllers (ControlScheme)
        /// </summary>
        private void UpdateButtonUi()
        {
            switch (_playerController.ControlScheme)
            {
                case "Keyboard":
                    for (int i = 0; i < _buttonImages.Length; i++)
                    {
                        _buttonImages[i].sprite = _uIButtonsSO.keyboardSprites[i];
                    }
                    break;
                case "Gamepad":
                    for (int i = 0; i < _buttonImages.Length; i++)
                    {
                        _buttonImages[i].sprite = _uIButtonsSO.gamepadSprites[i];
                    }
                    break;
                default:
                    break;
            }

        }

        private void UpdateTextUi()
        {
            _waveNumberText.text = $"Current Wave: {_gameStatsSO.currentWaveCount} \n Max Wave: {_gameStatsSO.maxReachedWaveCount}";
            _mutagenPointsText.text = "Mutagen Points";
            _currentColor = _mutagenPointsText.color;
            if (_currentColor != Color.green)
            {
                _colorCooldown += Time.deltaTime / 50.0f;
                _currentColor = Color.Lerp(_currentColor, Color.green, _colorCooldown);

            }
            else
            {
                _colorCooldown = 0.0f;

            }
            _mutagenPointsText.color = _currentColor;
        }

        public void GainPoints()
        {
            _mutagenPointsText.color = Color.red;
            _particleSystem.Play();

        }

    }
}
