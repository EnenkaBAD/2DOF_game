using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    [SerializeField] private TMP_Text countdownText; 
    [SerializeField] private AudioClip tickSound;  
    [SerializeField] private AudioClip goSound;
    [SerializeField] private GameObject stopWall;


    [SerializeField] private float startDelay = 0.5f; 
    [SerializeField] private float tickDuration = 1f;  

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();


        Invoke("StartCountdown", startDelay);
    }

    private void StartCountdown()
    {
        StartCoroutine(RunCountdown());
    }

    private IEnumerator RunCountdown()
    {
        countdownText.text = "3";
        PlayTickSound();
        yield return new WaitForSeconds(tickDuration);

        countdownText.text = "2";
        PlayTickSound();
        yield return new WaitForSeconds(tickDuration);

        countdownText.text = "1";
        PlayTickSound();
        yield return new WaitForSeconds(tickDuration);

        countdownText.text = "Поехали!";
        PlayGoSound();
        GameObject.Destroy(stopWall);
        yield return new WaitForSeconds(tickDuration);
        
        countdownText.text = "";

        // Здесь можно активировать игровые объекты, запустить таймер и т. п.
        // Например: GameManager.Instance.StartGame();
    }

    private void PlayTickSound()
    {
        if (tickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(tickSound);
        }
    }

    private void PlayGoSound()
    {
        if (goSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(goSound);
        }
    }
}
