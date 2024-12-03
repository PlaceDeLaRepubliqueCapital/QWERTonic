using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;
using System;


public class NotationSystemEditor : EditorWindow
{
    private string musicTitle = "";
    private string notesInput = ""; // Each line will be one bar, notes separated by spaces
    private GameObject notePrefabsContainer;
    private Vector3 scrollPosition;
    private string StandardizeNoteName(string note)
    {
        // Replace special sharp symbol (♯) with #
        return note.Replace("♯", "#").Trim();
    }
    // Dictionary to store note to key mappings
    private static readonly Dictionary<string, KeyCode> noteToKeyMapping = new Dictionary<string, KeyCode>
    {
        {"C1", KeyCode.Alpha1}, {"C#1", KeyCode.Q}, {"D1", KeyCode.Alpha2},
        {"D#1", KeyCode.W}, {"E1", KeyCode.Alpha3}, {"F1", KeyCode.E},
        {"F#1", KeyCode.Alpha4}, {"G1", KeyCode.R}, {"G#1", KeyCode.Alpha5},
        {"A1", KeyCode.T}, {"A#1", KeyCode.Alpha6}, {"B1", KeyCode.Y},
        {"C2", KeyCode.Alpha7}, {"C#2", KeyCode.U}, {"D2", KeyCode.Alpha8},
        {"D#2", KeyCode.I}, {"E2", KeyCode.Alpha9}, {"F2", KeyCode.O},
        {"F#2", KeyCode.Alpha0}, {"G2", KeyCode.P}, {"G#2", KeyCode.Minus},
        {"A2", KeyCode.LeftBracket}, {"A#2", KeyCode.Equals}, {"B2", KeyCode.RightBracket},
        {"C3", KeyCode.A}, {"C#3", KeyCode.Z}, {"D3", KeyCode.S},
        {"D#3", KeyCode.X}, {"E3", KeyCode.D}, {"F3", KeyCode.C},
        {"F#3", KeyCode.F}, {"G3", KeyCode.V}, {"G#3", KeyCode.G},
        {"A3", KeyCode.B}, {"A#3", KeyCode.H}, {"B3", KeyCode.N},
        {"C4", KeyCode.J}, {"C#4", KeyCode.M}, {"D4", KeyCode.K},
        {"D#4", KeyCode.Comma}, {"E4", KeyCode.L}, {"F4", KeyCode.Period},
        {"F#4", KeyCode.Colon}, {"G4", KeyCode.Slash}, {"G#4", KeyCode.Quote}
    };

    [MenuItem("Window/Notation System Generator")]
    public static void ShowWindow()
    {
        GetWindow<NotationSystemEditor>("Notation System Generator");
    }

    void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        EditorGUILayout.LabelField("Notation System Generator", EditorStyles.boldLabel);

        musicTitle = EditorGUILayout.TextField("Music Title", musicTitle);

        EditorGUILayout.HelpBox(
            "Enter notes for each bar on a new line, separated by spaces.\n" +
            "Example:\nC1 D1 E1\nF1 G1 A1",
            MessageType.Info);

        notesInput = EditorGUILayout.TextArea(notesInput, GUILayout.Height(200));

        notePrefabsContainer = (GameObject)EditorGUILayout.ObjectField(
            "Note Prefabs Container",
            notePrefabsContainer,
            typeof(GameObject),
            true);

        if (GUILayout.Button("Generate Notation System") && !string.IsNullOrEmpty(musicTitle))
        {
            GenerateNotationSystem();
        }

