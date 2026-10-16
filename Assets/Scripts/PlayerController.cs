using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float torqueAmount = 1f;
    [SerializeField] private float normalSpeed = 15f;
    [SerializeField] private float boostSpeed = 30f;
    
    [Header("Brake Mechanics")]
    [SerializeField] private float brakeSpeed = 6f;
    [Tooltip("Linear damping applied to the player while braking")]
    [SerializeField] private float brakeDamping = 6f;
    [SerializeField] private float normalDamping = 0f;
    
    [Header("Ground Pound / Slam Mechanics")]
    [Tooltip("Downward velocity when performing a ground pound in the air")]
    [SerializeField] private float slamDownForce = 25f;

    [SerializeField] private ParticleSystem snowEffect;
    [SerializeField] private ScoreManager scoreManager; //Reference to the ScoreManager script to update the score.

    private bool isGrounded;
    private bool hasSlammed;

    SurfaceEffector2D se;
    Rigidbody2D rb;
    
    float baseSpeed;
    InputAction moveAction;
    Vector2 moveInput;
    float previousRotation; // Store the previous rotation of the player.
    float totalRotation; // Store the total rotation of the player.
    int flipCount; // Store the number of flips the player has performed.
    int activePowerUpsCount; //Track the number of active power-ups.

    private bool canControlPlayer = true; // Flag to control player input.
    public bool CanControlPlayer { get => canControlPlayer; set => canControlPlayer = value; }

    void Start()
    {
        transform.GetChild(0).GetChild(PlayerPrefs.GetInt("SelectedCharacter", 0 )).gameObject.SetActive(true);

        moveAction = InputSystem.actions.FindAction("Move");
        rb = GetComponent<Rigidbody2D>();
        se = FindAnyObjectByType<SurfaceEffector2D>();
        baseSpeed = se.speed;

        rb.linearDamping = normalDamping;
    }

    void Update()
    {
        if(!canControlPlayer) return; // If player input is disabled, exit the method.
        
        moveInput = moveAction.ReadValue<Vector2>();

        PlayerTorque();
        ManageSpeedAndBrake();
        HandleAirSlam();
        CalculateFlips();        
    }
    /// <summary>
    /// Calculates the number of flips the player has made based on their rotation.
    /// </summary>
    private void CalculateFlips()
    {
        float currentRotation = transform.rotation.eulerAngles.z; //Get the current rotation of the player in degrees.
        totalRotation += Mathf.DeltaAngle(previousRotation, currentRotation); // Calculate the change in rotation since the last frame and add it to the total rotation.
        if (Math.Abs(totalRotation) > 340)
        {
            flipCount++;
            scoreManager.AddScore(flipCount*100); // Update the score in the ScoreManager script.
            totalRotation = 0; // Reset the total rotation after a flip is counted.
        }
        previousRotation = currentRotation; // Update the previous rotation for the next frame.
    }

    /// <summary>
    /// Applies torque to the player based on input from the Move action.
    /// </summary>
    void PlayerTorque()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        if (moveInput.x < 0)
        {
            rb.AddTorque(torqueAmount);
        }
        else if (moveInput.x > 0)
        {
            rb.AddTorque(-torqueAmount);
        }
    }
    /// <summary>
    /// Manages player acceleration, normal cruising, and braking behavior depending on vertical input and ground state.
    /// </summary>
    void ManageSpeedAndBrake()
    {
        if (moveInput.y > 0)
        {
            // Boost speed with Up arrow.
            se.speed = boostSpeed;
            rb.linearDamping = normalDamping;
        }
        else if (moveInput.y < 0 && isGrounded) 
        {
            // Reduce the effector speed and increases linear drag.
            se.speed = brakeSpeed;
            rb.linearDamping = brakeDamping;
        }
        else
        {
            se.speed = baseSpeed; // Return to regular speed.
            rb.linearDamping = normalDamping;
        }
    }
    // Slam in the air
    void HandleAirSlam()
    {
        if (!isGrounded && !hasSlammed && moveInput.y < 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0.7f, -slamDownForce);
            hasSlammed = true;
        }
    }
    
    // Detects when the player collides with the floor and plays the boost effect.
    void OnCollisionEnter2D(Collision2D collision)
    {
        int layerIndex = LayerMask.NameToLayer("Floor");
        if(collision.gameObject.layer == layerIndex)
        {
            isGrounded = true;
            hasSlammed = false; // It allows you to use the slam again on the next jump.
            snowEffect.Play();            
        }
    }
    // Detects when the player collides with the floor and plays the boost effect.
    void OnCollisionExit2D(Collision2D collision)
    {
        int layerIndex = LayerMask.NameToLayer("Floor");
        if(collision.gameObject.layer == layerIndex)
        {
            isGrounded = false;
            snowEffect.Stop();
        }
    }

    public void ApplyPowerUp(PowerUpScriptableObject powerUpData)
    {
        activePowerUpsCount++; // Increment the count of active power-ups.
        if (powerUpData.PowerUpType == "Speed")
        {
            baseSpeed += powerUpData.PowerUpValue; // Increase the base speed by the power-up value.
            boostSpeed += powerUpData.PowerUpValue; // Increase the boost speed by the power-up value.
        }
    }
    public void DeactivatePowerUp(PowerUpScriptableObject powerUpData)
    {
        activePowerUpsCount--; //Decrement the count of active power-ups.
        if (activePowerUpsCount == 0)
        {
            if (powerUpData.PowerUpType == "Speed")
            {
                baseSpeed -= powerUpData.PowerUpValue; //Decrease the base speed by the power-up value.
                boostSpeed -= powerUpData.PowerUpValue; //Decrease the boost speed by the power-up value.
            }
        }
    }
}
