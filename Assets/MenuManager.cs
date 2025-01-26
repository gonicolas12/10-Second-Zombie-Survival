using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR // Si l'application est en mode édition
        UnityEditor.EditorApplication.isPlaying = false;
        #else // Si l'application est en mode build
            Application.Quit();
        #endif
    }
}