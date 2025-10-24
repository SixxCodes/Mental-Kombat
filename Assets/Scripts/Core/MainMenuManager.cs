using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("CoreGameScene");
    }

    //public void QuitGame()
    //{
    //    Debug.Log("Quit Game");
    //    Application.Quit(); // Won't quit in WebGL but fine in builds
    //}
}
