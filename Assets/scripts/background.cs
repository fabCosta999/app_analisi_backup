using UnityEngine;

public class background : MonoBehaviour
{
    public Camera cam;

    private void Start()
    {
        cam.backgroundColor = 
            PlayerPrefs.GetInt(keysGeneral.LIGHTMODE) == (int)keysGeneral.lightMode.light ? 
                Color.white : 
                Color.black;
    }
}
