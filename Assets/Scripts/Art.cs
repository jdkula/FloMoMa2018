using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Art : MonoBehaviour
{
    public string[] Names;

    public static int SeedShifter = 0;
    public static float Volume = 1f;

    public float Radius = 5;

    public GameObject StaticLinePrefab;
    public GameObject DynamicLinePrefab;
    public GameObject LetterPrefab;
    public GameObject BubblePrefab;

    public GameObject Canvas;
    public Text StatusText;
    public Text ClockText;

    public Dictionary<char, Vector3> Locations;

    private int _currentIndex;

    private delegate void SimpleDelegate();

    private event SimpleDelegate OnLinkDone;

    private ArrayList _bubbles = new ArrayList();

    void Shuffle(string[] texts)
    {
        for (int t = 0; t < texts.Length; t++)
        {
            string tmp = texts[t];
            int r = Random.Range(t, texts.Length);
            texts[t] = texts[r];
            texts[r] = tmp;
        }
    }


    // Use this for initialization
    void Start()
    {
        var ta = Resources.Load<TextAsset>("residents");
        Names = ta.text.Split('\n');
        Shuffle(Names);
        Locations = new Dictionary<char, Vector3>();
        CreatePoints();

        for (int i = 0; i < Names.Length; i++)
        {
            Regex r = new Regex("[A-Z]+");
            MatchCollection m = r.Matches(Names[i].ToUpper());
            Names[i] = "";
            foreach (Match match in m)
            {
                Names[i] += match.Value;
            }
        }

        LinkName(Names[_currentIndex], 1);

        OnLinkDone += delegate
        {
            _currentIndex = (_currentIndex + 1) % Names.Length;
            if (_currentIndex == 0) Shuffle(Names);
            LinkName(Names[_currentIndex], 1);
        };
    }


    void CreatePoints()
    {
        float angle = 0;
        for (char ltr = 'A'; ltr <= 'Z'; ltr++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * Radius;
            float y = Mathf.Cos(Mathf.Deg2Rad * angle) * Radius;

            /*GameObject line = Instantiate(StaticLinePrefab, transform);
            line.transform.position = new Vector3(x, y, 0);
            LineRenderer lr = line.GetComponent<LineRenderer>();
            lr.positionCount = 2;
            lr.SetPosition(0,
                new Vector3(Mathf.Sin(Mathf.Deg2Rad * angle) * 10.3f * Radius / 10f,
                    Mathf.Cos(Mathf.Deg2Rad * angle) * 10.3f * Radius / 10f));
            lr.SetPosition(1,
                new Vector3(Mathf.Sin(Mathf.Deg2Rad * angle) * 9.7f * Radius / 10f,
                    Mathf.Cos(Mathf.Deg2Rad * angle) * 9.7f * Radius / 10f));
            lr.widthMultiplier = 0.1f;*/

            GameObject letter = Instantiate(LetterPrefab, transform);
            TextMesh text = letter.GetComponent<TextMesh>();
            text.text = ltr.ToString();
            letter.name = ltr.ToString();
            //line.name = ltr + " line";

            Vector3 offset = new Vector3(-GetWidth(text) / 2f, GetHeight(text) / 2f, 0);
            Vector3 letterPos = new Vector3(x, y, 0);
            letter.transform.position = letterPos + offset;
            Locations.Add(ltr, letterPos);

//            Letter l = letter.AddComponent<Letter>();
//            letter.AddComponent<Bubble>();
//            l.art = this;
//            l.letter = ltr;
//            l.offsetVector = offset;

            angle += 360f / 26f;
        }
    }

    public void LinkName(string name, int currentLine)
    {
        if (currentLine < name.Length)
        {
            if (currentLine == 1)
            {
                UnityEngine.Random.InitState(name.GetHashCode() + SeedShifter);
            }

            GameObject line = Instantiate(DynamicLinePrefab, transform);
            line.name = name;
            Line l = line.GetComponent<Line>();
            l.StartChar = name[currentLine - 1];
            l.EndChar = name[currentLine];
            l.Source = this;
            l.SecondsToFinish = 1;
            l.SecondsToFade = 2;
            l.ZOffset = currentLine;
            l.FullName = name;
            l.CStart();
            if (currentLine == 1)
            {
                l.PlayNote();
            }

            l.OnFinish += delegate
            {
                l.PlayNote();
                LinkName(name, currentLine + 1);
            };
        }
        else
        {
            var lines = FindObjectsOfType<Line>();
            foreach (var line in lines)
            {
                line.FadeAndDie();
            }

            StartCoroutine(WaitThenGo(1));
        }
    }

    private IEnumerator WaitThenGo(int waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        OnOnLinkDone();
    }

    public Vector3 GetLetterPosition(char letter)
    {
        return Locations[letter];
    }

    // https://answers.unity.com/questions/31622/is-it-possible-to-find-the-width-of-a-text-mesh.html
    public static float GetWidth(TextMesh mesh)
    {
        float width = 0;
        foreach (char symbol in mesh.text)
        {
            CharacterInfo info;
            if (mesh.font.GetCharacterInfo(symbol, out info, mesh.fontSize, mesh.fontStyle))
            {
                width += info.advance;
            }
        }

        return width * mesh.characterSize * 0.1f;
    }

    public static float GetHeight(TextMesh mesh)
    {
        float width = 0;
        foreach (char symbol in mesh.text)
        {
            CharacterInfo info;
            if (mesh.font.GetCharacterInfo(symbol, out info, mesh.fontSize, mesh.fontStyle))
            {
                width += info.glyphHeight;
            }
        }

        return width * mesh.characterSize * 0.1f;
    }

    public void RemoveBubble(GameObject bubble)
    {
        _bubbles.Remove(bubble);
    }

    private int mod(int x, int m)
    {
        return (x % m + m) % m;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (_bubbles.Count < 5)
        {
            GameObject bubble = Instantiate(BubblePrefab);
            _bubbles.Add(bubble);
            bubble.transform.position = new Vector3(
                Random.Range(Camera.main.OrthographicBounds().min.x, Camera.main.OrthographicBounds().max.x),
                Random.Range(Camera.main.OrthographicBounds().min.y, Camera.main.OrthographicBounds().max.y)
            );
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Time.timeScale -= 0.1f;
            if (!Canvas.activeSelf) Canvas.SetActive(true);
            StatusText.text = "Time Scale: " + Time.timeScale;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Time.timeScale += 0.1f;
            if (!Canvas.activeSelf) Canvas.SetActive(true);
            StatusText.text = "Time Scale: " + Time.timeScale;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SeedShifter += 1;
            if (!Canvas.activeSelf) Canvas.SetActive(true);
            StatusText.text = "Random Adjust: " + SeedShifter;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SeedShifter -= 1;
            if (!Canvas.activeSelf) Canvas.SetActive(true);
            StatusText.text = "Random Adjust: " + SeedShifter;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SeedShifter = 0;
            Time.timeScale = 1f;
            if (!Canvas.activeSelf) Canvas.SetActive(true);
            StatusText.text = "Reset";
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            Shuffle(Names);
            _currentIndex = 0;
            if (!Canvas.activeSelf) Canvas.SetActive(true);
            StatusText.text = "Shuffled Names";
        }
        
        if (Input.GetKey(KeyCode.S))
        {
            if (!Canvas.activeSelf) Canvas.SetActive(true);
            StatusText.text = "Random Adjust: " + SeedShifter + "\nTime Scale: " + Time.timeScale
                              + "\nVolume: " + Volume
                              + "\nName " + (_currentIndex + 1) + "/" + Names.Length;
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            if (!Canvas.activeSelf) Canvas.SetActive(true);
            StatusText.text = "Help:\nUp/Down: Adjust Speed\nLeft/Right: Adjust Random Generation\n"
                + "[/]: Previous/Skip\n,/. Volume down/up\nSPACE: Pause/Play\n"
                + "S: Show Status\nR: Reset Time and Random Generation\nF: Shuffle Names\nH: Help"
                + "\nESC: Quit";
        }

        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            _currentIndex = mod(_currentIndex - 1, Names.Length);
            if (!Canvas.activeSelf) Canvas.SetActive(true);
            StatusText.text = "Next Name: " + (_currentIndex + 2) % Names.Length + "/" + Names.Length;
        }

        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            _currentIndex = mod(_currentIndex + 1, Names.Length);
            if (!Canvas.activeSelf) Canvas.SetActive(true);
            StatusText.text = "Next Name: " + (_currentIndex + 2) % Names.Length + "/" + Names.Length;
        }

        if (Input.GetKeyDown(KeyCode.Comma))
        {
            Volume = bound(Volume - 0.1f, 0f, 1f);
            if (!Canvas.activeSelf) Canvas.SetActive(true);
            StatusText.text = "Volume: " + Volume;
        }
        
        if (Input.GetKeyDown(KeyCode.Period))
        {
            Volume = bound(Volume + 0.1f, 0f, 1f);
            if (!Canvas.activeSelf) Canvas.SetActive(true);
            StatusText.text = "Volume: " + Volume;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Math.Abs(Time.timeScale) > 0.001f)
            {
                Time.timeScale = 0;
                if (!Canvas.activeSelf) Canvas.SetActive(true);
                StatusText.text = "Paused.";
            }
            else
            {
                Time.timeScale = 1f;
                if (!Canvas.activeSelf) Canvas.SetActive(true);
                StatusText.text = "Unpaused.";
            }
        }

        if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.UpArrow) ||
            Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow)
            || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.R)
            || Input.GetKeyUp(KeyCode.F) || Input.GetKeyUp(KeyCode.H)
            || Input.GetKeyUp(KeyCode.RightBracket) || Input.GetKeyUp(KeyCode.LeftBracket)
            || Input.GetKeyUp(KeyCode.Comma) || Input.GetKeyUp(KeyCode.Period)
            || Input.GetKeyUp(KeyCode.Space))
        {
            StatusText.text = "";
            Canvas.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
#if UNITY_EDITOR || UNITY_EDITOR_64 || UNITY_EDITOR_WIN
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        var now = System.DateTime.Now;
        ClockText.text = now.ToString("h:mm");
    }

    private float bound(float value, float min, float max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }

    protected virtual void OnOnLinkDone()
    {
        var handler = OnLinkDone;
        if (handler != null) handler();
    }
}