using Bhaptics.SDK2;
using UnityEngine;

public class fwDamageEvent : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            BhapticsLibrary.Play(eventId: BhapticsEvent.FW_DAMAGE);
            Debug.Log("ршвйю яоепедх!!!!!!!!!!!");
        }
    }
}
