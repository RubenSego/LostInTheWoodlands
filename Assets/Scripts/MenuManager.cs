using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Level1_Forest");
    }
    public void GoToLevel(int level){
        SceneManager.LoadScene($"Level{level}_Forest");
    }
    public void PlayLevelSelection()
    {
        SceneManager.LoadScene("LevelSelection");
    }
    public void MenuReturn()
    {
        SceneManager.LoadScene("Menu");
    }
    public void PlayCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void PlayCharacters()
    {
        SceneManager.LoadScene("Character");
    }
    public void ExitGame()
    {
        Application.Quit(); // For builded game.

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stop game mode on editor.
        #endif    
    }
}
