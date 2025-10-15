//using System.Collections;
//using LogitechG29.Sample.Input;
//using UnityEngine;

//public class G29CarController : MonoBehaviour
//{
//    [Header("Car Settings")]
//    public float maxMotorTorque = 1500f;
//    public float maxSteeringAngle = 30f;
//    public float brakeForce = 3000f;

//    [Header("Wheel Colliders")]
//    public WheelCollider frontLeftWheel;
//    public WheelCollider frontRightWheel;
//    public WheelCollider rearLeftWheel;
//    public WheelCollider rearRightWheel;

//    [Header("Input Controller")]
//    public InputControllerReader inputControllerReader;

//    // Переменные управления
//    private float steeringInput;
//    private float throttleInput;
//    private float brakeInput;
//    private float clutchInput;
//    private bool handbrakeInput;

//    // Коробка передач
//    private int currentGear = 1; // Начинаем с 1 передачи
//    private bool engineRunning = true;

//    private Rigidbody carRigidbody;

//    void Start()
//    {
//        carRigidbody = GetComponent<Rigidbody>();

//        // Настройка физики
//        if (carRigidbody.mass < 1000f)
//            carRigidbody.mass = 1200f;

//        SetupWheelColliders();

//        if (inputControllerReader != null)
//        {
//            Debug.Log("InputControllerReader подключен успешно");
//        }
//        else
//        {
//            Debug.LogError("InputControllerReader не назначен!");
//        }
//    }

//    void SetupWheelColliders()
//    {
//        // Настройка колес для лучшего сцепления
//        ConfigureWheelFriction(rearLeftWheel);
//        ConfigureWheelFriction(rearRightWheel);
//        ConfigureWheelFriction(frontLeftWheel);
//        ConfigureWheelFriction(frontRightWheel);
//    }

//    void ConfigureWheelFriction(WheelCollider wheel)
//    {
//        WheelFrictionCurve forwardFriction = wheel.forwardFriction;
//        forwardFriction.stiffness = 2.0f;
//        wheel.forwardFriction = forwardFriction;

//        WheelFrictionCurve sidewaysFriction = wheel.sidewaysFriction;
//        sidewaysFriction.stiffness = 1.5f;
//        wheel.sidewaysFriction = sidewaysFriction;
//    }

//    void Update()
//    {
//        if (inputControllerReader != null)
//        {
//            GetControllerInput();
//            HandleGearShifting();
//        }

//        // Простая проверка ввода с клавиатуры для тестирования
//        HandleKeyboardInput();
//    }

//    void FixedUpdate()
//    {
//        ApplySteering();
//        ApplyMotorTorque();
//        ApplyBrakes();
//        ApplyHandbrake();
//    }

//    void GetControllerInput()
//    {
//        // Основные оси управления
//        steeringInput = inputControllerReader.Steering;
//        throttleInput = inputControllerReader.Throttle;
//        brakeInput = inputControllerReader.Brake;
//        clutchInput = inputControllerReader.Clutch;

//        // Кнопки
//        handbrakeInput = inputControllerReader.EastButton;

//        // Дебаг ввода
//        if (Time.frameCount % 60 == 0)
//        {
//            Debug.Log($"Ввод: Газ={throttleInput:F2}, Тормоз={brakeInput:F2}, Сцепление={clutchInput:F2}, Руление={steeringInput:F2}");
//        }
//    }

//    void HandleKeyboardInput()
//    {
//        // Резервное управление с клавиатуры для тестирования
//        if (Input.GetKey(KeyCode.UpArrow)) throttleInput = Mathf.Max(throttleInput, 0.5f);
//        if (Input.GetKey(KeyCode.DownArrow)) brakeInput = Mathf.Max(brakeInput, 0.5f);
//        if (Input.GetKey(KeyCode.LeftArrow)) steeringInput = -0.5f;
//        if (Input.GetKey(KeyCode.RightArrow)) steeringInput = 0.5f;
//        if (Input.GetKeyDown(KeyCode.H)) handbrakeInput = !handbrakeInput;
//    }

//    void HandleGearShifting()
//    {
//        // Простое переключение передач
//        if (inputControllerReader.Clutch == 0) return;
//        if (inputControllerReader.Shifter1) currentGear = 1;
//        if (inputControllerReader.Shifter2) currentGear = 2;
//        if (inputControllerReader.Shifter3) currentGear = -1; // Задняя
//        if (inputControllerReader.Shifter4 && !handbrakeInput) currentGear = 0; // Нейтраль
//    }

//    void ApplySteering()
//    {
//        float steeringAngle = steeringInput * maxSteeringAngle;
//        frontLeftWheel.steerAngle = steeringAngle;
//        frontRightWheel.steerAngle = steeringAngle;
//    }


