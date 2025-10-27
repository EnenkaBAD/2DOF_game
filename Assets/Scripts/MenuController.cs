using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{

    // Метод для кнопки "Играть"
    public void PlayGame()
    {
        SceneManager.LoadScene(1);        
    }   
    public void QuitGame()
    {
        Debug.Log("Выход из игры...");

        // Если мы в редакторе Unity
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // Если это собранная версия игры
            Application.Quit();
#endif
    }
}