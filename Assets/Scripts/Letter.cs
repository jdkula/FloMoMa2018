using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Letter : MonoBehaviour
{

	public Art art;
	public char letter;
	public Vector3 offsetVector;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		art.Locations[letter] = transform.position - offsetVector;
	}
}