//    void ApplyMotorTorque()
//    {
//        // ПРОСТАЯ ЛОГИКА: если не нейтраль и не задняя - едем вперед
//        if (currentGear == 0)
//        {
//            // Нейтраль - нет движения
//            SetAllMotorTorque(0f);
//            return;
//        }

//        if (currentGear == -1)
//        {
//            // Задняя передача
//            float reverseTorque = -throttleInput * maxMotorTorque * 0.5f;
//            SetAllMotorTorque(reverseTorque);
//            return;
//        }

//        // Передняя передача
//        if (currentGear >= 1)
//        {
//            float motorTorque = throttleInput * maxMotorTorque;

//            // Учет сцепления (если сцепление выжато - меньше мощности)
//            float clutchEffect = 1f - (clutchInput * 0.8f);
//            motorTorque *= clutchEffect;

//            SetAllMotorTorque(motorTorque);

//            // Дебаг
//            if (throttleInput > 0.1f && Time.frameCount % 30 == 0)
//            {
//                Debug.Log($"Двигатель: передача={currentGear}, крутящий момент={motorTorque:F0}, сцепление={clutchEffect:F2}");
//            }
//        }
//    }

//    void SetAllMotorTorque(float torque)
//    {
//        rearLeftWheel.motorTorque = torque;
//        rearRightWheel.motorTorque = torque;
//    }

//    void ApplyBrakes()
//    {
//        float brakeTorque = brakeInput * brakeForce;

//        // Применяем тормоза ко всем колесам
//        frontLeftWheel.brakeTorque = brakeTorque;
//        frontRightWheel.brakeTorque = brakeTorque;
//        rearLeftWheel.brakeTorque = brakeTorque;
//        rearRightWheel.brakeTorque = brakeTorque;

//        // Если тормозим - снимаем мощность
//        if (brakeInput > 0.2f)
//        {
//            rearLeftWheel.motorTorque = 0f;
//            rearRightWheel.motorTorque = 0f;
//        }
//    }

//    void ApplyHandbrake()
//    {
//        if (handbrakeInput)
//        {
//            // Ручной тормоз только на задние колеса
//            rearLeftWheel.brakeTorque = brakeForce;
//            rearRightWheel.brakeTorque = brakeForce;
//            rearLeftWheel.motorTorque = 0f;
//            rearRightWheel.motorTorque = 0f;
//        }
//    }

//    void OnGUI()
//    {
//        GUILayout.BeginArea(new Rect(10, 10, 400, 400));

//        GUILayout.Label("=== ДЕБАГ АВТОМОБИЛЯ ===");
//        GUILayout.Label($"Скорость: {carRigidbody.linearVelocity.magnitude * 3.6f:F1} км/ч");
//        GUILayout.Label($"Передача: {(currentGear == -1 ? "R" : (currentGear == 0 ? "N" : currentGear.ToString()))}");
//        GUILayout.Label($"Газ: {throttleInput:F2}");
//        GUILayout.Label($"Тормоз: {brakeInput:F2}");
//        GUILayout.Label($"Сцепление: {clutchInput:F2}");
//        GUILayout.Label($"Руление: {steeringInput:F2}");
//        GUILayout.Label($"Ручной тормоз: {handbrakeInput}");

//        GUILayout.Label("");
//        GUILayout.Label("=== ИНСТРУКЦИЯ ===");
//        GUILayout.Label("1. Убедитесь что передача не N (нейтраль)");
//        GUILayout.Label("2. Нажмите газ (правая педаль)");
//        GUILayout.Label("3. Отпустите ручной тормоз (East button)");
//        GUILayout.Label("");
//        GUILayout.Label("Передачи: North=1, South=2, West=R, East=N");
//        GUILayout.Label("Клавиши: Стрелки - управление, H - ручник");

//        // Информация о колесах
//        GUILayout.Label("");
//        GUILayout.Label("=== СОСТОЯНИЕ КОЛЕС ===");
//        GUILayout.Label($"Задние колеса: motorTorque={rearLeftWheel.motorTorque:F0}");

//        GUILayout.EndArea();
//    }

//    // Метод для принудительной проверки
//    public void ForceMoveTest()
//    {
//        Debug.Log("Принудительный тест движения!");
//        currentGear = 1;
//        throttleInput = 0.5f;
//        handbrakeInput = false;

//        // Принудительно применяем мощность
//        rearLeftWheel.motorTorque = maxMotorTorque * 0.5f;
//        rearRightWheel.motorTorque = maxMotorTorque * 0.5f;
//    }

//    // Вызывайте этот метод через консоль для тестирования
//    void OnEnable()
//    {
//        Debug.Log("CarController активирован. Для теста в консоли напишите: FindObjectOfType<G29CarController>().ForceMoveTest();");
//    }
//}



