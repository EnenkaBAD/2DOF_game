using Bhaptics.SDK2;
using UnityEngine;

public class rightDamageEvent : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            BhapticsLibrary.Play(eventId: BhapticsEvent.RIGHT_DAMAGE);
            Debug.Log("ÒÛ×ÊÀ ÑÏÐÀÂÀ!!!!!!!!!!!");
        }
    }
}
