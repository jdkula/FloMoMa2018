using Music;
using UnityEngine;
using Random = System.Random;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(AudioSource))]
public class Line : MonoBehaviour
{
	public delegate void SimpleDelegate();

	public event SimpleDelegate OnFinish;
	public event SimpleDelegate OnDead;

	public string FullName;

	public Art Source;
	public char StartChar;
	public char EndChar;

	public float ZOffset;

	public float SecondsToFinish;
	public float SecondsToFade;

	private float _percentDone;
	private float _timeStart;
	private Vector3 _startVector;
	private Vector3 _movementVector;

	private LineRenderer _line;
	private AudioSource _audioSource;

	private bool _fading;
	private float _fadePercentDone;
	
	
	// Use this for initialization
	public void CStart ()
	{
		_percentDone = 0.0f;
		_timeStart = Time.time;
		_line = GetComponent<LineRenderer>();
		_audioSource = GetComponent<AudioSource>();

		Material m = _line.materials[0];
		_line.materials[0] = m;
		var clr = UnityEngine.Random.ColorHSV(0, 1, 0.75f, 1, 0.5f, 1, 1, 1);
		m.SetColor("_EmissionColor", clr);
		
		
		_line.positionCount = 2;
		_startVector = Source.GetLetterPosition(StartChar) - new Vector3(0, 0, ZOffset);
		_line.SetPosition(0, _startVector);
		_line.SetPosition(1, _startVector + Vector3.back - new Vector3(0, 0, ZOffset));
		_movementVector = Source.GetLetterPosition(EndChar) - _startVector - new Vector3(0, 0, ZOffset);
		_line.widthMultiplier = 0.1f;
		_line.numCapVertices = 10;
		_line.numCornerVertices = 10;

		name = StartChar + "" + EndChar;
	}
	
	// Update is called once per frame
	void Update () {
		_startVector = Source.GetLetterPosition(StartChar) - new Vector3(0, 0, ZOffset);
		_movementVector = Source.GetLetterPosition(EndChar) - _startVector - new Vector3(0, 0, ZOffset);
		
		if (_percentDone < 1f)
		{
			_percentDone += Time.deltaTime / SecondsToFinish;
			if (_percentDone >= 1f)
			{
				_percentDone = 1f;
				OnOnFinish();
			}
		}
		_line.SetPosition(0, _startVector);
		_line.SetPosition(1, _startVector + _movementVector * _percentDone);
		
		if (_fading && _fadePercentDone < 1f)
		{
			Material m = _line.materials[0];
			Color c = m.GetColor("_Color");
			_fadePercentDone += Time.deltaTime / SecondsToFade;
			m.SetColor("_Color", new Color(c.r, c.g, c.b, 1 - _fadePercentDone));
			if (_fadePercentDone >= 1f)
			{
				Destroy(gameObject);
			}
		}
	}

	public void FadeAndDie()
	{
		_fading = true;
	}

	private static Note[][] _diddy;
	private static int _diddyIndex;

	public void PlayNote()
	{
		if (_diddy == null || _diddyIndex >= _diddy.GetLength(0))
		{
			_diddy = MusicGenerator.GetDiddy(FullName.Length, FullName.GetHashCode());
			_diddyIndex = 0;
		}

		foreach (Note note in _diddy[_diddyIndex])
		{
			_audioSource.PlayOneShot(note.AudioClip);
		}
		_diddyIndex++;
	}
	
	protected virtual void OnOnFinish()
	{
		var handler = OnFinish;
		if (handler != null) handler();
	}

	protected virtual void OnOnDead()
	{
		var handler = OnDead;
		if (handler != null) handler();
	}

	private void OnDestroy()
	{
		OnOnDead();
	}
}
