using UnityEngine;

[RequireComponent(typeof(Light))]
public class ColorRandomizer : MonoBehaviour
{

	public float SMax;
	public float VMax;
	public float SMin;
	public float VMin;
	public float StartOffset;
	public float SecondsPerCycle;

	public float CurrentHue { get; private set; }

	private Light _light;
	
	// Use this for initialization
	void Start ()
	{
		CurrentHue = StartOffset;
		_light = GetComponent<Light>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		CurrentHue += 1f / SecondsPerCycle * Time.deltaTime;
		CurrentHue %= 1f;
		_light.color = Color.HSVToRGB(
			CurrentHue,
			SMax,
			VMax
		);
	}
}
