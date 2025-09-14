using UnityEngine;
using TMPro;
using UnityAlgorithms.Unity;

public class DropdownConnector : MonoBehaviour
{
    void Start()
    {
        // Find the GameManager and TMP_Dropdown objects in the scene
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        TMP_Dropdown tmpDropdown = FindFirstObjectByType<TMP_Dropdown>();

        // Assign the dropdown to GameManager
        if (gameManager != null && tmpDropdown != null)
        {
            gameManager.difficultyDropdown = tmpDropdown;
            Debug.Log("TMP_Dropdown successfully assigned to GameManager.");
        }
        else
        {
            if (gameManager == null)
                Debug.LogError("GameManager not found in the scene!");
            if (tmpDropdown == null)
                Debug.LogError("TMP_Dropdown not found in the scene!");
        }
    }
}