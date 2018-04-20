using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Music
{
    public static class MusicGenerator
    {

        public static readonly Note[,] Notes;
        public static readonly Note[] Bass;
        private static readonly String[] NoteNames = {"C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B"};
        private static readonly int[] OctaveRange = {4, 5, 6};
    
        static MusicGenerator()
        {
            Notes = new Note[NoteNames.Length, OctaveRange.Length];
            Bass = new Note[NoteNames.Length];
            for (int n = 0; n < NoteNames.Length; n++)
            {
                for (int o = 0; o < OctaveRange.Length; o++)
                {
                    Notes[n, o] = new Note(NoteNames[n], OctaveRange[o]);
                }

                Bass[n] = new Note("SFX/Downbeat Bass/bass_" + NoteNames[n]);
            }
        }

        public static Note[][] GetDiddy(int length, int genSeed = 0)
        {
            Random r;
            if (genSeed == 0)
            {
                r = new Random();
            }
            else
            {
                r = new Random(genSeed);
            }

            Note[][] diddy = new Note[length][];

            Note startNote = GetRandomNote(r);
            Scale scale = new Scale(startNote, Scale.GetRandomScaleType(r));
            Debug.Log(scale);
            var nt1 = r.Next(0, 8);
            var nv1 = r.Next(0, 3);
            diddy[0] = scale.GetTriad(nt1, Tonality.Major, nv1);
            Debug.Log("TRIAD: " + nt1 + "/" + nv1 + " => " + diddy[0].ArrayToString());
            for (int i = 1; i < length; i++)
            {
                int numNotes = r.Next(3) + 1;
                switch (numNotes)
                {
                    case 1:
                        diddy[i] = new[] {scale.GetNote(r.Next(0, 8))};
                        break;
                    case 2:
                        diddy[i] = new[] {scale.GetNote(r.Next(0, 8))};
                        break;
                    case 3:
                        var nt = r.Next(0, 8);
                        var nv = r.Next(0, 3);
                        diddy[i] = scale.GetTriad(nt, Tonality.Major, nv);
                        Debug.Log("TRIAD: " + nt + "/" + nv + " => " + diddy[i].ArrayToString());
                        break;
                }
            }

            diddy[diddy.Length - 1] = scale.GetTriad(0, Tonality.Major, 1);
            Debug.Log("First Note: " + startNote);
            diddy.PrintNoteArray();
            return diddy;
        }

        private static Note GetRandomNote(Random r = null)
        {
            if (r == null) r = new Random();
            int randomNote = r.Next(Notes.GetLength(0));
            int randomOctave = r.Next(1, Notes.GetLength(1) - 1);
            return Notes[randomNote, randomOctave];
        }

        public static int GetNoteIndex(string noteName)
        {
            return new List<string>(NoteNames).FindIndex(s => noteName == s);
        }

        public static int GetOctaveIndex(int octaveNumber)
        {
            return octaveNumber >= OctaveRange[0] && octaveNumber <= OctaveRange[OctaveRange.Length - 1]
                ? octaveNumber - OctaveRange[0]
                : -1;
        }

        public static void PrintNoteArray(this Note[][] notes)
        {
            string s = "{";
            foreach (Note[] ns in notes)
            {
                s += ns.ArrayToString();
                s += ", ";
            }

            s = s.Remove(s.Length - 2, 2);
            s += "}";
            Debug.Log(s);
        }
        
        public static string ArrayToString(this object[] arr)
        {
            string s = "{";
            foreach (object o in arr)
            {
                s += o.ToString();
                s += ", ";
            }

            s = s.Remove(s.Length - 2, 2);
            s += "}";
            return s;
        }
    
    }
}
