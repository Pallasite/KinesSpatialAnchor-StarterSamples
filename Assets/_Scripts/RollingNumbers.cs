using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class RollingNumbers : MonoBehaviour
{
    public int operand = 0;
    TextMeshProUGUI numerical_canvas;
    private static System.Random rnd;

    // Start is called before the first frame update
    void Start()
    {
        numerical_canvas = this.GetComponent<TextMeshProUGUI>();
        StartCoroutine(RollingNumbersLoop());
    }

    IEnumerator RollingNumbersLoop()
    {
        rnd = new System.Random();
        while (true)
        {     
            operand = rnd.Next(1, 99);
            
            Debug.Log(operand);
            numerical_canvas.text = (operand.ToString());
            yield return new WaitForSeconds(19f);
        }
    }
}
