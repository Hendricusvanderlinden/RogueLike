using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Messages : MonoBehaviour
{
    private Label[] labels = new Label[5];
    private VisualElement root;

    void Start()
    {
        // Haal de root VisualElement op van de UI Document
        var uiDocument = GetComponent<UIDocument>();
        if (uiDocument != null)
        {
            root = uiDocument.rootVisualElement;

            // Vind de labels in de UI en wijs ze toe aan de labels array
            for (int i = 0; i < labels.Length; i++)
            {
                labels[i] = root.Q<Label>($"Label{i + 1}");
            }
        }
        else
        {
            Debug.LogError("UIDocument component not found on the GameObject.");
        }

        // Voer de Clear functie uit om alle labels leeg te maken
        Clear();

        // Voeg een welkomstbericht toe
        AddMessage("Welcome to the dungeon, Adventurer!", Color.green);
    }

    public void Clear()
    {
        // Stel de tekst van alle labels gelijk aan een lege string
        foreach (var label in labels)
        {
            label.text = string.Empty;
        }
    }

    public void MoveUp()
    {
        // Verplaats de tekst en kleur van elk label een plaats omhoog
        for (int i = labels.Length - 1; i > 0; i--)
        {
            labels[i].text = labels[i - 1].text;
            labels[i].style.color = labels[i - 1].style.color;
        }

        // Maak het eerste label leeg
        labels[0].text = string.Empty;
        labels[0].style.color = Color.clear;
    }

    public void AddMessage(string content, Color color)
    {
        // Voer eerst de MoveUp functie uit
        MoveUp();

        // Stel het eerste label in met de voorziene tekst en kleur
        labels[0].text = content;
        labels[0].style.color = color;
    }
}
