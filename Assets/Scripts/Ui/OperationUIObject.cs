using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperationUIObject : MonoBehaviour
{
    [SerializeField] private UserOperation operation;

    public void SetUserOperation()
    {
        GameManager.Instance.operation = operation;
        GridVisualizer.Instance.ResetAllGridHighlights();
    }
}
