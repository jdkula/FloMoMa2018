using UnityEngine;

public class Letter : MonoBehaviour
{

	public Art art;
	public char letter;
	public Vector3 offsetVector;

	void Update ()
	{
		art.Locations[letter] = transform.position - offsetVector;
	}
}