////2
//using UnityEngine;
//using System.Collections;
//using LogitechG29.Sample.Input;

//public class G29CarController : MonoBehaviour
//{
//    [Header("Car Settings")]
//    public float maxMotorTorque = 1500f;
//    public float maxSteeringAngle = 30f;
//    public float brakeForce = 3000f;

//    [Header("Transmission Settings")]
//    public float[] gearRatios = { 3.67f, 2.50f, 1.80f, 1.40f, 1.00f, 0.80f };
//    public float finalDriveRatio = 3.42f;
//    public float reverseGearRatio = 3.67f;
//    public float shiftDelay = 0.1f;

//    [Header("Physics Settings")]
//    public float airResistance = 0.5f;
//    public float rollingResistance = 0.1f;
//    public float engineBrakingForce = 2.0f;
//    public float idleRPM = 800f;
//    public float maxRPM = 7000f;

//    [Header("Wheel Colliders")]
//    public WheelCollider frontLeftWheel;
//    public WheelCollider frontRightWheel;
//    public WheelCollider rearLeftWheel;
//    public WheelCollider rearRightWheel;

//    [Header("Input Controller")]
//    public InputControllerReader inputControllerReader;

//    // Переменные управления
//    private float steeringInput;
//    private float throttleInput;
//    private float brakeInput;
//    private float clutchInput;
//    private bool handbrakeInput;

//    // Коробка передач
//    private int currentGear = 1;
//    private bool isShifting = false;
//    private float shiftTimer = 0f;
//    private float engineRPM;
//    private float carSpeed;

//    private Rigidbody carRigidbody;

//    void Start()
//    {
//        carRigidbody = GetComponent<Rigidbody>();

//        if (carRigidbody.mass < 1000f)
//            carRigidbody.mass = 1200f;

//        SetupWheelColliders();

//        if (inputControllerReader != null)
//        {
//            Debug.Log("InputControllerReader подключен успешно");
//        }
//        else
//        {
//            Debug.LogError("InputControllerReader не назначен!");
//        }
//    }

//    void SetupWheelColliders()
//    {
//        ConfigureWheelFriction(rearLeftWheel);
//        ConfigureWheelFriction(rearRightWheel);
//        ConfigureWheelFriction(frontLeftWheel);
//        ConfigureWheelFriction(frontRightWheel);
//    }

//    void ConfigureWheelFriction(WheelCollider wheel)
//    {
//        // Увеличиваем трение качения
//        WheelFrictionCurve forwardFriction = wheel.forwardFriction;
//        forwardFriction.stiffness = 2.5f;
//        forwardFriction.extremumSlip = 0.4f;
//        forwardFriction.extremumValue = 1.0f;
//        forwardFriction.asymptoteSlip = 0.8f;
//        forwardFriction.asymptoteValue = 0.5f;
//        wheel.forwardFriction = forwardFriction;

//        WheelFrictionCurve sidewaysFriction = wheel.sidewaysFriction;
//        sidewaysFriction.stiffness = 2.0f;
//        sidewaysFriction.extremumSlip = 0.2f;
//        sidewaysFriction.extremumValue = 1.0f;
//        sidewaysFriction.asymptoteSlip = 0.5f;
//        sidewaysFriction.asymptoteValue = 0.75f;
//        wheel.sidewaysFriction = sidewaysFriction;
//    }

//    void Update()
//    {
//        if (inputControllerReader != null)
//        {
//            GetControllerInput();
//            HandleGearShifting();
//            CalculateRPMAndSpeed();
//        }

//        if (isShifting)
//        {
//            shiftTimer += Time.deltaTime;
//            if (shiftTimer >= shiftDelay)
//            {
//                isShifting = false;
//                shiftTimer = 0f;
//            }
//        }

//        HandleKeyboardInput();
//        ApplyResistanceForces();
//    }

//    void FixedUpdate()
//    {
//        ApplySteering();
//        ApplyMotorTorque();
//        ApplyBrakes();
//        ApplyHandbrake();
//        ApplyEngineBraking();
//    }

//    void GetControllerInput()
//    {
//        steeringInput = inputControllerReader.Steering;
//        throttleInput = inputControllerReader.Throttle;
//        brakeInput = inputControllerReader.Brake;
//        clutchInput = inputControllerReader.Clutch;
//        handbrakeInput = inputControllerReader.EastButton;
//    }

//    void HandleKeyboardInput()
//    {
//        if (Input.GetKey(KeyCode.UpArrow)) throttleInput = Mathf.Max(throttleInput, 0.5f);
//        if (Input.GetKey(KeyCode.DownArrow)) brakeInput = Mathf.Max(brakeInput, 0.5f);
//        if (Input.GetKey(KeyCode.LeftArrow)) steeringInput = -0.5f;
//        if (Input.GetKey(KeyCode.RightArrow)) steeringInput = 0.5f;
//        if (Input.GetKeyDown(KeyCode.H)) handbrakeInput = !handbrakeInput;

