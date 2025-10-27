using LogitechG29.Sample.Input;
using UnityEngine;

public class ShootPrefab : MonoBehaviour
{
    public Transform shootPoint;    
    public GameObject bulletPrefab; 
    public float speed = 10f;       



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // Создаём экземпляр префаба в точке выстрела
        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);

        // Добавляем силу для движения
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.AddForce(shootPoint.forward * speed, ForceMode.Impulse);
    }
}

