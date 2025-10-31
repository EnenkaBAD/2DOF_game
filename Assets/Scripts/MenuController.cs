using LogitechG29.Sample.Input;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public InputControllerReader inputControllerReader;
    public void PlayGame()
    { 
        SceneManager.LoadSceneAsync(1);
        Debug.Log("Загрузка сцены");   
    }   
    public void QuitGame()
    {
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // Если это собранная версия игры
            Application.Quit();
#endif
    }

    private void Update()
    {
        if (inputControllerReader.LeftShift) PlayGame();
        if (inputControllerReader.RightShift) QuitGame();
    }
}