//        if (Input.GetKeyDown(KeyCode.Alpha1)) ShiftToGear(1);
//        if (Input.GetKeyDown(KeyCode.Alpha2)) ShiftToGear(2);
//        if (Input.GetKeyDown(KeyCode.Alpha3)) ShiftToGear(3);
//        if (Input.GetKeyDown(KeyCode.Alpha4)) ShiftToGear(4);
//        if (Input.GetKeyDown(KeyCode.Alpha5)) ShiftToGear(5);
//        if (Input.GetKeyDown(KeyCode.Alpha6)) ShiftToGear(6);
//        if (Input.GetKeyDown(KeyCode.R)) ShiftToGear(-1);
//        if (Input.GetKeyDown(KeyCode.N)) ShiftToGear(0);
//    }

//    void HandleGearShifting()
//    {
//        if (clutchInput < 0.3f && !isShifting) return;

//        if (inputControllerReader.Shifter1)
//        {
//            ShiftToGear(1);
//        }
//        else if (inputControllerReader.Shifter2)
//        {
//            ShiftToGear(2);
//        }
//        else if (inputControllerReader.Shifter3)
//        {
//            ShiftToGear(3);
//        }
//        else if (inputControllerReader.Shifter4)
//        {
//            ShiftToGear(4);
//        }
//        //else if (inputControllerReader.RightShoulder)
//        //{
//        //    ShiftToGear(5);
//        //}
//        //else if (inputControllerReader.LeftShoulder)
//        //{
//        //    ShiftToGear(6);
//        //}
//        //else if (inputControllerReader.RightTrigger)
//        //{
//        //    ShiftToGear(-1);
//        //}
//        //else if (inputControllerReader.LeftTrigger)
//        //{
//        //    ShiftToGear(0);
//        //}
//    }

//    void ShiftToGear(int newGear)
//    {
//        if (isShifting) return;
//        if (newGear < -1 || newGear > 6) return;

//        if (newGear == -1 && carSpeed > 3f)
//        {
//            Debug.LogWarning("Нельзя включить заднюю передачу на скорости!");
//            return;
//        }

//        isShifting = true;
//        int oldGear = currentGear;
//        currentGear = newGear;

//        Debug.Log($"Переключение с {GetGearName(oldGear)} на {GetGearName(currentGear)}");
//    }

//    void CalculateRPMAndSpeed()
//    {
//        carSpeed = carRigidbody.linearVelocity.magnitude * 3.6f;

//        if (currentGear != 0 && clutchInput < 0.2f && !isShifting)
//        {
//            float wheelRPM = Mathf.Abs(rearLeftWheel.rpm);
//            float gearRatio = currentGear == -1 ? reverseGearRatio : gearRatios[currentGear - 1];
//            float calculatedRPM = wheelRPM * gearRatio * finalDriveRatio;

//            // Плавное изменение RPM
//            engineRPM = Mathf.Lerp(engineRPM, calculatedRPM, Time.deltaTime * 5f);
//            engineRPM = Mathf.Clamp(engineRPM, idleRPM, maxRPM);
//        }
//        else
//        {
//            float targetRPM = idleRPM + (throttleInput * 2000f);
//            engineRPM = Mathf.Lerp(engineRPM, targetRPM, Time.deltaTime * 3f);
//        }
//    }

//    void ApplyResistanceForces()
//    {
//        // Сопротивление воздуха (пропорционально квадрату скорости)
//        float speed = carRigidbody.linearVelocity.magnitude;
//        float airDragForce = airResistance * speed * speed;
//        Vector3 airDrag = -carRigidbody.linearVelocity.normalized * airDragForce;
//        carRigidbody.AddForce(airDrag);

//        // Сопротивление качению
//        float rollingDragForce = rollingResistance * carRigidbody.mass * 9.81f;
//        Vector3 rollingDrag = -carRigidbody.linearVelocity.normalized * rollingDragForce;
//        carRigidbody.AddForce(rollingDrag);

//        // Внутреннее трение в трансмиссии
//        if (currentGear != 0 && clutchInput < 0.1f)
//        {
//            float transmissionDrag = 0.1f * carSpeed;
//            Vector3 transmissionDragForce = -carRigidbody.linearVelocity.normalized * transmissionDrag;
//            carRigidbody.AddForce(transmissionDragForce);
//        }
//    }

