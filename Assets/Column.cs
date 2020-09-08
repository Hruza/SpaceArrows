using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Column : MonoBehaviour
{
    public RectTransform column;

    public Text valueText;

    public float Value {
        set {
            column.localScale = new Vector3(column.localScale.x,value,1);
            valueText.text = value.ToString("F3");
        }
    }
}
