using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLine : MonoBehaviour
{
    [SerializeField] private float reloadDelay = 1f; //Delay before reloading the scene.
    [SerializeField] private ParticleSystem finishEffect; //Particle effect to play when the player crosses the finish line.
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player has crossed the finish line!");
            finishEffect.Play();
            
            Invoke(nameof(NextLevel), reloadDelay); // Reload the scene after 1 second delay
            //TO DO: You can add additional logic here, such as triggering a win condition or loading a new scene.
        }
    }
    void NextLevel()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        PlayerPrefs.SetInt("UnlockedLevel", unlockedLevel+1); //Unlock the next level
        PlayerPrefs.Save();
        // Reload the current scene.
        if (unlockedLevel >= 3)
        {
            SceneManager.LoadScene($"Menu"); //TODO: Go to Win Scene.
            Debug.Log("Juego completado, volviendo al menú. Nivel: " + unlockedLevel);
        }
        else SceneManager.LoadScene($"Level{unlockedLevel+1}_Forest"); //Load the next level.
    } 
}