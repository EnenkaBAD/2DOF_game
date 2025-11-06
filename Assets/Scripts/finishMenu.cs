using LogitechG29.Sample.Input;
using UnityEngine;
using UnityEngine.SceneManagement;

public class finishMenu : MonoBehaviour
{
    public GameTimer timer;

    public InputControllerReader inputControllerReader;
    public void LoadMenu()
    {
        timer.StopTimer();
        gameObject.SetActive(false);
        SceneManager.LoadSceneAsync(0);
    }
    public void RestartGame()
    {
        timer.StopTimer();
        gameObject.SetActive(false);
        SceneManager.LoadSceneAsync(1);
    }

    private void Update()
    {
        if (inputControllerReader.LeftShift) LoadMenu();
        if (inputControllerReader.RightShift) RestartGame();
    }
}
