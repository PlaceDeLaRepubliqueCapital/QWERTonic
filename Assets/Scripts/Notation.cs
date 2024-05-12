using System;
using System.Collections.Generic;
using UnityEngine;
public class Notation : MonoBehaviour
{
    public KeyCode[] requiredKeys;
    public GameObject[] nextObjects;

    bool activated = false;

    Dictionary<GameObject, bool> activationStates = new Dictionary<GameObject, bool>();

    void Start()
    {
        if (nextObjects != null)
        {
            foreach (GameObject obj in nextObjects)
            {
                obj.SetActive(false); // Ensure the next objects are initially hidden
                activationStates[obj] = false; // Initialize the activation state for each nextObject
            }
        }
    }

    void Update()
    {
        if (!activated && CheckKeysPressed())
        {
            activated = true;
            if (nextObjects != null)
            {
                foreach (GameObject obj in nextObjects)
                {
                    if (!activationStates[obj] && CheckKeysPressedForNextObject(obj))
                    {
                        activationStates[obj] = true; // Update the activation state for the nextObject
                        obj.SetActive(true); // Show the next object
                    }
                }
                gameObject.SetActive(false); // Hide the current object
            }
        }
    }

    bool CheckKeysPressed()
    {
        if (requiredKeys.Length == 1)
        {
            return Input.GetKeyDown(requiredKeys[0]); // Return true if the single required key is pressed
        }
        else
        {
            bool[] keysPressed = new bool[requiredKeys.Length]; // Create an array to track the state of each required key

            for (int i = 0; i < requiredKeys.Length; i++)
            {
                keysPressed[i] = Input.GetKey(requiredKeys[i]); // Check if each required key is pressed
                Debug.Log("Key " + requiredKeys[i] + " pressed: " + keysPressed[i]); // Add debug log
            }

            return Array.TrueForAll(keysPressed, x => x); // Return true if all required keys are pressed
        }
    }
    bool CheckKeysPressedForNextObject(GameObject obj)
    {
        if (requiredKeys.Length == 1)
        {
            return Input.GetKeyDown(requiredKeys[0]); // Return true if the single required key is pressed
        }
        else
        {
            if (!activationStates[obj])
            {
                bool[] keysPressed = new bool[requiredKeys.Length]; // Create an array to track the state of each required key

                for (int i = 0; i < requiredKeys.Length; i++)
                {
                    keysPressed[i] = Input.GetKey(requiredKeys[i]); // Check if each required key is pressed
                    Debug.Log("Key " + requiredKeys[i] + " pressed: " + keysPressed[i]); // Add debug log
                }

                return Array.TrueForAll(keysPressed, x => x); // Return true if all required keys are pressed
            }
            else
            {
                return false; // Return false if the nextObject is already activated
            }
        }
    }

}