using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MutagenPointsUpdate : MonoBehaviour
{
    [SerializeField] private GameStatsSO _gameStats;

    //UI elements
    private Slider _mpSlider;
    private Text _mpText;

    private void Awake()
    {
        _mpSlider = GetComponent<Slider>();
        _mpText = GetComponentInChildren<Text>();
    }

    public void Update()
    {
        _mpSlider.value = _gameStats.mutagenPoints / 1000.0f;
        _mpText.text = $"{Mathf.Round(_gameStats.mutagenPoints)}";
    }

}
