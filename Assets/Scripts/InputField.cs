using TMPro;
using UnityEngine;

/**
 * Takes the prompt from the inputfield and passes it to the ClientgRPC script's ChangeMaterial function.
 */
public class InputField : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI inputField;

    public void GetPrompt()
    {
        Debug.Log(inputField.text);
        FindObjectOfType<ClientgRPC>().ChangeMaterial(inputField.text);
    }
}
