using UnityEngine;

public class Note
{

    public AudioClip AudioClip { private set; get; }

    public readonly string NoteName;
    public readonly int Octave;
    
    public Note(string note, int octave)
    {
        NoteName = note;
        Octave = octave;
        AudioClip = Resources.Load<AudioClip>("SFX/Downbeat Sounds/StationAdded_" + note + octave);
    }

    public Note(string resource)
    {
        AudioClip = Resources.Load<AudioClip>(resource);
    }

    public override string ToString()
    {
        return NoteName + Octave;
    }
}
