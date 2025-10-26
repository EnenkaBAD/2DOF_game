using LogitechG29.Sample.Input;
using UnityEngine;

public class ShootPrefab : MonoBehaviour
{
    // Публичные переменные для настройки
    public Transform shootPoint;    // Точка выстрела
    public GameObject bulletPrefab; // Префаб пули/снаряда
    public float speed = 10f;       // Скорость выстрела
    public InputControllerReader inputControllerReader;

    private float cooldown = 0f;    // Время перезарядки
    private float fireRate = 0.2f;  // Интервал между выстрелами


    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, что это игрок и можно стрелять
        if (other.CompareTag("Player") && cooldown <= 0)
        {
            Shoot();
            cooldown = fireRate; // Устанавливаем время перезарядки
        }
    }
    void Update()
    {
        // Уменьшаем время перезарядки
        if (cooldown > 0)
            cooldown -= Time.deltaTime;
    }

    void Shoot()
    {
        // Создаём экземпляр префаба в точке выстрела
        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);

        // Добавляем силу для движения
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Для 3D
            rb.AddForce(shootPoint.forward * speed, ForceMode.Impulse);
        }
    }
}

