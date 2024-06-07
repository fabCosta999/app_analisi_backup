using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class sliderLogic : MonoBehaviour
{
    public Slider slider_x, slider_y;
    public TextMeshProUGUI leftText_x, rightText_x, leftText_y, rightText_y, x_text, y_text, upText_x, upText_y;
    public keyboard inputFunction;

    private void Update()
    {
        SetTextColors();
        ComputeXY();
    }

    public void Center()
    {
        slider_x.value = 0;
    }

    private void SetTextColors()
    {
        if (PlayerPrefs.GetInt(keysGeneral.LIGHTMODE) == (int)keysGeneral.lightMode.light)
        {
            rightText_x.color = Color.black;
            leftText_x.color = Color.black;
            rightText_y.color = Color.black;
            leftText_y.color = Color.black;
            upText_x.color = Color.black;
            upText_y.color = Color.black;
        }
        else
        {
            rightText_x.color = Color.white;
            leftText_x.color = Color.white;
            rightText_y.color = Color.white;
            leftText_y.color = Color.white;
            upText_x.color = Color.white;
            upText_y.color = Color.white;
        }
    }

    private void ComputeXY()
    {
        float x = (Mathf.Round(slider_x.value * 100) / 100f);
        slider_x.value = x;
        x_text.text = x.ToString();

        float y = inputFunction.Function(x);
        if (float.IsNaN(y) || float.IsInfinity(y) || float.IsNegativeInfinity(y)) slider_y.value = 0;
        else if (y < -10) slider_y.value = -10;
        else if (y > 10) slider_y.value = 10;
        else slider_y.value = y;
        if (float.IsNaN(y) || float.IsInfinity(y) || float.IsNegativeInfinity(y)) y_text.text = "NaN";
        else y_text.text = (Mathf.Round(y * 100) / 100f).ToString();
    }
}
