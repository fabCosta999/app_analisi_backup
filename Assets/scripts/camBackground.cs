using UnityEngine;
using UnityEngine.UI;

public class camBackground : MonoBehaviour
{
    public Camera cam;
    public Button lightButton;

    private void Start()
    {
        if (!PlayerPrefs.HasKey(keysGeneral.LIGHTMODE)) PlayerPrefs.SetInt(keysGeneral.LIGHTMODE, (int) keysGeneral.lightMode.light);
        if (PlayerPrefs.GetInt(keysGeneral.LIGHTMODE) == (int)keysGeneral.lightMode.light)
        {
            cam.backgroundColor = Color.white;
            lightButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("img/luna");
            lightButton.GetComponent<Image>().color = Color.black;
        }
        else
        {
            cam.backgroundColor = Color.black;
            lightButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("img/sole");
            lightButton.GetComponent<Image>().color = Color.white;
        }
    }

    public void ChangeMode()
    {
        if (PlayerPrefs.GetInt(keysGeneral.LIGHTMODE) == (int)keysGeneral.lightMode.light)
        {
            PlayerPrefs.SetInt(keysGeneral.LIGHTMODE, (int)keysGeneral.lightMode.dark);
            cam.backgroundColor = Color.black;
            lightButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("img/sole");
            lightButton.GetComponent<Image>().color = Color.white;
        }
        else
        {
            PlayerPrefs.SetInt(keysGeneral.LIGHTMODE, (int)keysGeneral.lightMode.light);
            cam.backgroundColor = Color.white;
            lightButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("img/luna");
            lightButton.GetComponent<Image>().color = Color.black;
        }

    }
}
