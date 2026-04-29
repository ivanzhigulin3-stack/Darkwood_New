using UnityEngine;
using System.Collections.Generic;

public class Container : MonoBehaviour
{
    [Header("Настройки контейнера")]
    public string containerName = "Сундук";
    public int containerSize = 20;
    public KeyCode openKey = KeyCode.E;
    public float interactionRange = 2f;

    [Header("Визуальные эффекты")]
    public GameObject openEffect;
    public GameObject closeEffect;
    public Sprite openSprite;
    public Sprite closeSprite;

    [Header("Начальные предметы")]
    //public List<ContainerStartItem> startItems;

    private bool isOpen = false;
    private bool playerInRange = false;
    private GameObject currentPlayer;
}
