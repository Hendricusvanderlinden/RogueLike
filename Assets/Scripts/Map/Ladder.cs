using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    [SerializeField]
    private bool up; // Can be modified in the Unity editor

    public bool Up { get => up; set => up = value; }

    private void Start()
    {
        GameManager.Get.AddLadder(this);
    }
}