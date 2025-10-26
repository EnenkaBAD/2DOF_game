using UnityEngine;

public class TreeFall : MonoBehaviour
{
    public Rigidbody _rigidbody;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && _rigidbody != null)
        {
            _rigidbody.isKinematic = false;
        }
    }
}
