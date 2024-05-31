using System.Collections;
using UnityEngine;



[CreateAssetMenu(fileName="buildingSO", menuName="SO/BuildingSO")]
public class BuildingSO : ScriptableObject
{
    public GameObject prefab;
    public Vector2Int buildingSize;
    public string buildingName; 
}
