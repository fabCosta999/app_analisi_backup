using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class lineLogic : MonoBehaviour
{
    private enum TouchState
    {
        coordinates, move
    }

    public TextMeshProUGUI upText, downText, leftText, rightText;
    public LineRenderer functionRenderer, axisRenderer_x, axisRenderer_y;
    public GameObject coordinateImage;
    public keyboard inputFunction;
    public Material axisMaterial; // functionMaterial; se voglio cambiare il colore della funzione
    public Button touchStateButton;
    public Camera Cam;
    public Vector2 sensibility;
    public int numberOfPoints;

    private Vector2 center, rangeOfView;
    private TouchState touchState;
    private bool changed;
    private float upExtreme, downExtreme, leftExtreme, rightExtreme;

    private void Start()
    {
        center = Vector2.zero;
        rangeOfView = Vector2.one * 10f;

        changed = true;
        touchState = TouchState.coordinates;
        SetColors();
    }

    private void Update()
    {
        HandleInput();

        if (!changed) return;
        DrawAxis();
        DrawFunction();
        UpdateTexts();
        changed = false;
    }

    private void DrawAxis()
    {
        ComputeExtremes();

        if (upExtreme >= 0 && 0 >= downExtreme) DrawX(Mathf.Lerp(0, Screen.height, -downExtreme/(upExtreme-downExtreme)));
        else axisRenderer_x.positionCount = 0;

        if (rightExtreme >= 0 && 0 >= leftExtreme) DrawY(Mathf.Lerp(0, Screen.width, -leftExtreme / (rightExtreme - leftExtreme)));
        else axisRenderer_y.positionCount = 0;
 
    }

    private void DrawX(float pos)
    {
        axisRenderer_x.positionCount = 2;
        Vector3 leftScreen = new Vector3(0f, pos, 0f);
        Vector3 rightScreen = new Vector3(Screen.width, pos, 0f);
        axisRenderer_x.SetPosition(0, Cam.ScreenToWorldPoint(leftScreen) + Vector3.forward);
        axisRenderer_x.SetPosition(1, Cam.ScreenToWorldPoint(rightScreen) + Vector3.forward);
    }

    private void DrawY(float pos)
    {
        axisRenderer_y.positionCount = 2;
        Vector3 upScreen = new Vector3(pos, 0f, 0f);
        Vector3 downScreen = new Vector3(pos, Screen.height, 0f);
        axisRenderer_y.SetPosition(0, Cam.ScreenToWorldPoint(upScreen) + Vector3.forward);
        axisRenderer_y.SetPosition(1, Cam.ScreenToWorldPoint(downScreen) + Vector3.forward);
    }

    private void DrawFunction()
    {
        ComputeExtremes();
        int j = 0;
        Vector3[] positions = new Vector3[numberOfPoints];

        for (int i=0; i<numberOfPoints; i++)
        {
            float x = Mathf.Lerp(leftExtreme, rightExtreme, (float)i / (float)(numberOfPoints - 1));
            float y = inputFunction.Function(x);
            if (float.IsNaN(y)) continue;
            if (y > upExtreme) continue;
            else if (y < downExtreme) continue;

            int xPixel = (int)Mathf.Lerp(0, Screen.width, (x - leftExtreme) / (rightExtreme - leftExtreme));
            int yPixel = (int)Mathf.Lerp(0, Screen.height, (y - downExtreme) / (upExtreme - downExtreme));
            positions[j++] = Cam.ScreenToWorldPoint(new Vector3(xPixel, yPixel, 0f)) + Vector3.forward;
        }

        functionRenderer.positionCount = j;
        for (int i=0; i<j; i++)
            functionRenderer.SetPosition(i, positions[i]);
    }

    private void HandleInput()
    {
        coordinateImage.SetActive(false);
        switch (touchState)
        {
            case TouchState.coordinates:
                if (Input.touchCount == 1)
                {
                    Touch touch = Input.GetTouch(0);
                    if (touch.position.y < Screen.height / 2) return;
                    float x = Mathf.Lerp(leftExtreme, rightExtreme, touch.position.x / Screen.width);
                    float y = inputFunction.Function(x);
                    coordinateImage.GetComponentInChildren<TextMeshProUGUI>().text = "(" + (Mathf.Round(x * 10) / 10f).ToString() + ", " + (Mathf.Round(y * 10f) / 10f).ToString() + ")";
                    float yPos;
                    if (float.IsNaN(y) || float.IsInfinity(y) || float.IsNegativeInfinity(y)) return;
                    if (y > upExtreme) yPos = Screen.height;
                    else if (y < downExtreme) yPos = 0;
                    else yPos = Mathf.Lerp(0, Screen.height, (y - downExtreme) / (upExtreme - downExtreme));

                    coordinateImage.SetActive(true);
                    coordinateImage.transform.position = new Vector3(touch.position.x, yPos, 0f);
                }
                break;
            case TouchState.move:
                if (sensibility.y >= 1f) sensibility.y = 0.9f;

                if (Input.touchCount == 1)
                {
                    Touch touch = Input.GetTouch(0);

                    if (touch.phase == TouchPhase.Moved)
                    {
                        Vector2 delta = touch.deltaPosition;
                        center -= delta * sensibility.x;
                        changed = true;
                    }
                }

                else if (Input.touchCount == 2)
                {
                    Touch touch1 = Input.GetTouch(0);
                    Touch touch2 = Input.GetTouch(1);

                    Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;
                    Vector2 touch2PrevPos = touch2.position - touch2.deltaPosition;

                    float prevTouchDeltaMag = (touch1PrevPos - touch2PrevPos).magnitude;
                    float touchDeltaMag = (touch1.position - touch2.position).magnitude;

                    float deltaMagDiff = prevTouchDeltaMag - touchDeltaMag;

                    if (deltaMagDiff < 0) rangeOfView *= Vector2.one * sensibility.y;
                    else rangeOfView /= Vector2.one * sensibility.y;
                    changed = true;
                }
                break;
        }

    }

    public void ChangeTouch()
    {
        switch (touchState)
        {
            case TouchState.coordinates:
                touchState = TouchState.move;
                ColorBlock colors = touchStateButton.colors;
                colors.normalColor = Color.gray;
                touchStateButton.colors = colors;
                break;
            case TouchState.move:
                touchState = TouchState.coordinates;
                ColorBlock colors2 = touchStateButton.colors;
                colors2.normalColor = Color.red;
                touchStateButton.colors = colors2;
                break;
        }
    }
    public void newFunction()
    {
        changed = true;
    }

    public void Center()
    {
        center = new Vector2(0f, 0f);
        rangeOfView = new Vector2(10f, 10f);
        changed = true;
    }

    private void SetColors()
    {
        if (PlayerPrefs.GetInt(keysGeneral.LIGHTMODE) == (int)keysGeneral.lightMode.light)
        {
            upText.color = Color.black;
            downText.color = Color.black;
            rightText.color = Color.black;
            leftText.color = Color.black;
            axisMaterial.color = Color.black;
            axisMaterial.SetColor("_EmissionColor", Color.black);
        }
        else
        {
            upText.color = Color.white;
            downText.color = Color.white;
            rightText.color = Color.white;
            leftText.color = Color.white;
            axisMaterial.color = Color.white;
            axisMaterial.SetColor("_EmissionColor", Color.white);
        }
    }

    private void UpdateTexts()
    {
        upText.text = (Mathf.Round(upExtreme * 100) / 100f).ToString();
        downText.text = (Mathf.Round(downExtreme * 100) / 100f).ToString();
        rightText.text = (Mathf.Round(rightExtreme * 100) / 100f).ToString();
        leftText.text = (Mathf.Round(leftExtreme * 100) / 100f).ToString();
    }

    private void ComputeExtremes()
    {
        upExtreme = center.y + rangeOfView.y;
        downExtreme = center.y - rangeOfView.y;
        rightExtreme = center.x + rangeOfView.x;
        leftExtreme = center.x - rangeOfView.x;
    }
}