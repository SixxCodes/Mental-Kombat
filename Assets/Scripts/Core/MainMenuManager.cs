// ============== IMPORTS  ==================
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("CoreGameScene"); // Load Assets/Scenes/CoreGameScene.unity
    }

    // Wla ko nag-add ug quit kay Web game sa mn ni
    //public void QuitGame()
    //{
    //    Debug.Log("Quit Game");
    //    Application.Quit(); // Won't quit in WebGL but fine in builds
    //}
}