//    void ApplyEngineBraking()
//    {
//        // Торможение двигателем когда газ отпущен и передача включена
//        if (currentGear > 0 && throttleInput < 0.1f && clutchInput < 0.1f && !isShifting && carSpeed > 1f)
//        {
//            // Сила торможения зависит от передачи (сильнее на низких передачах)
//            float engineBrakePower = engineBrakingForce * (gearRatios[currentGear - 1] / gearRatios[0]);

//            // Применяем тормозной момент к двигательным колесам
//            float brakeTorque = engineBrakePower * brakeForce * 0.1f;
//            rearLeftWheel.brakeTorque = brakeTorque;
//            rearRightWheel.brakeTorque = brakeTorque;

//            // Также добавляем сопротивление через физику
//            float resistance = engineBrakePower * carSpeed * 10f;
//            Vector3 engineBrakeForce = -carRigidbody.linearVelocity.normalized * resistance;
//            carRigidbody.AddForce(engineBrakeForce);
//        }
//        else
//        {
//            // Снимаем торможение двигателем когда оно не нужно
//            if (brakeInput < 0.1f && !handbrakeInput)
//            {
//                rearLeftWheel.brakeTorque = 0f;
//                rearRightWheel.brakeTorque = 0f;
//            }
//        }
//    }

//    void ApplySteering()
//    {
//        float steeringAngle = steeringInput * maxSteeringAngle;
//        frontLeftWheel.steerAngle = steeringAngle;
//        frontRightWheel.steerAngle = steeringAngle;
//    }

//    void ApplyMotorTorque()
//    {
//        if (currentGear == 0 || isShifting || clutchInput > 0.8f)
//        {
//            SetAllMotorTorque(0f);
//            return;
//        }

//        float motorTorque = 0f;

//        if (currentGear == -1)
//        {
//            motorTorque = -throttleInput * maxMotorTorque * 0.4f;
//        }
//        else if (currentGear >= 1)
//        {
//            float gearRatio = gearRatios[currentGear - 1];
//            motorTorque = throttleInput * maxMotorTorque * gearRatio * finalDriveRatio;

//            float clutchEffect = 1f - (clutchInput * 0.8f);
//            motorTorque *= clutchEffect;

//            // Ограничение мощности на низких оборотах
//            if (engineRPM < 1500)
//            {
//                motorTorque *= Mathf.Clamp01((engineRPM - idleRPM) / 700f);
//            }

//            // Ограничение мощности на высоких оборотах
//            if (engineRPM > 6000)
//            {
//                motorTorque *= Mathf.Clamp01(1f - ((engineRPM - 6000) / 1000f));
//            }
//        }

//        SetAllMotorTorque(motorTorque);
//    }

//    void SetAllMotorTorque(float torque)
//    {
//        rearLeftWheel.motorTorque = torque;
//        rearRightWheel.motorTorque = torque;
//    }

//    void ApplyBrakes()
//    {
//        float brakeTorque = brakeInput * brakeForce;

//        // Основные тормоза
//        frontLeftWheel.brakeTorque = brakeTorque;
//        frontRightWheel.brakeTorque = brakeTorque;
//        rearLeftWheel.brakeTorque = brakeTorque;
//        rearRightWheel.brakeTorque = brakeTorque;

//        // Если тормозим - снимаем мощность
//        if (brakeInput > 0.2f)
//        {
//            rearLeftWheel.motorTorque = 0f;
//            rearRightWheel.motorTorque = 0f;
//        }
//    }

//    void ApplyHandbrake()
//    {
//        if (handbrakeInput)
//        {
//            rearLeftWheel.brakeTorque = brakeForce * 0.8f;
//            rearRightWheel.brakeTorque = brakeForce * 0.8f;
//            rearLeftWheel.motorTorque = 0f;
//            rearRightWheel.motorTorque = 0f;
//        }
//        else if (brakeInput < 0.1f)
//        {
//            // Снимаем ручной тормоз если не нажаты основные тормоза
//            rearLeftWheel.brakeTorque = 0f;
//            rearRightWheel.brakeTorque = 0f;
//        }
//    }

//    string GetGearName(int gear)
//    {
//        switch (gear)
//        {
//            case -1: return "R (Задняя)";
//            case 0: return "N (Нейтраль)";
//            default: return gear.ToString();
//        }
//    }

//    void OnGUI()
//    {
//        GUILayout.BeginArea(new Rect(10, 10, 450, 550));

//        GUILayout.Label("=== ФИЗИКА АВТОМОБИЛЯ ===");
//        GUILayout.Label($"Скорость: {carSpeed:F1} км/ч");
//        GUILayout.Label($"Обороты: {engineRPM:F0} RPM");
//        GUILayout.Label($"Передача: {GetGearName(currentGear)}");
//        GUILayout.Label($"Газ: {(throttleInput * 100):F0}%");
//        GUILayout.Label($"Тормоз: {(brakeInput * 100):F0}%");
//        GUILayout.Label($"Сцепление: {(clutchInput * 100):F0}%");
//        GUILayout.Label($"Ручной тормоз: {(handbrakeInput ? "ВКЛ" : "ВЫКЛ")}");

