using UnityEngine;

namespace Music
{
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
            NoteName = resource;
            Octave = -1;
            AudioClip = Resources.Load<AudioClip>(resource);
        }

        public override string ToString()
        {
            return NoteName + Octave;
        }

        public Note GetInterval(int size, Tonality tone)
        {
            switch (tone)
            {
                case Tonality.Major:
                    return new Scale(this, ScaleType.Major).GetNote(size);
                case Tonality.Minor:
                    return new Scale(this, ScaleType.NaturalMinor).GetNote(size);
                case Tonality.Augmented:
                    return new Scale(this, ScaleType.Major).GetNote(size, 1);
                case Tonality.Diminished:
                    return new Scale(this, ScaleType.NaturalMinor).GetNote(size, -1);
                default:
                    return null;
            }
        }

        public Note Adjust(int semitones)
        {
            int noteIndex = MusicGenerator.GetNoteIndex(NoteName);
            int octaveIndex = MusicGenerator.GetOctaveIndex(Octave);
            int over = (noteIndex + semitones) / MusicGenerator.Notes.GetLength(0);
            noteIndex = mod(noteIndex + semitones, MusicGenerator.Notes.GetLength(0));
            octaveIndex = mod(octaveIndex + over, MusicGenerator.Notes.GetLength(1));

            return MusicGenerator.Notes[noteIndex, octaveIndex];
        }
        
        
        private int mod(int x, int m)
        {
            return (x % m + m) % m;
        }

        public Note GetBass()
        {
            return new Note("SFX/Downbeat Bass/bass_" + NoteName);
        }
    }
}