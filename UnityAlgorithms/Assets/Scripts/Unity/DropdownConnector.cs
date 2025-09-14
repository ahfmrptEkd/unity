using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityAlgorithms.Unity;

public class DropdownConnector : MonoBehaviour
{
    void Start()
    {
        // Find the GameManager and Dropdown objects in the scene
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        
        // Try both regular Dropdown and TMP_Dropdown
        Dropdown regularDropdown = FindFirstObjectByType<Dropdown>();
        TMP_Dropdown tmpDropdown = FindFirstObjectByType<TMP_Dropdown>();

        // Assign the appropriate dropdown to GameManager
        if (gameManager != null)
        {
            if (tmpDropdown != null)
            {
                gameManager.difficultyDropdown = tmpDropdown;
                Debug.Log("TMP_Dropdown successfully assigned to GameManager.");
            }
            else if (regularDropdown != null)
            {
                gameManager.difficultyDropdown = regularDropdown;
                Debug.Log("Regular Dropdown successfully assigned to GameManager.");
            }
            else
            {
                Debug.LogError("No Dropdown found in the scene!");
            }
        }
        else
        {
            Debug.LogError("GameManager not found in the scene!");
        }
    }
}