//        GUILayout.Label("");
//        GUILayout.Label("=== СОПРОТИВЛЕНИЯ ===");
//        GUILayout.Label($"Сопр. воздуха: {airResistance * carSpeed * carSpeed * 0.01f:F1} N");
//        GUILayout.Label($"Сопр. качения: {rollingResistance * carRigidbody.mass * 9.81f * 0.01f:F1} N");
//        GUILayout.Label($"Торм. двигателем: {(currentGear > 0 && throttleInput < 0.1f ? "АКТИВНО" : "НЕТ")}");

//        GUILayout.Label("");
//        GUILayout.Label("=== СИЛЫ ДЕЙСТВУЮЩИЕ НА АВТО ===");
//        Vector3 localVelocity = transform.InverseTransformDirection(carRigidbody.linearVelocity);
//        GUILayout.Label($"Передняя скорость: {localVelocity.z:F1} м/с");
//        GUILayout.Label($"Боковая скорость: {localVelocity.x:F1} м/с");

//        GUILayout.Label("");
//        GUILayout.Label("=== ТЕСТ ФИЗИКИ ===");
//        GUILayout.Label("1. Разгонитесь и отпустите газ");
//        GUILayout.Label("2. Наблюдайте замедление");
//        GUILayout.Label("3. Попробуйте торможение двигателем");

//        GUILayout.EndArea();
//    }
//}



using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using LogitechG29.Sample.Input;

public class G29CarController : MonoBehaviour
{
    [Header("Car Settings")]
    public float maxMotorTorque = 1500f;
    public float maxSteeringAngle = 30f;
    public float brakeForce = 3000f;

    [Header("Transmission Settings")]
    public float[] gearRatios = { 3.67f, 2.50f, 1.80f, 1.40f, 1.00f, 0.80f };
    public float finalDriveRatio = 3.42f;
    public float reverseGearRatio = 3.67f;
    public float shiftDelay = 0.1f; // Уменьшили задержку

    [Header("Physics Settings")]
    public float airResistance = 0.5f;
    public float rollingResistance = 0.1f;
    public float engineBrakingForce = 2.0f;
    public float idleRPM = 800f;
    public float maxRPM = 7000f;

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
    private int currentGear = 0;
    private bool isShifting = false;
    private float shiftTimer = 0f;
    private float engineRPM;
    private float carSpeed;

    private Rigidbody carRigidbody;

    void Start()
    {
        carRigidbody = GetComponent<Rigidbody>();

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
        ConfigureWheelFriction(rearLeftWheel);
        ConfigureWheelFriction(rearRightWheel);
        ConfigureWheelFriction(frontLeftWheel);
        ConfigureWheelFriction(frontRightWheel);
    }

    void ConfigureWheelFriction(WheelCollider wheel)
    {
        WheelFrictionCurve forwardFriction = wheel.forwardFriction;
        forwardFriction.stiffness = 2.5f;
        wheel.forwardFriction = forwardFriction;

        WheelFrictionCurve sidewaysFriction = wheel.sidewaysFriction;
        sidewaysFriction.stiffness = 2.0f;
        wheel.sidewaysFriction = sidewaysFriction;
    }

    void Update()
    {
        if (inputControllerReader != null)
        {
            GetControllerInput();
            HandleGearShifting();
            CalculateRPMAndSpeed();
        }

        // УПРОЩЕННЫЙ ТАЙМЕР ПЕРЕКЛЮЧЕНИЯ
        if (isShifting)
        {
            shiftTimer += Time.deltaTime;
            if (shiftTimer >= shiftDelay)
            {
                isShifting = false;
                shiftTimer = 0f;
                Debug.Log("Переключение завершено!");
            }
        }

        HandleKeyboardInput();
        ApplyResistanceForces();

        // ДЕБАГ: Показываем состояние передачи мощности
        if (Time.frameCount % 60 == 0)
        {
            DebugPowerStatus();
        }
    }

    void FixedUpdate()
    {
        //ApplySteering();
        ApplyMotorTorque(); // ВАЖНО: вызываем ДО тормозов
        ApplyBrakes();
        ApplyHandbrake();
        ApplyEngineBraking();
    }

    void GetControllerInput()
    {
        steeringInput = inputControllerReader.Steering;
        throttleInput = inputControllerReader.Throttle;
        brakeInput = inputControllerReader.Brake;
        clutchInput = inputControllerReader.Clutch;
        handbrakeInput = inputControllerReader.EastButton;
    }

