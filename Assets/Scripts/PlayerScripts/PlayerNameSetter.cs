using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerNameSetter : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_InputField _nameInputField = null;
    [SerializeField] private Button _continueButton = null;

    public static string PlayerDisplayName { get; private set; }
    private const string PlayerPrefsNameKey = "Name";

    private void Start() => SetUpInputField();

    // autofills InputField with player's last saved name
    private void SetUpInputField()
    {
        // returns if the player doesn't have a saved name yet
        if (!PlayerPrefs.HasKey(PlayerPrefsNameKey)) { return; }

        // gets saved name from Playerprefs
        string defaultName = PlayerPrefs.GetString(PlayerPrefsNameKey);
        // sets the name input field text to the saved name
        _nameInputField.text = defaultName;

        CheckPlayerName(defaultName);
    }

    // call when InputField is updated
    // checks if the player name can be used
    public void CheckPlayerName(string NewName)
    {
        // makes button un-interactable if the player name is null or empty
        _continueButton.interactable = !string.IsNullOrEmpty(name);
    }

    // call from a UI button
    // saves the players name
    public void SavePlayerName()
    {
        // sets the player's name to the text in the input field
        PlayerDisplayName = _nameInputField.text;
        // Saves the key to have names pre-filled in input field
        // Saves the name so it's a value we can access somewhere else
        PlayerPrefs.SetString(PlayerPrefsNameKey, PlayerDisplayName);
    }
}
