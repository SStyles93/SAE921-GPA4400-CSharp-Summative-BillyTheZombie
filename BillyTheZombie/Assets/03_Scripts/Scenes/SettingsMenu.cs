using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Managers
{
    public class SettingsMenu : MonoBehaviour
    {
        public AudioMixer audioMixer;
        public Dropdown resolutionDropdown;

        Resolution[] resolutions;

        public void Start()
        {
            resolutions = Screen.resolutions;
            resolutionDropdown.ClearOptions();

            List<string> options = new List<string>();
            int currentResolutionIndex = 0;
            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height;
                options.Add(option);

                if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                }
            }

            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }

        /// <summary>
        /// Sets the resolution of the screen
        /// </summary>
        /// <param name="resolutionIndex">Screen resolution Index</param>
        public void SetResolution(int resolutionIndex)
        {
            Resolution resolution = resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }

        /// <summary>
        /// Sets the volume of the General AudioMixer
        /// </summary>
        /// <param name="volume">volume at which the AudioMixer is set</param>
        public void SetVolume(float volume)
        {
            audioMixer.SetFloat("Volume", volume);

        }

        /// <summary>
        /// Sets the Quality of the game
        /// </summary>
        /// <param name="qualityIndex">Quality Index</param>
        public void SetQuality(int qualityIndex)
        {
            QualitySettings.SetQualityLevel(qualityIndex);
        }

        /// <summary>
        /// Sets the game in full screen or not
        /// </summary>
        /// <param name="isFullScreen">True for full screen</param>
        public void SetFullScreen(bool isFullScreen)
        {
            Screen.fullScreen = isFullScreen;
        }
    }
}