        EditorGUILayout.EndScrollView();
    }

    // ... (previous code remains the same until GenerateNotationSystem method) ...

    private void GenerateNotationSystem()
    {
        if (notePrefabsContainer == null)
        {
            EditorUtility.DisplayDialog("Error", "Please assign the Note Prefabs Container!", "OK");
            return;
        }

        // Create main music piece object
        GameObject musicPieceObject = new GameObject(musicTitle);
        Undo.RegisterCreatedObjectUndo(musicPieceObject, "Generate Notation System");

        string[] bars = notesInput.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
        GameObject lastNoteOfPreviousBar = null;

        try
        {
            for (int barIndex = 0; barIndex < bars.Length; barIndex++)
            {
                if (string.IsNullOrWhiteSpace(bars[barIndex])) continue;

                // Create bar object
                GameObject barObject = new GameObject($"bar{barIndex + 1}");
                Undo.RegisterCreatedObjectUndo(barObject, "Create Bar Object");
                barObject.transform.SetParent(musicPieceObject.transform);

                string[] chords = bars[barIndex].Trim().Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
                GameObject previousNoteObject = null;
                GameObject firstNoteInBar = null;

                for (int chordIndex = 0; chordIndex < chords.Length; chordIndex++)
                {
                    string chord = chords[chordIndex].Trim();
                    if (string.IsNullOrEmpty(chord)) continue;

                    // Split the chord into individual notes
                    string[] notes = chord.Split('+');

                    // Create note object for the chord
                    GameObject noteObject = new GameObject($"{chordIndex + 1}-{barIndex + 1}");
                    Undo.RegisterCreatedObjectUndo(noteObject, "Create Note Object");
                    noteObject.transform.SetParent(barObject.transform);

                    // Store reference to first note in bar
                    if (chordIndex == 0)
                    {
                        firstNoteInBar = noteObject;
                        // Link last note of previous bar to first note of current bar
                        if (lastNoteOfPreviousBar != null)
                        {
                            Notation lastNotation = lastNoteOfPreviousBar.GetComponent<Notation>();
                            if (lastNotation.nextObjects != null)
                            {
                                lastNotation.nextObjects[0] = noteObject;
                            }
                        }
                    }

                    // Set active state: only activate 1-1
                    bool isFirstNote = (barIndex == 0 && chordIndex == 0);
                    noteObject.SetActive(isFirstNote);

                    // Add Notation component
                    Notation notation = noteObject.AddComponent<Notation>();

                    // Set required keys for the chord
                    List<KeyCode> requiredKeys = new List<KeyCode>();
                    foreach (string note in notes)
                    {
                        string standardizedNote = StandardizeNoteName(note);
                        if (noteToKeyMapping.TryGetValue(standardizedNote, out KeyCode keyCode))
                        {
                            requiredKeys.Add(keyCode);
                        }
                        else
                        {
                            Debug.LogError($"No key mapping found for note: {note} (standardized: {standardizedNote})");
                        }
                    }
                    notation.requiredKeys = requiredKeys.ToArray();

                    // Initialize nextObjects array if not the last chord in the entire piece
                    bool isLastChordInPiece = (barIndex == bars.Length - 1) && (chordIndex == chords.Length - 1);
                    if (!isLastChordInPiece)
                    {
                        notation.nextObjects = new GameObject[1];
                    }

                    // Link to previous note within the same bar
                    if (previousNoteObject != null)
                    {
                        Notation prevNotation = previousNoteObject.GetComponent<Notation>();
                        if (prevNotation.nextObjects != null)
                        {
                            prevNotation.nextObjects[0] = noteObject;
                        }
                    }

                    // For each note in the chord, instantiate its prefab
                    foreach (string note in notes)
                    {
                        string trimmedNote = StandardizeNoteName(note);
                        Debug.Log($"Looking for note prefab: {trimmedNote}");

                        if (notePrefabsContainer == null)
                        {
                            Debug.LogError("Note Prefabs Container is null!");
                            continue;
                        }

                        Transform notePrefab = FindNotePrefab(trimmedNote);
                        if (notePrefab != null)
                        {
                            // Create the instance as a direct child of the note object
                            GameObject prefabInstance = GameObject.Instantiate(notePrefab.gameObject);
                            if (prefabInstance != null)
                            {
                                Undo.RegisterCreatedObjectUndo(prefabInstance, "Create Note Prefab Instance");

                                prefabInstance.transform.SetParent(noteObject.transform);
                                prefabInstance.name = trimmedNote;

                                Vector3 prefabLocalPos = notePrefab.localPosition;
                                prefabInstance.transform.localPosition = new Vector3(
                                    prefabLocalPos.x + 27f,
                                    prefabLocalPos.y + 308f,
                                    prefabLocalPos.z
                                );

                                prefabInstance.transform.localRotation = notePrefab.localRotation;
                                prefabInstance.transform.localScale = notePrefab.localScale;

                                Debug.Log($"Successfully created prefab instance for note: {trimmedNote} under {noteObject.name}");
                            }
                        }
                        else
                        {
                            Debug.LogError($"Could not find note prefab for '{trimmedNote}' after trying all search methods");
                        }
                    }

                    previousNoteObject = noteObject;

                    // Update last note of bar reference
                    if (chordIndex == chords.Length - 1)
                    {
                        lastNoteOfPreviousBar = noteObject;
                    }
                }
            }

            Debug.Log($"Generated notation system for: {musicTitle}");
            EditorUtility.DisplayDialog("Success", $"Generated notation system for: {musicTitle}", "OK");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error generating notation system: {e.Message}\n{e.StackTrace}");
            EditorUtility.DisplayDialog("Error", "Failed to generate notation system. Check console for details.", "OK");

            // Clean up on error
            if (musicPieceObject != null)
            {
                Undo.DestroyObjectImmediate(musicPieceObject);
            }
        }
    }

    private Transform FindNotePrefab(string noteName)
    {
        if (notePrefabsContainer == null) return null;

        // Try direct find first
        Transform prefab = notePrefabsContainer.transform.Find(noteName);
        if (prefab != null) return prefab;

        // If not found, try alternative sharp symbol
        string alternateNoteName = noteName.Contains("#")
            ? noteName.Replace("#", "♯")
            : noteName.Replace("♯", "#");

        prefab = notePrefabsContainer.transform.Find(alternateNoteName);
        if (prefab != null) return prefab;

        // If still not found, try searching all children
        foreach (Transform child in notePrefabsContainer.transform)
        {
            if (string.Equals(child.name, noteName, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(child.name, alternateNoteName, StringComparison.OrdinalIgnoreCase))
            {
                return child;
            }
        }

        // Debug information - fixed string interpolation
        Debug.LogError("Searching for note: '" + noteName + "' or '" + alternateNoteName + "'");
        string availablePrefabs = string.Join(", ", notePrefabsContainer.transform.Cast<Transform>().Select(t => t.name));
        Debug.LogError("Available prefabs: " + availablePrefabs);

        // Also print each prefab name with its length to check for hidden characters
        foreach (Transform child in notePrefabsContainer.transform)
        {
            Debug.LogError($"Prefab: '{child.name}' (Length: {child.name.Length})");
        }

        return null;
    }
}