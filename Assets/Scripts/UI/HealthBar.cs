using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HealthBar : MonoBehaviour
{
    private VisualElement root;
    private VisualElement healthBar;
    private Label healthLabel;
    private Label levelLabel;
    private Label xpLabel;

    void Start()
    {
        var uiDocument = GetComponent<UIDocument>();
        if (uiDocument != null)
        {
            root = uiDocument.rootVisualElement;
            healthBar = root.Q<VisualElement>("HealthBar");
            healthLabel = root.Q<Label>("HealthText");
            levelLabel = root.Q<Label>("LevelText");
            xpLabel = root.Q<Label>("XPText");
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

        float percent = (float)currentHitPoints / maxHitPoints * 100f;
        healthBar.style.width = Length.Percent(percent);
        healthLabel.text = $"{currentHitPoints}/{maxHitPoints} HP";
    }

    public void SetLevel(int level)
    {
        levelLabel.text = $"Level: {level}";
    }

    public void SetXP(int xp)
    {
        xpLabel.text = $"XP: {xp}";
    }
}