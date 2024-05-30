using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RollingColors : MonoBehaviour
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

        //String array of 6 color words
        string[] colors = { "Red", "Green", "Blue", "Yellow", "Purple", "0range" };

        while (true)
        {
            operand = rnd.Next(1, 99);           

            //randomly select a color from the array
            string color = colors[rnd.Next(0, colors.Length)];

            Debug.Log(operand);
            numerical_canvas.text = (operand.ToString());
            numerical_canvas.text = color;

            float  r = (float)rnd.Next(0, 2);
            float  g = (float)rnd.Next(0, 2);
            float  b = (float)rnd.Next(0, 2);

            Debug.Log(r + " " + g + " " + b);

            numerical_canvas.color = new Color(r, g, b);

            yield return new WaitForSeconds(19f);            
        }
    }
}
