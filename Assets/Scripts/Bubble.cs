﻿using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

/// <summary>
/// Represents a little randomly-floating bubble
/// </summary>
public class Bubble : MonoBehaviour
{

	public float zOffset;
	public Sprite popSprite;
	
	private Vector3 _destinationPosition;
	private Vector3 _destinationVelocity;
	private Vector3 _destinationAcceleration;

	private Vector3 _bubbleVelocity;
	private Vector3 _bubbleAcceleration;

	private float _endAlpha;
	private float _percentAlpha;
	private const float TimeToFull = 5;

	void DoAccelStep(float deltaTime)
	{
		_bubbleVelocity += _bubbleAcceleration * deltaTime;
		_destinationVelocity += _destinationAcceleration * deltaTime;
	}

	void DoVelocityStep(float deltaTime)
	{
		_destinationPosition += _destinationVelocity * deltaTime + 0.5f * deltaTime * deltaTime * _destinationAcceleration;
		transform.position += _bubbleVelocity * deltaTime + 0.5f * deltaTime * deltaTime * _bubbleAcceleration;
	}

	void DoRandomAccelStep()
	{
		_bubbleAcceleration = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
		_destinationAcceleration = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
	}

	bool IsTouchingSideBounds(Vector3 position)
	{
		return position.x < Camera.main.OrthographicBounds().min.x
		       || position.x > Camera.main.OrthographicBounds().max.x;
	}

	bool IsTouchingTopBottomBounds(Vector3 position)
	{
		return position.y < Camera.main.OrthographicBounds().min.y
		       || position.y > Camera.main.OrthographicBounds().max.y;
	}
	
	bool IsOutOfBounds(Vector3 position)
	{
		return IsTouchingSideBounds(position) || IsTouchingTopBottomBounds(position);
	}

	private SpriteRenderer _sr;
	
	void Start () {
		transform.position = new Vector3(transform.position.x, transform.position.y, zOffset);
		_sr = GetComponent<SpriteRenderer>();
		_endAlpha = _sr.color.a;
		_percentAlpha = 0;
		_sr.color = new Color(_sr.color.r, _sr.color.g, _sr.color.b, 0);
	}
	
	void Update ()
	{
		if (_percentAlpha < 1)
		{
			_percentAlpha += Time.deltaTime / TimeToFull;
			if (_percentAlpha > 1)
			{
				_percentAlpha = 1;
			}
			_sr.color = new Color(_sr.color.r, _sr.color.g, _sr.color.b, _endAlpha * _percentAlpha);
		}
		
		DoRandomAccelStep();
		DoAccelStep(Time.deltaTime);
		DoVelocityStep(Time.deltaTime);
		if (IsOutOfBounds(transform.position) || _bubbleVelocity.magnitude >= 3)
		{
			Pop();
		}
		Debug.DrawLine(transform.position, _destinationPosition);
		
	}

	public void Pop()
	{
		_bubbleVelocity = Vector3.zero;
		SpriteRenderer sr = GetComponent<SpriteRenderer>();
		sr.sprite = popSprite;
		StartCoroutine(DieSoon());
		FindObjectOfType<Art>().RemoveBubble(gameObject);
	}

	private IEnumerator DieSoon()
	{
		yield return new WaitForSeconds(0.5f);
		Destroy(gameObject);
	}
}
