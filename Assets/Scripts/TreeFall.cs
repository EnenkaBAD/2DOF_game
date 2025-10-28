using UnityEngine;

public class TreeFall : MonoBehaviour
{
    public Rigidbody _rigidbody;
    [SerializeField]
    private AudioSource _audioSource;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && _rigidbody != null)
        {
            _rigidbody.isKinematic = false;
            _audioSource.Play();
        }
    }
}
