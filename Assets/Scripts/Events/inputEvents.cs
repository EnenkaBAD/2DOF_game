using Bhaptics.SDK2;
using LogitechG29.Sample.Input;
using UnityEngine;

public class inputEvents : MonoBehaviour
{
    [SerializeField] private InputControllerReader inputControllerReader;
    void Update()
    {
        if (inputControllerReader.Steering == -1 || inputControllerReader.Steering == 1) { 
            BhapticsLibrary.Play(eventId: BhapticsEvent.HANDS_VIBRATION);
            Debug.Log("–”À‹ ”œ®–—ﬂ!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        }

    }
}
