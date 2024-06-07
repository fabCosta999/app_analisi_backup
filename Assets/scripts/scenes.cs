using UnityEngine;
using UnityEngine.SceneManagement;

public class scenes : MonoBehaviour
{
    public void ChangeScene(int n)
    {
        if (n >= 0) SceneManager.LoadScene(n);
        else SceneManager.LoadScene(PlayerPrefs.GetInt(keysGeneral.TOPIC) + 1);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            int sceneIndex = SceneManager.GetActiveScene().buildIndex;
            if (sceneIndex == 0) Application.Quit();
            else if (sceneIndex <= 8 || sceneIndex==10 || sceneIndex == 11) SceneManager.LoadScene(0);
            else SceneManager.LoadScene(PlayerPrefs.GetInt(keysGeneral.TOPIC) + 1);
        }
    }
}
