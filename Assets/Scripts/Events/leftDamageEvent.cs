using Bhaptics.SDK2;
using UnityEngine;

public class leftDamageEvent: MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            BhapticsLibrary.Play(eventId: BhapticsEvent.LEFT_DAMAGE);
            Debug.Log("ршвйю якебю!!!!!!!!!!!");
        }
    }
}
