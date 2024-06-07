using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class lessoncript : MonoBehaviour
{
    private enum GraphType
    {
        graph, slider
    }


    public GameObject buttonPrefab, textArea, image, buttonPopupForward, buttonPopupBack, PopUp, graphTypeButton;
    public TextMeshProUGUI mainText, subText, imageText;
    public lineLogicLesson functionGraph;
    public sliderLogicLesson functionSlider;

    private Dictionary<int, string> texts;
    private Dictionary<int, string> popups;
    private Dictionary<int, string> images;
    private GameObject[] buttons;
    private int numOfPages, actualPage, actualPopupIndex, actualPopUpCount;
    private string[] actualPopup;
    private GraphType graphType;

    private void Start()
    {
        texts = new Dictionary<int, string>();
        popups = new Dictionary<int, string>();
        images = new Dictionary<int, string>();
        actualPopUpCount = 0;
        PopUp.GetComponent<Image>().sprite = PlayerPrefs.GetInt(keysGeneral.LIGHTMODE) == (int) keysGeneral.lightMode.light ?
            Resources.Load<Sprite>("img/vignetta") :
            Resources.Load<Sprite>("img/vignettanotte");

        subText.color = PlayerPrefs.GetInt(keysGeneral.LIGHTMODE) == (int)keysGeneral.lightMode.light ?
            Color.black :
            Color.white;


        ReadFile("Files/" + PlayerPrefs.GetInt(keysGeneral.TOPIC).ToString() + "_" + PlayerPrefs.GetInt(keysGeneral.LESSON).ToString());
        CreateButtons();
        
        actualPage = 0;
        graphType = GraphType.graph;
        actualPopupIndex = 0;
        
        UpdateTexts();
    }

    private void ReadFile(string path)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(path);
        string text = textAsset.text;
        text = text.Replace('\n', ' ');
        text = text.Replace("\r", "");
        text = text.Replace("  ", " ");
        text = Regex.Replace(text, @"\s*<math>\s*", "\n<font=latinmodern-math SDF>");
        text = Regex.Replace(text, @"\s*</math>\s*", "</font>\n");
        string[] rows = text.Split('#', System.StringSplitOptions.RemoveEmptyEntries);
        char[] brackets = { '[', ']', '{', '}' };


        numOfPages = int.Parse(rows[0].Trim(brackets));
        int page = 0;

        for (int i = 1; i < rows.Length; i++)
        {
            rows[i] = rows[i].Trim();
            if (rows[i][0] == '{') page = int.Parse(rows[i].Trim(brackets));
            else if (rows[i] == keysGeneral.TEXT) texts.Add(page, keysGeneral.ReplaceMath(rows[i + 1]).Trim());
            else if (rows[i] == keysGeneral.POPUP) popups.Add(page, keysGeneral.ReplaceMath(rows[i + 1]));
            else if (rows[i] == keysGeneral.IMAGE) images.Add(page, rows[i + 1].Trim());
        }
    }

    private void ButtonHandler(int page)
    {
        functionGraph.Hide();
        actualPage = page;
        actualPopupIndex = 0;
        actualPopUpCount = 0;
        UpdateTexts();
    }

    public void PopUpButton(int direction)
    {
        actualPopupIndex += direction;
        UpdateTexts();
    }

    public void changeGraphType()
    {
        switch (graphType)
        {
            case GraphType.graph:
                graphType = GraphType.slider;
                graphTypeButton.GetComponentInChildren<TextMeshProUGUI>().text = "grafico";
                break;
            case GraphType.slider:
                graphType = GraphType.graph;
                graphTypeButton.GetComponentInChildren<TextMeshProUGUI>().text = "slider";
                break;
        }
        UpdateTexts();
    }

    private void UpdateTexts()
    {
        for (int i=0; i<numOfPages; i++)
            buttons[i].GetComponent<Image>().color = i == actualPage ? Color.blue : Color.grey;

        UpdateMainTexts();
        UpdatePopups();
        UpdateImages();
    }

    private void UpdateMainTexts()
    {
        if (texts.ContainsKey(actualPage))
        {
            textArea.SetActive(true);
            mainText.text = texts[actualPage];
        }
        else
        {
            textArea.SetActive(false);
            mainText.text = "";
        }
    }

    private void UpdatePopups()
    {
        if (popups.ContainsKey(actualPage))
        {
            PopUp.SetActive(false);
            buttonPopupForward.SetActive(false);
            buttonPopupBack.SetActive(false);
            actualPopup = popups[actualPage].Split('|', System.StringSplitOptions.RemoveEmptyEntries);
            actualPopUpCount = actualPopup.Length;
            if (actualPopupIndex < actualPopUpCount)
            {
                subText.text = actualPopup[actualPopupIndex].Replace("[abs]", "|").Trim();
                buttonPopupForward.SetActive(true);
                PopUp.SetActive(true);
            }
            if (actualPopupIndex > 0) buttonPopupBack.SetActive(true);
        }
        else
        {
            PopUp.SetActive(false);
            buttonPopupBack.SetActive(false);
            buttonPopupForward.SetActive(false);
            subText.text = "";
        }
    }


    private void UpdateImages()
    {
        functionGraph.Hide();
        functionSlider.Hide();
        graphTypeButton.SetActive(false);

        if (images.ContainsKey(actualPage))
        {
            image.SetActive(true);
            imageText.text = "";
            image.GetComponent<Image>().color = Color.white;
            image.GetComponent<Image>().sprite = null;
            string tag = images[actualPage].Substring(0, images[actualPage].IndexOf(';'));
            string img = images[actualPage].Substring(images[actualPage].IndexOf(';') + 1);
            switch (imageScript.IMAGE_TYPE[tag])
            {
                case imageScript.image_type.image:
                    Sprite sprite = Resources.Load<Sprite>("img/i" + img);
                    if (sprite != null)
                        image.GetComponent<Image>().sprite = sprite;
                    else
                        image.GetComponent<Image>().sprite = Resources.Load<Sprite>("img/notFound");
                    break;
                case imageScript.image_type.table:
                    string text = Resources.Load<TextAsset>("tab/t" + img).text;
                    string[,] tab = imageScript.ReadTable(text);
                    imageText.text = imageScript.WriteTab(tab);
                    break;
                case imageScript.image_type.truthTable:
                    string[,] truthTab = imageScript.TruthTable(img);
                    imageText.text = imageScript.WriteTab(truthTab);
                    break;
                case imageScript.image_type.graphOnly:
                    image.SetActive(false);
                    functionGraph.Show(img);
                    break;
                case imageScript.image_type.animation:
                    image.SetActive(false);
                    functionSlider.Show(img);
                    break;
                case imageScript.image_type.graph:
                    image.SetActive(false);
                    graphTypeButton.SetActive(true);
                    switch (graphType)
                    {
                        case GraphType.graph:
                            functionSlider.Hide();
                            functionGraph.Show(img);
                            break;
                        case GraphType.slider:
                            functionGraph.Hide();
                            functionSlider.Show(img);
                            break;
                    }
                    break;
                case imageScript.image_type.formula:
                    imageText.enableWordWrapping = false;
                    imageText.text = imageScript.Formula(img);
                    break;
                case imageScript.image_type.graphTangent:
                    image.SetActive(false);
                    functionGraph.ShowTangent(img);
                    break;
                default:
                    image.GetComponent<Image>().sprite = Resources.Load<Sprite>("img/notFound");
                    break;
            }
        }
        else image.SetActive(false);
    }

    private void CreateButtons()
    {
        buttons = new GameObject[numOfPages];

        float y = -460;
        float x = 0;
        int num = 0;
        int cnt = 0;
        if (numOfPages % 2 != 0)
        {
            int i = numOfPages / 2;
            buttons[i] = (GameObject)Instantiate<GameObject>(buttonPrefab);
            buttons[i].transform.SetParent(transform);
            buttons[i].transform.localPosition = new Vector3(0f, y, 0f);
            buttons[i].GetComponent<Button>().onClick.AddListener(() => ButtonHandler(i));
            num = i - 1;
            cnt = 2;
            x = -60;
            numOfPages--;
        }
        else
        {
            x = -30;
            num = numOfPages / 2 - 1;
            cnt = 1;
        }
        while (numOfPages > 0)
        {
            int i = num;
            buttons[i] = (GameObject)Instantiate<GameObject>(buttonPrefab);
            buttons[i].transform.SetParent(transform);
            buttons[i].transform.localPosition = new Vector3(x, y, 0f);
            buttons[i].GetComponent<Button>().onClick.AddListener(() => ButtonHandler(i));

            int j = num + cnt;
            buttons[j] = (GameObject)Instantiate<GameObject>(buttonPrefab);
            buttons[j].transform.SetParent(transform);
            buttons[j].transform.localPosition = new Vector3(-x, y, 0f);
            buttons[j].GetComponent<Button>().onClick.AddListener(() => ButtonHandler(j));

            num--;
            cnt += 2;
            numOfPages -= 2;
            x -= 60;
        }

        numOfPages = buttons.Length;
    }
}
