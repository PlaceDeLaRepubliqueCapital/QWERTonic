using System;
using System.Collections.Generic;
using UnityEngine;

public class Notation : MonoBehaviour
{
    public KeyCode[] requiredKeys;
    public GameObject[] nextObjects;

    private bool activated = false;
    private Dictionary<GameObject, bool> activationStates = new Dictionary<GameObject, bool>();

    void Start()
    {
        if (nextObjects != null)
        {
            foreach (GameObject obj in nextObjects)
            {
                obj.SetActive(false);
                activationStates[obj] = false;
            }
        }
    }

    void Update()
    {
        // Handle chord/key detection
        if (!activated && CheckKeysPressed())
        {
            activated = true;
            if (nextObjects != null)
            {
                foreach (GameObject obj in nextObjects)
                {
                    if (!activationStates[obj])
                    {
                        activationStates[obj] = true;
                        obj.SetActive(true);
                    }
                }
                gameObject.SetActive(false);
            }
        }
    }

    bool CheckKeysPressed()
    {
        if (requiredKeys.Length == 1)
        {
            return Input.GetKeyDown(requiredKeys[0]);
        }

        // For chords (multiple keys)
        bool anyKeyDown = false;
        bool allKeysHeld = true;

        // Check if any key was just pressed down and all required keys are held
        foreach (KeyCode key in requiredKeys)
        {
            if (Input.GetKeyDown(key))
            {
                anyKeyDown = true;
            }
            if (!Input.GetKey(key))
            {
                allKeysHeld = false;
            }
        }

        return anyKeyDown && allKeysHeld;
    }
}