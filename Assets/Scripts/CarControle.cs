using System.Collections;
using LogitechG29.Sample.Input;
using UnityEngine;

public class G29CarController : MonoBehaviour
{
    [Header("Car Settings")]
    public float maxMotorTorque = 1500f;
    public float maxSteeringAngle = 30f;
    public float brakeForce = 3000f;

    [Header("Wheel Colliders")]
    public WheelCollider frontLeftWheel;
    public WheelCollider frontRightWheel;
    public WheelCollider rearLeftWheel;
    public WheelCollider rearRightWheel;

    [Header("Input Controller")]
    public InputControllerReader inputControllerReader;

    // Переменные управления
    private float steeringInput;
    private float throttleInput;
    private float brakeInput;
    private float clutchInput;
    private bool handbrakeInput;

    // Коробка передач
    private int currentGear = 1; // Начинаем с 1 передачи
    private bool engineRunning = true;

    private Rigidbody carRigidbody;

    void Start()
    {
        carRigidbody = GetComponent<Rigidbody>();

        // Настройка физики
        if (carRigidbody.mass < 1000f)
            carRigidbody.mass = 1200f;

        SetupWheelColliders();

        if (inputControllerReader != null)
        {
            Debug.Log("InputControllerReader подключен успешно");
        }
        else
        {
            Debug.LogError("InputControllerReader не назначен!");
        }
    }

    void SetupWheelColliders()
    {
        // Настройка колес для лучшего сцепления
        ConfigureWheelFriction(rearLeftWheel);
        ConfigureWheelFriction(rearRightWheel);
        ConfigureWheelFriction(frontLeftWheel);
        ConfigureWheelFriction(frontRightWheel);
    }

    void ConfigureWheelFriction(WheelCollider wheel)
    {
        WheelFrictionCurve forwardFriction = wheel.forwardFriction;
        forwardFriction.stiffness = 2.0f;
        wheel.forwardFriction = forwardFriction;

        WheelFrictionCurve sidewaysFriction = wheel.sidewaysFriction;
        sidewaysFriction.stiffness = 1.5f;
        wheel.sidewaysFriction = sidewaysFriction;
    }

    void Update()
    {
        if (inputControllerReader != null)
        {
            GetControllerInput();
            HandleGearShifting();
        }

        // Простая проверка ввода с клавиатуры для тестирования
        HandleKeyboardInput();
    }

    void FixedUpdate()
    {
        ApplySteering();
        ApplyMotorTorque();
        ApplyBrakes();
        ApplyHandbrake();
    }

    void GetControllerInput()
    {
        // Основные оси управления
        steeringInput = inputControllerReader.Steering;
        throttleInput = inputControllerReader.Throttle;
        brakeInput = inputControllerReader.Brake;
        clutchInput = inputControllerReader.Clutch;

        // Кнопки
        handbrakeInput = inputControllerReader.EastButton;

        // Дебаг ввода
        if (Time.frameCount % 60 == 0)
        {
            Debug.Log($"Ввод: Газ={throttleInput:F2}, Тормоз={brakeInput:F2}, Сцепление={clutchInput:F2}, Руление={steeringInput:F2}");
        }
    }

    void HandleKeyboardInput()
    {
        // Резервное управление с клавиатуры для тестирования
        if (Input.GetKey(KeyCode.UpArrow)) throttleInput = Mathf.Max(throttleInput, 0.5f);
        if (Input.GetKey(KeyCode.DownArrow)) brakeInput = Mathf.Max(brakeInput, 0.5f);
        if (Input.GetKey(KeyCode.LeftArrow)) steeringInput = -0.5f;
        if (Input.GetKey(KeyCode.RightArrow)) steeringInput = 0.5f;
        if (Input.GetKeyDown(KeyCode.H)) handbrakeInput = !handbrakeInput;
    }

    void HandleGearShifting()
    {
        // Простое переключение передач
        if (inputControllerReader.Clutch == 0) return;
        if (inputControllerReader.Shifter1) currentGear = 1;
        if (inputControllerReader.Shifter2) currentGear = 2;
        if (inputControllerReader.Shifter3) currentGear = -1; // Задняя
        if (inputControllerReader.Shifter4 && !handbrakeInput) currentGear = 0; // Нейтраль
    }

    void ApplySteering()
    {
        float steeringAngle = steeringInput * maxSteeringAngle;
        frontLeftWheel.steerAngle = steeringAngle;
        frontRightWheel.steerAngle = steeringAngle;
    }


