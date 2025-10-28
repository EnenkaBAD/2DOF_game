using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
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
}