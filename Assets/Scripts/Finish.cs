using UnityEngine;

public class Finish : MonoBehaviour
{
    public GameTimer timer;
    public GameObject restartUI;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            timer.PauseTimer();
            restartUI.SetActive(true);
        }
    }
}
