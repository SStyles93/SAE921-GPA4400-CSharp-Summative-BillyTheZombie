using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Player;

namespace Managers
{
    public class SceneManagement : MonoBehaviour
    {
        [Header("UI Transition")]
        [Tooltip("The duratrion of a FadeIn transition")]
        [SerializeField] private float _fadeInDuration = 1.0f;
        private float _maxFadeInDuration;

        [Tooltip("The duratrion of a FadeOut transition")]
        [SerializeField] private float _fadeOutDuration = 1.0f;
        private float _maxFadeOutDuration;
        
        [Tooltip("The image to set a Color Fade In/Out on")]
        [SerializeField] private Image _transitionImage;
        [SerializeField] private AudioSource _musicAudioSource;
        [SerializeField] private GameObject _pauseCanvas;

        //Reference GameObjects
        private GameObject _player;

        //Components
        private Color _currentColor;

        //Variables
        private int _sceneIndex;
        private bool _fadeIn;
        private bool _fadeOut;

        //Properties
        public bool FadeIn { get => _fadeIn; set => _fadeIn = value; }
        public bool FadeOut { get => _fadeOut; set => _fadeOut = value; }
        public int SceneIndex { get => _sceneIndex; set => _sceneIndex = value; }
        public GameObject Player { get => _player; set => _player = value; }

        private void Start()
        {
            _maxFadeInDuration = _fadeInDuration;
            _maxFadeOutDuration = _fadeOutDuration;

            if (_transitionImage == null)
            {
                return;
            }
            _currentColor = _transitionImage.color = Color.black;
            _fadeIn = true;
        }

        private void Update()
        {
            if (_transitionImage == null)
            {
                return;
            }

            if (_fadeIn)
            {
                FadeInTransition();
            }
            if (_fadeOut)
            {
                _fadeIn = false;
                FadeOutTransition(_sceneIndex);
            }
        }

        /// <summary>
        /// Fades out and loads the indicated scene
        /// </summary>
        /// <param name="SceneIndex">The scene to load</param>
        private void FadeOutTransition(int SceneIndex)
        {
            //Gets the current color of the transition Image
            _transitionImage.color = _currentColor;
            //Time value
            _fadeOutDuration -= Time.deltaTime;
            float currentValue = _fadeOutDuration / _maxFadeOutDuration;

            if (currentValue > 0.0f)
            {
                //Transition image Lerp to final color
                _currentColor.a = 1 - currentValue;
                _transitionImage.color = _currentColor;
                //Music "Lerp" 
                _musicAudioSource.volume = currentValue;

                //If no player in the scene
                if (_player == null) return;
                _player.GetComponent<PlayerStats>().IsInvicible = true;
            }
            else
            {
                _fadeOut = false;
                ActivateScene(SceneIndex);
            }
        }
        
        /// <summary>
        /// Fades in to a new scene
        /// </summary>
        private void FadeInTransition()
        {
            _transitionImage.color = _currentColor;

            _fadeInDuration -= Time.deltaTime;
            float currentValue = _fadeInDuration / _maxFadeInDuration;

            if (currentValue > 0.0f)
            {

                //Transition image Color Lerp
                _currentColor.a = currentValue;
                _transitionImage.color = _currentColor;
                //Music transition lerp
                _musicAudioSource.volume = 1 - currentValue;
            }
            else
            {
                _fadeIn = false;
            }
        }

        /// <summary>
        /// Activates the wanted scene according to its index
        /// </summary>
        /// <param name="Index">Index of the wanted scene</param>
        public void ActivateScene(int Index)
        {
            _sceneIndex = Index;
            SceneManager.LoadScene(_sceneIndex);
        }
        public void QuitGame()
        {
            if (!_fadeOut)
            {
                Application.Quit();
            }

        }

        /// <summary>
        /// Activates/Disactivates the canvas according to state
        /// </summary>
        /// <param name="state">state of the canvas</param>
        public void PauseCanvas(bool state)
        {
            _pauseCanvas.SetActive(!state);
        }
    }
}
