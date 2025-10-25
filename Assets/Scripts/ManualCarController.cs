using UnityEngine;
using LogitechG29.Sample.Input;

[RequireComponent(typeof(Rigidbody))]
public class RealisticManualCarController : MonoBehaviour
{
    [Header("Car Settings")]
    public float maxMotorTorque = 1800f;
    public float maxSteeringAngle = 30f;
    public float brakeForce = 3000f;

    [Header("Transmission Settings")]
    public float[] gearRatios = { 3.6f, 2.2f, 1.5f, 1.1f, 0.9f }; // 1-5
    public float reverseGearRatio = 3.6f; // задний ход
    public float finalDrive = 3.42f;
    public float clutchEngageSpeed = 5f; // как быстро схватывает сцепление
    public float stallRpmThreshold = 500f;

    [Header("Engine Settings")]
    public float idleRPM = 800f;
    public float maxRPM = 7000f;
    public float engineTorque = 400f;

    [Header("Physics")]
    public float airResistance = 0.5f;
    public float rollingResistance = 0.1f;
    public float engineBraking = 1.5f;

    [Header("Wheels")]
    public WheelCollider frontLeft;
    public WheelCollider frontRight;
    public WheelCollider rearLeft;
    public WheelCollider rearRight;

    [Header("Input")]
    public InputControllerReader inputControllerReader;

    private Rigidbody rb;

    private int currentGear = 0; // 0 = нейтраль, 1–5 = вперёд, -1 = задняя
    private float engineRpm;
    private bool engineRunning = true;
    private bool clutchPressed;