    void HandleKeyboardInput()
    {
        if (Input.GetKey(KeyCode.UpArrow)) throttleInput = Mathf.Max(throttleInput, 0.5f);
        if (Input.GetKey(KeyCode.DownArrow)) brakeInput = Mathf.Max(brakeInput, 0.5f);
        if (Input.GetKey(KeyCode.LeftArrow)) steeringInput = -0.5f;
        if (Input.GetKey(KeyCode.RightArrow)) steeringInput = 0.5f;
        if (Input.GetKeyDown(KeyCode.H)) handbrakeInput = !handbrakeInput;

        if (Input.GetKeyDown(KeyCode.Alpha1)) ShiftToGear(1);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ShiftToGear(2);
        if (Input.GetKeyDown(KeyCode.Alpha3)) ShiftToGear(3);
        if (Input.GetKeyDown(KeyCode.Alpha4)) ShiftToGear(4);
        if (Input.GetKeyDown(KeyCode.Alpha5)) ShiftToGear(5);
        if (Input.GetKeyDown(KeyCode.Alpha6)) ShiftToGear(6);
        if (Input.GetKeyDown(KeyCode.R)) ShiftToGear(-1);
        if (Input.GetKeyDown(KeyCode.N)) ShiftToGear(0);
    }

    void HandleGearShifting()
    {
        // ПРОСТАЯ ПРОВЕРКА: переключаемся только если сцепление выжато
        if (clutchInput > 0.5f)
        {
            if (inputControllerReader.Shifter1) ShiftToGear(1);
            else if (inputControllerReader.Shifter2) ShiftToGear(2);
            else if (inputControllerReader.Shifter3) ShiftToGear(3);
            else if (inputControllerReader.Shifter4) ShiftToGear(-1);
            //else if (inputControllerReader.RightShoulder) ShiftToGear(5);
            //else if (inputControllerReader.LeftShoulder) ShiftToGear(6);
            //else if (inputControllerReader.RightTrigger) ShiftToGear(-1);
            else ShiftToGear(0);
        }
    }

    void ShiftToGear(int newGear)
    {
        if (isShifting) return;
        if (newGear < -1 || newGear > 6) return;

        if (newGear == -1 && carSpeed > 3f)
        {
            Debug.LogWarning("Нельзя включить заднюю передачу на скорости!");
            return;
        }

        isShifting = true;
        int oldGear = currentGear;
        currentGear = newGear;

        Debug.Log($"Переключение с {GetGearName(oldGear)} на {GetGearName(currentGear)}");
    }

    void CalculateRPMAndSpeed()
    {
        carSpeed = carRigidbody.linearVelocity.magnitude * 3.6f;

        // УПРОЩЕННЫЙ РАСЧЕТ RPM
        if (currentGear != 0 && clutchInput < 0.8f && carSpeed > 0.1f)
        {
            float wheelRPM = Mathf.Max(Mathf.Abs(rearLeftWheel.rpm), 100f);
            float gearRatio = currentGear == -1 ? reverseGearRatio : gearRatios[currentGear - 1];
            engineRPM = wheelRPM * gearRatio * finalDriveRatio;
            engineRPM = Mathf.Clamp(engineRPM, idleRPM, maxRPM);
        }
        else
        {
            // На нейтрали или с выжатым сцеплением
            engineRPM = Mathf.Lerp(engineRPM, idleRPM + (throttleInput * 3000f), Time.deltaTime * 2f);
        }
    }

