using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShaker : MonoBehaviour
{
    // https://medium.com/nice-things-ios-android-development/basic-2d-screen-shake-in-unity-9c27b56b516
    // Transform of the GameObject you want to shake
    private new Transform transform;
 
    // Desired duration of the shake effect
    private float shakeDuration = 0f;
 
    // A measure of magnitude for the shake. Tweak based on your preference
    private float TotalShakeMagnitude = 0.2f;
    private float currShakeMagnitude = 0.2f;
 
    // A measure of how quickly the shake effect should evaporate
    private float dampingSpeed = 1.0f;
 
    // The initial position of the GameObject
    Vector3 initialPosition;
    
    // singleton
    public static ScreenShaker Instance;
    
    void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        if (transform == null)
        {
            transform = GetComponent(typeof(Transform)) as Transform;
        }
    }
    
    void OnEnable()
    {
        initialPosition = transform.localPosition;
    }
    
    void Update()
    {
        if (shakeDuration > 0)
        {
            transform.localPosition = initialPosition + Random.insideUnitSphere * currShakeMagnitude;
   
            shakeDuration -= Time.deltaTime * dampingSpeed;
        }
        else
        {
            shakeDuration = 0f;
            transform.localPosition = initialPosition;
        }
    }
    
    public void TriggerShake(float strength = 1) {
        shakeDuration = 0.5f;
        currShakeMagnitude = TotalShakeMagnitude * strength;
    }
}
