using Random = System.Random;

namespace Music
{
    public class Scale
    {
        public const int ScaleNoteLength = 7;

        private static StepType[] _majorScale =
        {
            StepType.Whole, StepType.Whole, StepType.Half, StepType.Whole, StepType.Whole, StepType.Whole, StepType.Half
        };
        
        private static StepType[] _naturalMinorScale =
        {
            StepType.Whole, StepType.Half, StepType.Whole, StepType.Whole, StepType.Half, StepType.Whole, StepType.Whole
        };

        private static StepType[] _harmonicMinorScale =
        {
            StepType.Whole, StepType.Half, StepType.Whole, StepType.Whole, StepType.Half, StepType.Augmented,
            StepType.Half
        };

        private static StepType[] _melodicMinorScale =
        {
            StepType.Whole, StepType.Half, StepType.Whole, StepType.Whole, StepType.Whole, StepType.Whole, StepType.Half
        };

        private static StepType[] GetSteps(ScaleType type)
        {
            switch (type)
            {
                case ScaleType.Major:
                    return _majorScale;
                case ScaleType.NaturalMinor:
                    return _naturalMinorScale;
                case ScaleType.HarmonicMinor:
                    return _harmonicMinorScale;
                case ScaleType.MelodicMinor:
                    return _melodicMinorScale;
            }

            return null;
        }

        private Note _startNote;
        private StepType[] _scale;

        public Note GetNote(int scaleDegree, Accidental acc = Accidental.Natural)
        {
            int noteIndex = MusicGenerator.GetNoteIndex(_startNote.NoteName);
            int octaveIndex = MusicGenerator.GetOctaveIndex(_startNote.Octave);
            int multiplier = scaleDegree < 0 ? -1 : 1;
            int additive = scaleDegree < 0 ? -1 : 0;
            scaleDegree *= multiplier;
            noteIndex += (int) acc;
            for (int i = 0; i < scaleDegree; i++)
            {
                noteIndex += (int) _scale[mod(i * multiplier + additive, _scale.Length)] * multiplier;
                if (noteIndex >= MusicGenerator.Notes.GetLength(0) || noteIndex < 0)
                {
                    noteIndex = mod(noteIndex, MusicGenerator.Notes.GetLength(0));
                    octaveIndex += multiplier;
                    if (octaveIndex >= MusicGenerator.Notes.GetLength(1))
                    {
                        octaveIndex = 0;
                    }

                    if (octaveIndex < 0)
                    {
                        octaveIndex = MusicGenerator.Notes.GetLength(1) - 1;
                    }
                }
            }

            return MusicGenerator.Notes[noteIndex, octaveIndex];
        }
        
        public Note[] GetTriad(int rootDegree, Tonality tonality, int inversion = 0)
        {
            if (tonality == Tonality.Major)
            {
                switch (inversion)
                {
                    case 0:
                        return new[] {GetNote(rootDegree), GetNote(rootDegree + 2), GetNote(rootDegree + 4)};
                    case 1:
                        return new[] {GetNote(rootDegree + 2 - 7), GetNote(rootDegree + 4 - 7), GetNote(rootDegree)};
                    case 2:
                        return new[] {GetNote(rootDegree + 4 - 7), GetNote(rootDegree), GetNote(rootDegree + 2)};
                }
            }
            else if (tonality == Tonality.Minor)
            {
                switch (inversion)
                {
                    case 0:
                        return new[]
                            {GetNote(rootDegree), GetNote(rootDegree + 2, Accidental.Flat), GetNote(rootDegree + 4)};
                    case 1:
                        return new[]
                            {GetNote(rootDegree - 5, Accidental.Flat), GetNote(rootDegree - 3), GetNote(rootDegree)};
                    case 2:
                        return new[]
                            {GetNote(rootDegree - 3), GetNote(rootDegree), GetNote(rootDegree + 2, Accidental.Flat)};
                }
            }
            
            return null;
        }
        
        private int mod(int x, int m)
        {
            return (x % m + m) % m;
        }
        
        public Scale(Note startNote, ScaleType type)
        {
            _startNote = startNote;
            _scale = GetSteps(type);
        }

        public static ScaleType GetRandomScaleType(Random r = null)
        {
            if (r == null) r = new Random();
            return (ScaleType) r.Next(1);
        }

        public override string ToString()
        {
            Note[] arr =
            {
                GetNote(0),
                GetNote(1),
                GetNote(2),
                GetNote(3),
                GetNote(4),
                GetNote(5),
                GetNote(6),
                GetNote(7)
            };
            Note[] arr2 =
            {
                GetNote(0),
                GetNote(-1),
                GetNote(-2),
                GetNote(-3),
                GetNote(-4),
                GetNote(-5),
                GetNote(-6),
                GetNote(-7)
            };
            return arr2.ArrayToString() + " | " + arr.ArrayToString();
        }
    }
}
