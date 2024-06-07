using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class sliderLogicLesson : MonoBehaviour
{
    public GameObject slider_x, slider_y;
    public TextMeshProUGUI leftText_x, rightText_x, leftText_y, rightText_y, x_text, y_text, upText_x, upText_y;

    private binTree.BinaryTree functionTree;
    private bool active;

    private void Update()
    {
        if (active)
        {
            slider_x.SetActive(true);
            slider_y.SetActive(true);

            SetTextColors();
            SetTexts();
            ComputeXY();
        }
    }

    public void Hide()
    {
        leftText_x.text = "";
        rightText_x.text = "";
        upText_x.text = "";
        x_text.text = "";
        leftText_y.text = "";
        rightText_y.text = "";
        upText_y.text = "";
        y_text.text = "";
        slider_x.SetActive(false);
        slider_y.SetActive(false);
        active = false;
    }

    public void Show(string img)
    {
        active = true;
        functionTree = imageScript.Function(img);
        slider_x.GetComponent<Slider>().value = 0f;
    }

    private void SetTexts()
    {
        leftText_x.text = "-10";
        rightText_x.text = "10";
        upText_x.text = "x";
        leftText_y.text = "-10";
        rightText_y.text = "10";
        upText_y.text = "y";
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
        float x = (Mathf.Round(slider_x.GetComponent<Slider>().value * 100) / 100f);
        slider_x.GetComponent<Slider>().value = x;

        x_text.text = x.ToString();

        float y = functionTree.Evaluate(x);
        if (float.IsNaN(y) || float.IsInfinity(y) || float.IsNegativeInfinity(y)) slider_y.GetComponent<Slider>().value = 0;
        else if (y < -10) slider_y.GetComponent<Slider>().value = -10;
        else if (y > 10) slider_y.GetComponent<Slider>().value = 10;
        else slider_y.GetComponent<Slider>().value = y;

        if (float.IsNaN(y) || float.IsInfinity(y) || float.IsNegativeInfinity(y)) y_text.text = "NaN";
        else y_text.text = (Mathf.Round(y * 100) / 100f).ToString();
    }
}
