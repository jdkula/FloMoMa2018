using UnityEngine;

public class Spinner : MonoBehaviour
{

	public float Radius;
	public float SecondsPerRotation;
	public float StartAngle;

	private float _currentAngle;
	private float _initialZ;

	public float GetX()
	{
		return Mathf.Cos(Mathf.Deg2Rad * _currentAngle) * Radius;
	}

	public float GetY()
	{
		return Mathf.Sin(Mathf.Deg2Rad * _currentAngle) * Radius;
	}
	
	public Vector3 GetPosition()
	{
		return new Vector3(GetX(), GetY(), _initialZ);
	}
	
	// Use this for initialization
	void Start ()
	{
		_currentAngle = StartAngle;
		_initialZ = transform.position.z;
	}
	
	// Update is called once per frame
	void Update ()
	{
		_currentAngle += 360f / SecondsPerRotation * Time.deltaTime;
		transform.position = GetPosition();
	}
}
