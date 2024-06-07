using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class lineLogicLesson : MonoBehaviour
{
    private enum TouchState
    {
        coordinates, move
    }

    public LineRenderer functionRenderer, axisRenderer_x, axisRenderer_y, tangentRenderer;
    public Camera cam;
    public TextMeshProUGUI upText, downText, leftText, rightText;
    public Material axisMaterial; 
    public GameObject image, coordinateImage, touchStateButton;
    public Vector2 sensibility;
    public int numberOfPoints;

    private binTree.BinaryTree functionTree, functionTangentTree;
    private Vector2 center, rangeOfView;
    private TouchState touchState;
    private bool active, changed, tangentActive;
    private float upExtreme, downExtreme, rightExtreme, leftExtreme;
    private float x0;

    private void Start()
    {

        center = Vector2.zero;
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (sceneIndex == 8) rangeOfView = Vector2.one * 10f;
        else rangeOfView = new Vector2(10f, 5f);


        changed = true;
        tangentActive = false;
        touchState = TouchState.coordinates;

        SetColors();
    }

   
    private void Update()
    {
        if (active)
        {
            touchStateButton.SetActive(true);
            HandleInput();
            if (!changed) return;

            DrawAxis();
            DrawFunction();
            UpdateTexts();
        }
    }

    public void Hide()
    {
        functionRenderer.positionCount = 0;
        tangentRenderer.positionCount = 0;
        axisRenderer_x.positionCount = 0;
        axisRenderer_y.positionCount = 0;
        upText.text = "";
        downText.text = "";
        leftText.text = "";
        rightText.text = "";
        active = false;
        tangentActive = false;
        coordinateImage.SetActive(false);
        touchStateButton.SetActive(false);
    }

    public void Show(string img)
    {
        active = true;
        tangentActive = false;
        functionTree = imageScript.Function(img);
    }

    public void ShowTangent(string img)
    {
        active = true;
        tangentActive = true;
        string main = img.Substring(0, img.IndexOf('|'));
        string tangent = img.Substring(img.IndexOf('|') + 1);
        functionTree = imageScript.Function(main);
        functionTangentTree = imageScript.Function(tangent);
    }

    public void ChangeTouch()
    {
        switch (touchState)
        {
            case TouchState.coordinates:
                touchState = TouchState.move;
                ColorBlock colors = touchStateButton.GetComponent<Button>().colors;
                colors.normalColor = Color.gray;
                touchStateButton.GetComponent<Button>().colors = colors;
                break;
            case TouchState.move:
                touchState = TouchState.coordinates;
                ColorBlock colors2 = touchStateButton.GetComponent<Button>().colors;
                colors2.normalColor = Color.red;
                touchStateButton.GetComponent<Button>().colors = colors2;
                break;
        }
    }


    private void DrawAxis()
    {
        ComputeExtremes();

        RectTransform imageTransform = image.GetComponent<RectTransform>();
        float imageLeft = imageTransform.position.x - imageTransform.sizeDelta.x / 2f;
        float imageRight = imageTransform.position.x + imageTransform.sizeDelta.x / 2f;
        float imageTop = imageTransform.position.y + imageTransform.sizeDelta.y / 2f;
        float imageDown = imageTransform.position.y - imageTransform.sizeDelta.y / 2f;

        if (upExtreme >= 0 && 0 >= downExtreme) DrawX(Mathf.Lerp(imageDown, imageTop, -downExtreme / (upExtreme - downExtreme)) - imageTransform.position.y);
        else axisRenderer_x.positionCount = 0;

        if (rightExtreme >= 0 && 0 >= leftExtreme) DrawY(Mathf.Lerp(imageLeft, imageRight, -leftExtreme / (rightExtreme - leftExtreme)) - imageTransform.position.x);
        else axisRenderer_y.positionCount = 0;
    }
    
    private void DrawX(float pos)
    {
        axisRenderer_x.positionCount = 2;
        RectTransform imageTransform = image.GetComponent<RectTransform>();
        Vector3 leftScreen = CanvasToWorld(imageTransform.position - Vector3.right * imageTransform.sizeDelta.x/2f + pos * Vector3.up);
        Vector3 rightScreen = CanvasToWorld(imageTransform.position + Vector3.right * imageTransform.sizeDelta.x/ 2f + pos * Vector3.up);
        axisRenderer_x.SetPosition(0, leftScreen);
        axisRenderer_x.SetPosition(1, rightScreen);
    }

    private void DrawY(float pos)
    {
        axisRenderer_y.positionCount = 2;
        RectTransform imageTransform = image.GetComponent<RectTransform>();
        Vector3 downScreen = CanvasToWorld(imageTransform.position - Vector3.up * imageTransform.sizeDelta.y/2f + pos * Vector3.right);
        Vector3 upScreen = CanvasToWorld(imageTransform.position + Vector3.up * imageTransform.sizeDelta.y/2f + pos * Vector3.right);
        axisRenderer_y.SetPosition(0, downScreen);
        axisRenderer_y.SetPosition(1, upScreen);
    }
    
    private void DrawFunction()
    {
        ComputeExtremes();

        int j = 0;
        int jt = 0;
        Vector3[] positions = new Vector3[numberOfPoints];
        Vector3[] tangentPositions = new Vector3[numberOfPoints];
    

        for (int i = 0; i < numberOfPoints; i++)
        {
            float x = Mathf.Lerp(leftExtreme, rightExtreme, (float)i / (float)(numberOfPoints - 1));
            float y = functionTree.Evaluate(x);
            if (float.IsNaN(y)) continue;
            if (y > upExtreme) continue;
            else if (y < downExtreme) continue;
            RectTransform imageTransform = image.GetComponent<RectTransform>();
            float xPos = Mathf.Lerp(imageTransform.position.x - imageTransform.sizeDelta.x / 2f, imageTransform.position.x + imageTransform.sizeDelta.x / 2f, (x - leftExtreme) / (rightExtreme - leftExtreme));
            float yPos = Mathf.Lerp(imageTransform.position.y - imageTransform.sizeDelta.y / 2f, imageTransform.position.y + imageTransform.sizeDelta.y / 2f, (y - downExtreme) / (upExtreme - downExtreme));
            positions[j++] = CanvasToWorld(new Vector3(xPos, yPos, 0f));
        }
        if (tangentActive)
        {
            for (int i = 0; i < numberOfPoints; i++)
            {
                float x = Mathf.Lerp(leftExtreme, rightExtreme, (float)i / (float)(numberOfPoints - 1));
                float y = functionTangentTree.Evaluate(x, x0);
                if (float.IsNaN(y)) continue;
                if (y > upExtreme) continue;
                else if (y < downExtreme) continue;
                RectTransform imageTransform = image.GetComponent<RectTransform>();
                float xPos = Mathf.Lerp(imageTransform.position.x - imageTransform.sizeDelta.x / 2f, imageTransform.position.x + imageTransform.sizeDelta.x / 2f, (x - leftExtreme) / (rightExtreme - leftExtreme));
                float yPos = Mathf.Lerp(imageTransform.position.y - imageTransform.sizeDelta.y / 2f, imageTransform.position.y + imageTransform.sizeDelta.y / 2f, (y - downExtreme) / (upExtreme - downExtreme));
                tangentPositions[jt++] = CanvasToWorld(new Vector3(xPos, yPos, 0f));
            }
        }

        functionRenderer.positionCount = j;
        for (int i = 0; i < j; i++)
            functionRenderer.SetPosition(i, positions[i]);

        tangentRenderer.positionCount = jt;
        for (int i = 0; i < jt; i++)
            tangentRenderer.SetPosition(i, tangentPositions[i]);


    }

    private void HandleInput()
    {
        switch (touchState)
        {
            case TouchState.coordinates:
                coordinateImage.SetActive(false);
                if (Input.touchCount == 1)
                {
                    RectTransform imageTransform = image.GetComponent<RectTransform>();
                    Touch touch = Input.GetTouch(0);
                    float imageLeft = imageTransform.position.x - imageTransform.sizeDelta.x / 2f;
                    float imageRight = imageTransform.position.x + imageTransform.sizeDelta.x / 2f;
                    float imageTop = imageTransform.position.y + imageTransform.sizeDelta.y / 2f;
                    float imageDown = imageTransform.position.y - imageTransform.sizeDelta.y / 2f;
                    if (touch.position.x < imageLeft) return;
                    if (touch.position.x > imageRight) return;
                    if (touch.position.y < imageDown) return;
                    if (touch.position.y > imageTop) return;
                    float x = Mathf.Lerp(leftExtreme, rightExtreme, (touch.position.x - imageLeft) / (imageRight - imageLeft));
                    float y = functionTree.Evaluate(x);
                    coordinateImage.GetComponentInChildren<TextMeshProUGUI>().text = "(" + (Mathf.Round(x * 10) / 10f).ToString() + ", " + (Mathf.Round(y * 10f) / 10f).ToString() + ")";
                    float yPos;
                    if (float.IsNaN(y) || float.IsInfinity(y) || float.IsNegativeInfinity(y)) return;
                    if (y > upExtreme) yPos = imageTop;
                    else if (y < downExtreme) yPos = imageDown;
                    else yPos = Mathf.Lerp(imageDown, imageTop, (y - downExtreme) / (upExtreme - downExtreme));

                    coordinateImage.SetActive(true);
                    coordinateImage.transform.position = new Vector3(touch.position.x, yPos, 0f);
                    if (tangentActive)
                    {
                        x0 = x;
                    }
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

    private Vector3 CanvasToWorld(Vector3 canvasPos)
    {
        return cam.ScreenToWorldPoint(canvasPos - cam.transform.position.z * Vector3.forward);
    }

    private void SetColors()
    {
        ColorBlock colors = touchStateButton.GetComponent<Button>().colors;
        colors.normalColor = Color.red;
        touchStateButton.GetComponent<Button>().colors = colors;

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
