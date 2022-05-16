using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BubbleSineWave : MonoBehaviour
{
	[SerializeField] private float period;
	private float alphaFactor;
	[SerializeField] private Color color;

	[SerializeField] private Image _image;

	public void Update()
	{
		SineWaveEffect();
	}

	/// <summary>
	/// SineWave acting on the alpha value of a color
	/// </summary>
	public void SineWaveEffect()
	{
		if (period <= Mathf.Epsilon) return;

		float cycle = Time.time / period;
		const float tau = Mathf.PI * 2.0f;
		float sineWave = Mathf.Sin(cycle * tau);
		alphaFactor = (sineWave + 1.0f) / 2.0f; //SineWave = -1 to 1 // +1 to go from 0 to 2 // divided by 2 for 0 to 1

		color.a = alphaFactor;
		//GetComponent</*ComponentToGet*/>().color = color;
		_image.color = color;
		Vector3 scale = _image.transform.localScale;
		scale.y = alphaFactor;
		_image.transform.localScale = scale;
	}
}
