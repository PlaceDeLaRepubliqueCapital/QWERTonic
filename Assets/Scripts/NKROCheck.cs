using UnityEngine;
using UnityEngine.SceneManagement;

public class NKROCheck : MonoBehaviour
{
    public string nextSceneName;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LoadNextScene();
        }
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
