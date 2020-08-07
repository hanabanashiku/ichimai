using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SinglePlayerMenuEvents : MonoBehaviour {

    //Settings settings;

    public InputField nameField;
    public InputField countField;

    void Start() {
        //settings = Settings.Load();
        //Debug.Log(nameField);
        //nameField.GetComponent<Text>().text = settings.LastPlayerName;
        //nameField.GetComponent<Text>().text = settings.LastNumberOfPlayers.ToString();
    }

    public void StartGame() {
        if (nameField.text == string.Empty || countField.text == string.Empty) return;
        SinglePlayerSceneDataRelayer.InitSceneData(int.Parse(countField.text), nameField.text);
        SceneManager.LoadScene("SinglePlayerGame");
    }

    public void ShowOptions() { }

    public void PreviousScreen() {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnCountChanged() {
        if ("01-".Contains(countField.text)) countField.text = "2";
        else if (countField.text == "9") countField.text = "8";
    }
}
