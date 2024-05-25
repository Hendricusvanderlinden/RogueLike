using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HealthBar : MonoBehaviour
{
    private VisualElement root;
    private VisualElement healthBar;
    private Label healthLabel;

    void Start()
    {
        // Haal de root VisualElement op van de UI Document
        var uiDocument = GetComponent<UIDocument>();
        if (uiDocument != null)
        {
            root = uiDocument.rootVisualElement;

            // Vind de healthBar en healthLabel elementen in de UI
            healthBar = root.Q<VisualElement>("HealthBar");
            healthLabel = root.Q<Label>("HealthText");
        }
        else
        {
            Debug.LogError("UIDocument component not found on the GameObject.");
        }
    }

    public void SetValues(int currentHitPoints, int maxHitPoints)
    {
        if (maxHitPoints <= 0)
        {
            Debug.LogError("maxHitPoints must be greater than zero.");
            return;
        }

        // Bereken het percentage van de huidige hit points ten opzichte van de maximale hit points
        float percent = (float)currentHitPoints / maxHitPoints * 100f;

        // Pas de breedte van de healthBar aan op basis van het percentage
        healthBar.style.width = Length.Percent(percent);

        // Pas de tekst van het healthLabel aan
        healthLabel.text = $"{currentHitPoints}/{maxHitPoints} HP";
    }
}
