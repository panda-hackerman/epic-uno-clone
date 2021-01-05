using UnityEngine;
using TMPro;

[System.Obsolete]
public class RandomText : MonoBehaviour
{
    [SerializeField] private string[] _sentance = null;
    private TextMeshProUGUI _text;

    private void Start()
    {
        _text = GetComponent<TextMeshProUGUI>();
        _text.text = _sentance[Random.Range(0, _sentance.Length)];
    }
}
