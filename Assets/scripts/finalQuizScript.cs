using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class finalQuiz : MonoBehaviour
{

    private enum GraphType
    {
        graph, slider
    }

    public GameObject buttonLeft, buttonRigth, answerBox, image, graphTypeButton;
    public TextMeshProUGUI questionText, answerText, imageText;
    public TMP_InputField answerGiven;
    public binTree.BinaryTree functionTree;
    public lineLogicLesson functionGraph;
    public sliderLogicLesson functionSlider;
   

    private Dictionary<int, string> questions;
    private Dictionary<int, string> answers;
    private Dictionary<int, string> completeAnswers;
    private Dictionary<int, string> images;
    private Dictionary<int, bool> answersGiven;
    private Dictionary<int, string> hiddenImages;
    private int actualPage, numOfPages;
    private GraphType graphType;

    private void Start()
    {
        questions = new Dictionary<int, string>();
        answers = new Dictionary<int, string>();
        images = new Dictionary<int, string>();
        hiddenImages = new Dictionary<int, string>();
        answersGiven = new Dictionary<int, bool>();
        completeAnswers = new Dictionary<int, string>();

        string path = "Files/q" + PlayerPrefs.GetInt(keysGeneral.TOPIC).ToString();
        ReadFile(path);
        
        graphType = GraphType.graph;


        UpdateTexts();
    }

    public void ChangePage(int page)
    {
        actualPage += page;
        answerGiven.text = "";
        UpdateTexts();
    }

    public void SubmitAnswer()
    {
        if (!answersGiven.ContainsKey(actualPage))
        {
            answersGiven.Add(actualPage, answerGiven.text.Trim().Replace("\u200B", "").Equals(answers[actualPage].Trim(), StringComparison.OrdinalIgnoreCase));
            UpdateTexts();
        }
    }

    private void ReadFile(string path)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(path);
        string text = textAsset.text;
        text = text.Replace('\n', ' ');
        text = text.Replace("\r", "");
        text = text.Replace("  ", " ");
        string[] rows = text.Split('#', StringSplitOptions.RemoveEmptyEntries);
        char[] brackets = { '[', ']', '{', '}' };

        numOfPages = int.Parse(rows[0].Trim(brackets));
        int actualRow = 1;
        for (int i = 0; i < numOfPages; i++)
        {
            if (rows[actualRow].Contains(keysGeneral.IMAGE))
            {
                string[] strings = rows[actualRow].Split(keysGeneral.IMAGE, StringSplitOptions.RemoveEmptyEntries);
                images.Add(i, strings[1]);
                rows[actualRow] = strings[0];
            }
            questions.Add(i, rows[actualRow]);
            answers.Add(i, rows[actualRow + 1]);
            if (rows[actualRow + 2].Contains(keysGeneral.IMAGE))
            {
                string[] strings = rows[actualRow + 2].Split(keysGeneral.IMAGE, StringSplitOptions.RemoveEmptyEntries);
                hiddenImages.Add(i, strings[1]);
                rows[actualRow + 2] = strings[0];
            }
            completeAnswers.Add(i, rows[actualRow + 2]);
            actualRow += 3;
        }
    }

    private void UpdateTexts()
    {
        questionText.text = questions[actualPage].Trim();
        UpdateButtons();
        UpdateImages();
    }

    private void UpdateButtons()
    {
        if (actualPage == 0) buttonLeft.SetActive(false);
        else buttonLeft.SetActive(true);

        if (actualPage == numOfPages - 1 || !answersGiven.ContainsKey(actualPage)) buttonRigth.SetActive(false);
        else buttonRigth.SetActive(true);

        if (answersGiven.ContainsKey(actualPage))
        {
            answerBox.GetComponent<Image>().color = answersGiven[actualPage] ? Color.green : Color.red;
            answerText.text = completeAnswers[actualPage].Trim();
        }
        else
        {
            answerBox.GetComponent<Image>().color = Color.gray;
            answerText.text = "";
        }
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

    private void UpdateImages()
    {
        functionGraph.Hide();
        functionSlider.Hide();
        graphTypeButton.SetActive(false);
        if (images.ContainsKey(actualPage) || (hiddenImages.ContainsKey(actualPage) && answersGiven.ContainsKey(actualPage)))
        {
            image.SetActive(true);
            imageText.text = "";
            image.GetComponent<Image>().color = Color.white;
            string tag;
            string img;
            if (images.ContainsKey(actualPage))
            {
                tag = images[actualPage].Substring(0, images[actualPage].IndexOf(';'));
                img = images[actualPage].Substring(images[actualPage].IndexOf(';') + 1);
            }
            else
            {
                tag = hiddenImages[actualPage].Substring(0, hiddenImages[actualPage].IndexOf(';'));
                img = hiddenImages[actualPage].Substring(hiddenImages[actualPage].IndexOf(';') + 1);
            }
            try
            {
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
                        string text = File.ReadAllText("Assets/Resources/tab/t" + img + ".txt");
                        string[,] tab = imageScript.ReadTable(text);
                        imageText.text = imageScript.WriteTab(tab);
                        break;
                    case imageScript.image_type.truthTable:
                        string[,] truthTab = imageScript.TruthTable(img);
                        imageText.text = imageScript.WriteTab(truthTab);
                        break;
                    case imageScript.image_type.formula:
                        imageText.enableWordWrapping = false;
                        imageText.text = imageScript.Formula(img);
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
                    default:
                        image.GetComponent<Image>().sprite = Resources.Load<Sprite>("img/notFound");
                        break;
                }
            }
            catch
            {
                image.GetComponent<Image>().sprite = Resources.Load<Sprite>("img/notFound");
            }
        }
        else image.SetActive(false);
    }
}

