using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance;

    [SerializeField] private GameObject itemContainerUI;


    private void Start()
    {
        itemContainerUI.SetActive(false);    
    }
    private void Awake()
    {
        if(Instance != null || Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this.gameObject);
        Instance = this;
    }
}