    private float throttle;
    private float brake;
    private float clutch;
    private float steering;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb.mass < 1000) rb.mass = 1200;
    }

    void Update()
    {
        ReadInput();
        HandleGearShift();
        CalculateEngineRPM();
        HandleStall();
    }

    void FixedUpdate()
    {
        ApplySteering();
        ApplyDrive();
        ApplyBrakes();
        ApplyResistance();
    }

    void ReadInput()
    {
        throttle = Mathf.Clamp01(inputControllerReader.Throttle);
        brake = Mathf.Clamp01(inputControllerReader.Brake);
        clutch = Mathf.Clamp01(inputControllerReader.Clutch);
        clutchPressed = clutch > 0.8f;
        steering = Mathf.Clamp(inputControllerReader.Steering, -1f, 1f);

        // Поддержка клавиатуры для теста
        //if (Input.GetKey(KeyCode.UpArrow)) throttle = Mathf.Max(throttle, 0.5f);
        //if (Input.GetKey(KeyCode.DownArrow)) brake = Mathf.Max(brake, 0.5f);
        //if (Input.GetKey(KeyCode.LeftArrow)) steering = -0.5f;
        //if (Input.GetKey(KeyCode.RightArrow)) steering = 0.5f;
    }

    void HandleGearShift()
    {
        if (clutchPressed)
        {
            if (inputControllerReader.Shifter1) currentGear = 1;
            else if (inputControllerReader.Shifter2) currentGear = 2;
            else if (inputControllerReader.Shifter3) currentGear = 3;
            else if (inputControllerReader.Shifter4) currentGear = 4;
            else if (inputControllerReader.Shifter5) currentGear = 5;
            else if (inputControllerReader.Shifter6) currentGear = -1; // R
            else currentGear = 0; // нейтраль
        }
    }

    void CalculateEngineRPM()
    {
        float speed = rb.linearVelocity.magnitude; // м/с
        float wheelRPM = speed / (2f * Mathf.PI * rearLeft.radius) * 60f;

        if (currentGear == 0 || clutchPressed)
        {
            // сцепление выжато или нейтраль → RPM от газа
            engineRpm = Mathf.Lerp(engineRpm, idleRPM + throttle * (maxRPM - idleRPM), Time.deltaTime * 5f);
        }
        else
        {
            // сцепление отпущено — двигатель связан с колесами
            float gearRatio = (currentGear == -1) ? reverseGearRatio : gearRatios[currentGear - 1];
            float targetRpm = wheelRPM * gearRatio * finalDrive;
            float slip = Mathf.Clamp01(1f - clutch);
            engineRpm = Mathf.Lerp(engineRpm, targetRpm, Time.deltaTime * clutchEngageSpeed * slip);
        }

        engineRpm = Mathf.Clamp(engineRpm, 0f, maxRPM);
    }

    void HandleStall()
    {
        // если сцепление отпущено, RPM падает ниже порога — двигатель глохнет
        if (engineRunning && !clutchPressed && currentGear != 0 && throttle < 0.05f && engineRpm < stallRpmThreshold)
        {
            engineRunning = false;
            engineRpm = 0f;
            Debug.Log("🚨 Двигатель заглох");
        }

        // если сцепление выжато или нейтраль и газ — можно «завести»
        if (!engineRunning && clutchPressed && throttle > 0.2f)
        {
            engineRunning = true;
            engineRpm = idleRPM;
            Debug.Log("✅ Двигатель завёлся");
        }
    }

    void ApplySteering()
    {
        float speed = rb.linearVelocity.magnitude;
        float angle = 0f;
        if (speed > 0.5f) angle = steering * maxSteeringAngle;
        frontLeft.steerAngle = angle;
        frontRight.steerAngle = angle;
    }

    void ApplyDrive()
    {
        if (!engineRunning || clutchPressed || currentGear == 0)
        {
            SetMotorTorque(0f);
            return;
        }

        float torque = 0f;
        if (currentGear == -1)
        {
            torque = -throttle * maxMotorTorque * reverseGearRatio;
        }
        else
        {
            float gearRatio = gearRatios[currentGear - 1];
            float clutchFactor = 1f - clutch; // чем меньше сцепление выжато, тем больше момент
            torque = throttle * maxMotorTorque * gearRatio * finalDrive * clutchFactor;
        }

        SetMotorTorque(torque);
    }

    void SetMotorTorque(float torque)
    {
        rearLeft.motorTorque = torque;
        rearRight.motorTorque = torque;
    }

    void ApplyBrakes()
    {
        float brakeTorque = brake * brakeForce;
        frontLeft.brakeTorque = brakeTorque;
        frontRight.brakeTorque = brakeTorque;
        rearLeft.brakeTorque = brakeTorque;
        rearRight.brakeTorque = brakeTorque;
    }

    void ApplyResistance()
    {
        float speed = rb.linearVelocity.magnitude;
        Vector3 dir = -rb.linearVelocity.normalized;

        float airDrag = airResistance * speed * speed;
        float rollingDrag = rollingResistance * rb.mass * 9.81f;
        rb.AddForce(dir * (airDrag + rollingDrag));

        // Торможение двигателем при отпускании газа
        if (engineRunning && currentGear != 0 && !clutchPressed && throttle < 0.1f)
        {
            float brakeTorque = engineBraking * brakeForce * 0.05f;
            rearLeft.brakeTorque += brakeTorque;
            rearRight.brakeTorque += brakeTorque;
        }
    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 400, 400));

        GUILayout.Label("=== РЕАЛИСТИЧНАЯ МКПП ===");
        GUILayout.Label($"Скорость: {(rb.linearVelocity.magnitude * 3.6f):F1} км/ч");
        GUILayout.Label($"Обороты: {engineRpm:F0} RPM");
        GUILayout.Label($"Передача: {GetGearName(currentGear)}");
        GUILayout.Label($"Газ: {(throttle * 100):F0}%");
        GUILayout.Label($"Тормоз: {(brake * 100):F0}%");
        GUILayout.Label($"Сцепление: {(clutch * 100):F0}%");
        GUILayout.Label($"Двигатель: {(engineRunning ? "Работает" : "Заглох")}");

        GUILayout.EndArea();
    }

    string GetGearName(int gear)
    {
        if (gear == 0) return "N";
        if (gear == -1) return "R";
        return gear.ToString();
    }
}
