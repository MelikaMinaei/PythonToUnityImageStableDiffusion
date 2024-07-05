using TMPro;
using UnityEngine;

/**
 * Takes the prompt from the inputfield and passes it to the ClientgRPC script's ChangeMaterial function.
 */
public class InputField : MonoBehaviour
{
    public TMP_InputField tmpInputField;

    void Start()
    {
        tmpInputField.onEndEdit.AddListener(TextMeshUpdated);
    }

    public void TextMeshUpdated(string text)
    {
        Debug.Log("Entered prompt in unity: " + text);
        FindObjectOfType<ClientgRPC>().ChangeMaterial(tmpInputField.text);
    }
}
