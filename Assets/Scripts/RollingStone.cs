using UnityEngine;

public class RollingStone : MonoBehaviour
{
    public float destroyDelay = 10f;

    private AudioSource _audioSource;
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Invoke("DestroyStone", destroyDelay);
    }

    private void Update()
    {
        if (_rigidbody.linearVelocity.magnitude > 0.5f) 
        {
            if (!_audioSource.isPlaying)
            {
                _audioSource.Play();
            }
        }
        else
        {
            _audioSource.Stop();
        }
    }

    private void DestroyStone()
    {
        Destroy(gameObject);
    }
}