    void ApplyMotorTorque()
    {
        // ОСНОВНОЕ УСЛОВИЕ: передача не нейтральная И сцепление не выжато
        bool canApplyPower = (currentGear != 0) && (clutchInput < 0.8f) && (!isShifting);

        if (!canApplyPower)
        {
            SetAllMotorTorque(0f);
            return;
        }

        // ПРОВЕРКА РУЧНОГО ТОРМОЗА
        if (handbrakeInput)
        {
            SetAllMotorTorque(0f);
            return;
        }

        float motorTorque = 0f;

        if (currentGear == -1)
        {
            // ЗАДНЯЯ ПЕРЕДАЧА
            motorTorque = -throttleInput * maxMotorTorque * 0.5f;
        }
        else if (currentGear >= 1)
        {
            // ПЕРЕДНИЕ ПЕРЕДАЧИ
            float gearRatio = gearRatios[currentGear - 1];
            motorTorque = throttleInput * maxMotorTorque * gearRatio * finalDriveRatio;

            // ПЛАВНОЕ ПЕРЕДАЧА МОЩНОСТИ ЧЕРЕЗ СЦЕПЛЕНИЕ
            float clutchEffect = 1f - Mathf.Pow(clutchInput, 2f); // Квадрат для более плавного эффекта
            motorTorque *= clutchEffect;

            // ЗАЩИТА ОТ НИЗКИХ ОБОРОТОВ (чтоб не глох)
            if (engineRPM < 1200 && throttleInput < 0.3f)
            {
                motorTorque *= 0.3f;
            }
        }

        SetAllMotorTorque(motorTorque);

        // Дебаг информации
        if (throttleInput > 0.1f && Time.frameCount % 30 == 0)
        {
            Debug.Log($"Мощность: torque={motorTorque:F0}, gear={currentGear}, clutch={clutchInput:F2}, RPM={engineRPM:F0}");
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

        frontLeftWheel.brakeTorque = brakeTorque;
        frontRightWheel.brakeTorque = brakeTorque;
        rearLeftWheel.brakeTorque = brakeTorque;
        rearRightWheel.brakeTorque = brakeTorque;
    }

    void ApplyHandbrake()
    {
        if (handbrakeInput)
        {
            rearLeftWheel.brakeTorque = brakeForce * 0.8f;
            rearRightWheel.brakeTorque = brakeForce * 0.8f;
        }
    }

    void ApplyEngineBraking()
    {
        // Торможение двигателем когда газ отпущен
        if (currentGear > 0 && throttleInput < 0.1f && clutchInput < 0.1f && carSpeed > 1f)
        {
            float engineBrakePower = engineBrakingForce * (gearRatios[currentGear - 1] / gearRatios[0]);
            float brakeTorque = engineBrakePower * brakeForce * 0.05f;
            rearLeftWheel.brakeTorque += brakeTorque;
            rearRightWheel.brakeTorque += brakeTorque;
        }
    }

    void ApplyResistanceForces()
    {
        float speed = carRigidbody.linearVelocity.magnitude;
        float airDragForce = airResistance * speed * speed;
        Vector3 airDrag = -carRigidbody.linearVelocity.normalized * airDragForce;
        carRigidbody.AddForce(airDrag);

        float rollingDragForce = rollingResistance * carRigidbody.mass * 9.81f;
        Vector3 rollingDrag = -carRigidbody.linearVelocity.normalized * rollingDragForce;
        carRigidbody.AddForce(rollingDrag);
    }

    void DebugPowerStatus()
    {
        string status = $"Состояние мощности: ";
        status += $"Передача={currentGear}, ";
        status += $"Сцепление={clutchInput:F2}, ";
        status += $"isShifting={isShifting}, ";
        status += $"Газ={throttleInput:F2}, ";
        status += $"RPM={engineRPM:F0}";

        Debug.Log(status);
    }

    string GetGearName(int gear)
    {
        switch (gear)
        {
            case -1: return "R";
            case 0: return "N";
            default: return gear.ToString();
        }
    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 400, 400));

        GUILayout.Label("=== СИСТЕМА ПЕРЕДАЧИ МОЩНОСТИ ===");
        GUILayout.Label($"Скорость: {carSpeed:F1} км/ч");
        GUILayout.Label($"Обороты: {engineRPM:F0} RPM");
        GUILayout.Label($"Передача: {GetGearName(currentGear)}");
        GUILayout.Label($"Газ: {(throttleInput * 100):F0}%");
        GUILayout.Label($"Тормоз: {(brakeInput * 100):F0}%");
        GUILayout.Label($"Сцепление: {(clutchInput * 100):F0}%");
        GUILayout.Label($"Ручной тормоз: {(handbrakeInput ? "ВКЛ" : "ВЫКЛ")}");
        GUILayout.Label($"Переключение: {(isShifting ? "ДА" : "НЕТ")}");

        GUILayout.Label("");
        GUILayout.Label("=== УСЛОВИЯ ДВИЖЕНИЯ ===");
        bool canMove = (currentGear != 0) && (clutchInput < 0.8f) && (!isShifting) && (!handbrakeInput);
        GUILayout.Label($"Может ехать: {(canMove ? "ДА" : "НЕТ")}");

        if (!canMove)
        {
            GUILayout.Label("");
            GUILayout.Label("=== ПРИЧИНЫ ОСТАНОВКИ ===");
            if (currentGear == 0) GUILayout.Label("❌ Передача в нейтрали");
            if (clutchInput >= 0.8f) GUILayout.Label("❌ Сцепление выжато");
            if (isShifting) GUILayout.Label("❌ Идет переключение");
            if (handbrakeInput) GUILayout.Label("❌ Включен ручной тормоз");
        }

        GUILayout.Label("");
        GUILayout.Label("=== ИНСТРУКЦИЯ ===");
        GUILayout.Label("1. Включите передачу (1-6)");
        GUILayout.Label("2. ОТПУСТИТЕ сцепление полностью");
        GUILayout.Label("3. Нажмите газ");
        GUILayout.Label("4. Убедитесь что ручник выключен");

        GUILayout.EndArea();
    }
}