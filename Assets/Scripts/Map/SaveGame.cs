using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveGame : MonoBehaviour
{
    void Start()
    {
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            GameManager.Get.SetPlayer(player);
        }
    }
}