    void ApplyMotorTorque()
    {
        // ПРОСТАЯ ЛОГИКА: если не нейтраль и не задняя - едем вперед
        if (currentGear == 0)
        {
            // Нейтраль - нет движения
            SetAllMotorTorque(0f);
            return;
        }

        if (currentGear == -1)
        {
            // Задняя передача
            float reverseTorque = -throttleInput * maxMotorTorque * 0.5f;
            SetAllMotorTorque(reverseTorque);
            return;
        }

        // Передняя передача
        if (currentGear >= 1)
        {
            float motorTorque = throttleInput * maxMotorTorque;

            // Учет сцепления (если сцепление выжато - меньше мощности)
            float clutchEffect = 1f - (clutchInput * 0.8f);
            motorTorque *= clutchEffect;

            SetAllMotorTorque(motorTorque);

            // Дебаг
            if (throttleInput > 0.1f && Time.frameCount % 30 == 0)
            {
                Debug.Log($"Двигатель: передача={currentGear}, крутящий момент={motorTorque:F0}, сцепление={clutchEffect:F2}");
            }
        }
    }

    void SetAllMotorTorque(float torque)
    {
        rearLeftWheel.motorTorque = torque;
        rearRightWheel.motorTorque = torque;
    }

    void ApplyBrakes()
    {
        float brakeTorque = brakeInput * brakeForce;

        // Применяем тормоза ко всем колесам
        frontLeftWheel.brakeTorque = brakeTorque;
        frontRightWheel.brakeTorque = brakeTorque;
        rearLeftWheel.brakeTorque = brakeTorque;
        rearRightWheel.brakeTorque = brakeTorque;

        // Если тормозим - снимаем мощность
        if (brakeInput > 0.2f)
        {
            rearLeftWheel.motorTorque = 0f;
            rearRightWheel.motorTorque = 0f;
        }
    }

    void ApplyHandbrake()
    {
        if (handbrakeInput)
        {
            // Ручной тормоз только на задние колеса
            rearLeftWheel.brakeTorque = brakeForce;
            rearRightWheel.brakeTorque = brakeForce;
            rearLeftWheel.motorTorque = 0f;
            rearRightWheel.motorTorque = 0f;
        }
    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 400, 400));

        GUILayout.Label("=== ДЕБАГ АВТОМОБИЛЯ ===");
        GUILayout.Label($"Скорость: {carRigidbody.linearVelocity.magnitude * 3.6f:F1} км/ч");
        GUILayout.Label($"Передача: {(currentGear == -1 ? "R" : (currentGear == 0 ? "N" : currentGear.ToString()))}");
        GUILayout.Label($"Газ: {throttleInput:F2}");
        GUILayout.Label($"Тормоз: {brakeInput:F2}");
        GUILayout.Label($"Сцепление: {clutchInput:F2}");
        GUILayout.Label($"Руление: {steeringInput:F2}");
        GUILayout.Label($"Ручной тормоз: {handbrakeInput}");

        GUILayout.Label("");
        GUILayout.Label("=== ИНСТРУКЦИЯ ===");
        GUILayout.Label("1. Убедитесь что передача не N (нейтраль)");
        GUILayout.Label("2. Нажмите газ (правая педаль)");
        GUILayout.Label("3. Отпустите ручной тормоз (East button)");
        GUILayout.Label("");
        GUILayout.Label("Передачи: North=1, South=2, West=R, East=N");
        GUILayout.Label("Клавиши: Стрелки - управление, H - ручник");

        // Информация о колесах
        GUILayout.Label("");
        GUILayout.Label("=== СОСТОЯНИЕ КОЛЕС ===");
        GUILayout.Label($"Задние колеса: motorTorque={rearLeftWheel.motorTorque:F0}");

        GUILayout.EndArea();
    }

    // Метод для принудительной проверки
    public void ForceMoveTest()
    {
        Debug.Log("Принудительный тест движения!");
        currentGear = 1;
        throttleInput = 0.5f;
        handbrakeInput = false;

        // Принудительно применяем мощность
        rearLeftWheel.motorTorque = maxMotorTorque * 0.5f;
        rearRightWheel.motorTorque = maxMotorTorque * 0.5f;
    }

    // Вызывайте этот метод через консоль для тестирования
    void OnEnable()
    {
        Debug.Log("CarController активирован. Для теста в консоли напишите: FindObjectOfType<G29CarController>().ForceMoveTest();");
    }
}
