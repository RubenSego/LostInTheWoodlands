using UnityEngine;
using UnityEngine.SceneManagement;

public class Hazard : MonoBehaviour
{
    [Tooltip("Delay before reloading the scene after death")]
    [SerializeField] private float reloadDelay = 0.5f;

    [Tooltip("Particle effect played when colliding with the hazard")]
    [SerializeField] private ParticleSystem hitEffect;

    [Tooltip("Audio clip played upon death")]
    [SerializeField] private AudioClip deathSound;

    private bool hasTriggered;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the collided object is the player
        if (collision.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;

            // Disable player controls
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player == null)
            {
                // In case the collider is on a child object
                player = collision.GetComponentInParent<PlayerController>();
            }

            if (player != null)
            {
                player.CanControlPlayer = false;
            }

            // Play particles if assigned
            if (hitEffect != null)
            {
                hitEffect.Play();
            }

            // Play audio if assigned
            if (deathSound != null)
            {
                AudioSource.PlayClipAtPoint(deathSound, transform.position);
            }

            // Reload the active level
            Invoke(nameof(ReloadScene), reloadDelay);
        }
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}