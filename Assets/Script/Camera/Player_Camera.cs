using UnityEngine;

public class Player_Camera : MonoBehaviour
{
    [Header("Targets")]
    [Tooltip("Object that yaws left/right. Use your player root. If null, this Transform is used.")]
    public Transform yawTarget;
    [Tooltip("Object that pitches up/down. Use your Camera. If null, this Transform is used.")]
    public Transform pitchTarget;

    [Header("Settings")]
    public float sensitivity = 100f;
    public float smoothTime = 0.03f;   // lower = snappier
    [Range(-89f, 89f)] public float minPitch = -80f;
    [Range(-89f, 89f)] public float maxPitch = 80f;

    [Header("Cursor")]
    public bool lockCursor = true;

    float _yaw;           // degrees
    float _pitch;         // degrees
    Vector2 _vel;         // smoothing velocity for damp
    Vector2 _smoothed;    // smoothed delta

    void Awake()
    {
        if (yawTarget == null) yawTarget = transform;
        if (pitchTarget == null) pitchTarget = transform;
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // Initialize from current rotation
        Vector3 yawEuler = yawTarget.eulerAngles;
        Vector3 pitchEuler = pitchTarget.localEulerAngles;
        _yaw = yawEuler.y;
        _pitch = NormalizePitch(pitchEuler.x);
    }

    void Update()
    {
        // Raw mouse delta (unscaled)
        float dx = Input.GetAxisRaw("Mouse X");
        float dy = Input.GetAxisRaw("Mouse Y");

        // Smooth the delta to avoid jitter
        Vector2 target = new Vector2(dx, dy) * sensitivity;
        _smoothed = Vector2.SmoothDamp(_smoothed, target, ref _vel, smoothTime);

        // Yaw left/right is mouse X (+)
        _yaw += _smoothed.x * Time.unscaledDeltaTime;

        // Pitch up/down is mouse Y (inverted: up is negative)
        _pitch -= _smoothed.y * Time.unscaledDeltaTime;
        _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);

        // Apply
        if (yawTarget) yawTarget.rotation = Quaternion.Euler(0f, _yaw, 0f);
        if (pitchTarget) pitchTarget.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
    }

    static float NormalizePitch(float x)
    {
        // Convert 0..360 to -180..180 to clamp properly
        if (x > 180f) x -= 360f;
        return x;
    }
}
