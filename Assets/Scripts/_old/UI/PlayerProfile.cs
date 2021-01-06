using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Obsolete]
public class PlayerProfile : MonoBehaviour
{
    [Header("DisplayName")]
    [SerializeField] private TMP_InputField _nameInputField = null;
    [SerializeField] private Button _continueButton = null;

    public static string _playerDisplayName { get; private set; }
    private const string _playerPrefsNameKey = "Name";

    private void Start() => SetUpInputField();

    private void SetUpInputField()
    {
        // returns if the player doesn't have a saved name yet
        if(!PlayerPrefs.HasKey(_playerPrefsNameKey)) { return; }

        // gets saved name from Playerprefs
        string defaultName = PlayerPrefs.GetString(_playerPrefsNameKey);
        // sets the name input field text to the saved name
        _nameInputField.text = defaultName;

        CheckPlayerName(defaultName);
    }

    public void CheckPlayerName(string NewName)
    {
        // makes button un-interactable if the player name is null or empty
        _continueButton.interactable = !string.IsNullOrEmpty(name);
    }

    // called from a UI button
    public void SavePlayerName()
    {
        // sets the player's name to the text in the input field
        _playerDisplayName = _nameInputField.text;
        // Saves the key to have names pre-filled in input field
        // Saves the name so it's a value we can access somewhere else
        PlayerPrefs.SetString(_playerPrefsNameKey, _playerDisplayName);
    }
}
