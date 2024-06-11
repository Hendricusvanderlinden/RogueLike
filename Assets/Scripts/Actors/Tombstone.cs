using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tombstone : MonoBehaviour
{
    // Variabelen
    public string playerName = "Player"; // Naam van de speler
    public string causeOfDeath = "Unknown"; // Oorzaak van de dood

    private TextMesh textMesh; // Referentie naar de tekstcomponent op de tombe

    // Functie om de tombe in te stellen met de gegeven speler en oorzaak van de dood
    public void Setup(string player, string cause)
    {
        playerName = player;
        causeOfDeath = cause;

        // Update de tekst op de tombe
        UpdateTombstoneText();
    }

    // Functie om de tekst op de tombe bij te werken
    private void UpdateTombstoneText()
    {
        if (textMesh == null)
        {
            textMesh = GetComponentInChildren<TextMesh>();
            if (textMesh == null)
            {
                Debug.LogError("TextMesh component not found!");
                return;
            }
        }

        textMesh.text = $"{playerName}\n{causeOfDeath}";
    }

    private void Start()
    {
        // Voeg de tombe toe aan de GameManager
        GameManager.Get.AddTombstone(this);

        // Update de tekst op de tombe
        UpdateTombstoneText();
    }

    // Update is called once per frame
    private void Update()
    {
        // Voeg hier eventuele update-logica voor de tombe toe
    }

}
