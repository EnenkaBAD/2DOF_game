using UnityEngine;
using TMPro;  // Для TextMeshPro


public class GameTimer : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;  // Поле для отображения времени

    private float elapsedTime = 0f;
    private bool isRunning = false;


    // Метод: запустить таймер
    public void StartTimer()
    {
        isRunning = true;
    }

    // Метод: остановить таймер (сбросить время)
    public void StopTimer()
    {
        isRunning = false;
        elapsedTime = 0f;
        UpdateDisplay();
    }

    // Метод: приостановить таймер (не сбрасывает время)
    public void PauseTimer()
    {
        isRunning = false;
    }


    // Метод: получить текущее время в секундах
    public float GetElapsedTime()
    {
        return elapsedTime;
    }

    // Метод: проверить, работает ли таймер
    public bool IsRunning()
    {
        return isRunning;
    }

    private void Update()
    {
        if (isRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateDisplay();
        }
    }

    // Обновляет отображение времени (формат MM:SS.ff)
    private void UpdateDisplay()
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        int milliseconds = Mathf.FloorToInt((elapsedTime * 100) % 100);

        timerText.text = string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);
    }
}
