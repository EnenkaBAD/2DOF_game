using UnityEngine;

public class Finish : MonoBehaviour
{
    public GameTimer timer;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) timer.PauseTimer();

    }
}
