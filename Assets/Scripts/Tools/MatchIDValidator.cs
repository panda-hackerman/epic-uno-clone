using UnityEngine;
using UnityEngine.UI;

/*  This script makes sure everything entered
 *  into the matchID text box is upper case
 *  Otherwise the connection won't work
 *  Could also have put an alphanumeric filer but the tmp text box
 *  already handles that
 */
public class MatchIDValidator : MonoBehaviour
{
    public InputField matchIDField;

    public void ValueChanged(string input)
    {
        matchIDField.text = input.ToUpper();
    }
}
