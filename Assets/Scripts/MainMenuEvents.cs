using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuEvents : MonoBehaviour {

	public void StartSinglePlayer() {
        SceneManager.LoadScene("SinglePlayerGameSelector");
    }

    public void StartMultiplayer() {

    }

    public void ShowOptions() {

    }
    
    public void QuitGame() {
        Application.Quit();
    }
}
