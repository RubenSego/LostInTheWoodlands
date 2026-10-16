using UnityEngine;
using UnityEngine.SceneManagement;

public class CrashDetector : MonoBehaviour
{
    [SerializeField] private float crashDelay = 1f; // Delay before reloading the scene
    [SerializeField] private ParticleSystem crashEffect;
    [SerializeField] private AudioClip deathSound;
    PlayerController playerController;

    private AudioSource audioSource;
    private static bool hasCrashed = false;
    void Start()
    {
        hasCrashed = false;
        playerController = FindAnyObjectByType<PlayerController>();
        audioSource = GetComponent<AudioSource>();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        int layerIndex = LayerMask.NameToLayer("Floor");

        if (other.gameObject.layer == layerIndex)
        {
            if (hasCrashed) return;
            hasCrashed = true;
            playerController.CanControlPlayer = false; // Disable player control
            crashEffect.Play();
            if (audioSource != null && deathSound != null)
            {
                audioSource.PlayOneShot(deathSound);
            }
            Invoke(nameof(ReloadScene), crashDelay); // Reload the scene after the specified delay
        }
    }

    void ReloadScene()
    {
        // Reload the current scene.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    } 
}
