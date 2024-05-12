using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectMusic: MonoBehaviour
{
    public string nextSceneName;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            LoadNextScene();
        }
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
