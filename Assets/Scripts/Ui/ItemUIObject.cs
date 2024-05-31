using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemUIObject : MonoBehaviour
{
    public BuildingSO buildingData;
    private TextMeshProUGUI sizeText;

    private void Start()
    {
        sizeText = GetComponentInChildren<TextMeshProUGUI>();
        sizeText.text = buildingData.buildingSize.x + "," + buildingData.buildingSize.y;
    }

    public void SetUserBuildingData()
    {
        GameManager.Instance.SetBuildingData(buildingData);
    }
}
