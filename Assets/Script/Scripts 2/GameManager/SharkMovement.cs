using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public enum ControlMode { Keyboard, Voice }

public class SharkMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float dashSpeedMultiplier = 2f;
    public float dashStaminaMax = 1f;
    public float dashStaminaDrainRate = 1f;
    public float dashStaminaRegenRate = 0.5f;
    public float doubleTapTime = 0.25f;

    public ControlMode controlMode = ControlMode.Keyboard;
    public ControlModeUI controlModeUI;
    public Slider dashBar;
    public Transform dashBarFollowTarget;
    public Vector3 dashBarOffset = new Vector3(0, 1f, 0);

    [Header("Movement Bounds")]
    public float minX = -8f;
    public float maxX = 8f;
    public float minY = -4f;
    public float maxY = 4f;

    [Header("Homing Settings")]
    public float homingRange = 3f;
    public float homingSpeed = 2f;

    private Animator sharkAnimator;
    private float targetAngle = 0f;
    private bool isRotating = false;
    private SharkStats sharkStats;

    private float currentStamina;
    private bool isDashing;

    private float lastTapTimeX = -1f;
    private float lastTapTimeY = -1f;

    private Vector3 input;

    void Start()
    {
        sharkAnimator = GetComponent<Animator>();
        sharkStats = GetComponent<SharkStats>();
        currentStamina = dashStaminaMax;

        if (controlModeUI != null)
            controlModeUI.UpdateControlModeText(controlMode);

        if (PlayerPrefs.HasKey("controlMode"))
            controlMode = (ControlMode)PlayerPrefs.GetInt("controlMode");

        if (controlModeUI != null)
            controlModeUI.UpdateControlModeText(controlMode);

        FindObjectOfType<ControlIndicatorUI>()?.SetControlMode(controlMode);

        if (dashBar != null)
        {
            dashBar.minValue = 0f;
            dashBar.maxValue = dashStaminaMax;
            dashBar.value = dashStaminaMax;
        }
    }

    void Update()
    {
        float horizontal = 0f;
        float vertical = 0f;

        if (controlMode == ControlMode.Keyboard)
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");

            input = new Vector3(horizontal, vertical, 0).normalized;

            if (DetectDoubleTap(KeyCode.A, ref lastTapTimeX) || DetectDoubleTap(KeyCode.D, ref lastTapTimeX) ||
                DetectDoubleTap(KeyCode.W, ref lastTapTimeY) || DetectDoubleTap(KeyCode.S, ref lastTapTimeY))
            {
                if (currentStamina > 0f)
                    isDashing = true;
            }
            else if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
            {
                isDashing = false;
            }
        }
        else if (controlMode == ControlMode.Voice && VoiceManager.Instance != null)
        {
            Vector2 voiceDir = VoiceManager.Instance.VoiceDirection;

            if (voiceDir != Vector2.zero)
            {
                input = new Vector3(voiceDir.x, voiceDir.y, 0).normalized;
                isDashing = VoiceManager.Instance.IsDashRequested && currentStamina > 0f;
            }
            else
            {
                input = Vector3.zero; // 🛑 Reset movement when command ends

                // Homing
                GameObject nearestGarbage = FindNearestGarbage();
                if (nearestGarbage != null)
                {
                    float dist = Vector3.Distance(transform.position, nearestGarbage.transform.position);
                    GarbageType garbageType = nearestGarbage.GetComponent<GarbageType>();

                    if (dist < homingRange && garbageType != null && sharkStats.CanEat(garbageType.size))
                    {
                        Vector3 direction = (nearestGarbage.transform.position - transform.position).normalized;
                        input = Vector3.Lerp(Vector3.zero, direction, homingSpeed * Time.deltaTime);
                    }
                }

                isDashing = false;
            }

        }
        float currentSpeed = moveSpeed;
        if (isDashing && currentStamina > 0f)
        {
            currentSpeed *= dashSpeedMultiplier;
            currentStamina -= dashStaminaDrainRate * Time.deltaTime;
            currentStamina = Mathf.Max(0f, currentStamina);
        }
        else
        {
            isDashing = false;
            currentStamina += dashStaminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Min(dashStaminaMax, currentStamina);
        }

        Vector3 move = input * currentSpeed * Time.deltaTime;
        Vector3 newPosition = transform.position + move;

        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

        transform.position = newPosition;

        if (input.x < 0 && targetAngle != 180f)
        {
            targetAngle = 180f;
            isRotating = true;
        }
        else if (input.x > 0 && targetAngle != 0f)
        {
            targetAngle = 0f;
            isRotating = true;
        }

        if (isRotating)
        {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, 700f * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);
            if (Mathf.Abs(angle - targetAngle) < 1f)
                isRotating = false;
        }

        sharkAnimator.SetBool("IsSwimming", input.magnitude > 0);

        if (dashBar != null && dashBarFollowTarget != null)
        {
            dashBar.value = currentStamina;
            if (Camera.main != null)
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + dashBarOffset);
                dashBar.transform.position = screenPos;
            }
            dashBar.gameObject.SetActive(currentStamina < dashStaminaMax);
        }
    }

    private bool DetectDoubleTap(KeyCode key, ref float lastTapTime)
    {
        if (Input.GetKeyDown(key))
        {
            if (Time.time - lastTapTime < doubleTapTime)
            {
                lastTapTime = -1f;
                return true;
            }
            lastTapTime = Time.time;
        }
        return false;
    }

    private GameObject FindNearestGarbage()
    {
        GameObject[] allGarbage = GameObject.FindGameObjectsWithTag("Garbage");

        // Sort all garbage by distance to the shark
        var sorted = allGarbage
            .Select(g => new { obj = g, dist = Vector3.Distance(transform.position, g.transform.position) })
            .Where(x => x.dist < homingRange)
            .OrderBy(x => x.dist)
            .Take(2) // Limit to 2 closest
            .ToList();

        if (sorted.Count > 0)
            return sorted[0].obj;

        return null;
    }


    public void ToggleControlMode()
    {
        controlMode = controlMode == ControlMode.Keyboard ? ControlMode.Voice : ControlMode.Keyboard;

        PlayerPrefs.SetInt("controlMode", (int)controlMode);
        PlayerPrefs.Save();

        if (controlModeUI != null)
        {
            controlModeUI.UpdateControlModeText(controlMode);
            controlModeUI.ShowControlNotification(controlMode);
        }

        FindObjectOfType<ControlIndicatorUI>()?.SetControlMode(controlMode);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Garbage")) return;

        GarbageType garbage = other.GetComponent<GarbageType>();
        if (garbage == null || sharkStats == null) return;

        if (sharkStats.CanEat(garbage.size))
        {
            sharkAnimator.SetTrigger("BiteTrigger");
            Destroy(other.gameObject);
            sharkStats.AddScore(garbage.points);
            GetComponent<SharkSFX>()?.PlayBiteSound();
        }
        else
        {
            sharkStats.ShowTooBigPopup();
            Debug.Log("Shark is too small to eat: " + garbage.size);
        }
    }
}
