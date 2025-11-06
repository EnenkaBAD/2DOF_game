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
            Invoke("activeUI", 3f);
        }
    }

    private void activeUI()
        { restartUI.SetActive(true); }
}
