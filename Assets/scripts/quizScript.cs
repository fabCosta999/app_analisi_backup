using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class quizScript : MonoBehaviour
{
    public GameObject buttonPrefab, image;
    public GameObject[] answerButtons;
    public TextMeshProUGUI questionText, imageText;
    public binTree.BinaryTree functionTree;
    public lineLogicLesson functionGraph;
    public sliderLogicLesson functionSlider;

    public class Quiz
    {
        public Dictionary<int, string> questions;
        public Dictionary<int, string[]> answers;
        public Dictionary<int, int> correctAnswers;
        public Dictionary<int, string> images;
        public int numOfQuizzes;

        public Quiz()
        {
            questions = new Dictionary<int, string>();
            answers = new Dictionary<int, string[]>();
            correctAnswers = new Dictionary<int, int>();
            images = new Dictionary<int, string>();
            numOfQuizzes = 0;
        }

        public static Quiz operator +(Quiz self, Quiz other)
        {
            foreach (int i in other.questions.Keys) self.questions.Add(i + self.numOfQuizzes, other.questions[i]);
            foreach (int i in other.answers.Keys) self.answers.Add(i + self.numOfQuizzes, other.answers[i]);
            foreach (int i in other.correctAnswers.Keys) self.correctAnswers.Add(i + self.numOfQuizzes, other.correctAnswers[i]);
            foreach (int i in other.images.Keys) self.images.Add(i + self.numOfQuizzes, other.images[i]);
            self.numOfQuizzes += other.numOfQuizzes;
            other = null;
            return self;
        }
    }

    private Quiz quiz;
    private int actualPage, numOfPages;
    private GameObject[] pageButtons;
    private Dictionary<int, int> answerGiven;
    private int[] quizzes;

    private void Start()
    {
        string path = "Files/q" + PlayerPrefs.GetInt(keysGeneral.TOPIC).ToString() + "_" + PlayerPrefs.GetInt(keysGeneral.LESSON).ToString();

        quiz = ReadFile(path);
        if (quiz == null) return;

        CreateQuiz();
        CreateButtons();
        
        actualPage = 0;
        UpdateTexts();
    }

    public void Answer(int answer)
    {
        if (!answerGiven.ContainsKey(actualPage))
        {
            answerGiven[actualPage] = answer;
            UpdateTexts();
        }
    }

    private void ButtonHandler(int page)
    {
        actualPage = page;
        UpdateTexts();
    }

    private void UpdateTexts()
    {
        questionText.text = keysGeneral.ReplaceMath(quiz.questions[quizzes[actualPage]]);
        UpdateButtons();
        UpdateImages();
    }

    private void UpdateButtons()
    {
        for (int i = 0; i < numOfPages; i++)
            pageButtons[i].GetComponent<Image>().color = i == actualPage ? Color.blue : Color.grey;

        for (int i = 0; i < quiz.answers[quizzes[actualPage]].Length; i++)
        {
            answerButtons[i].SetActive(true);
            answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = keysGeneral.ReplaceMath(quiz.answers[quizzes[actualPage]][i]);
            if (answerGiven.ContainsKey(actualPage))
            {
                if (i == quiz.correctAnswers[quizzes[actualPage]]) answerButtons[i].GetComponent<Image>().color = Color.green;
                else if (i == answerGiven[actualPage]) answerButtons[i].GetComponent<Image>().color = Color.red;
                else answerButtons[i].GetComponent<Image>().color = Color.red;
            }
            else answerButtons[i].GetComponent<Image>().color = Color.grey;
        }
        for (int i = quiz.answers[quizzes[actualPage]].Length; i < 5; i++)
            answerButtons[i].SetActive(false);
    }

    private void UpdateImages()
    {
        functionGraph.Hide();
        functionSlider.Hide();
        if (quiz.images.ContainsKey(quizzes[actualPage]))
        {
            image.SetActive(true);
            imageText.text = "";
            image.GetComponent<Image>().color = Color.white;
            string tag = quiz.images[quizzes[actualPage]].Substring(0, quiz.images[quizzes[actualPage]].IndexOf(';'));
            string img = quiz.images[quizzes[actualPage]].Substring(quiz.images[quizzes[actualPage]].IndexOf(';') + 1);
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
                        string text = Resources.Load<TextAsset>("tab/t" + img).text;
                        string[,] tab = imageScript.ReadTable(text);
                        imageText.text = keysGeneral.ReplaceMathInImage(imageScript.WriteTab(tab));
                        break;
                    case imageScript.image_type.truthTable:
                        string[,] truthTab;
                        if (img.Contains(imageScript.TRUTHTABLE_INCORRECT)) truthTab = imageScript.TruthTable(img, imageScript.truthTableMode.incorrect);
                        else if (img.Contains(imageScript.TRUTHTABLE_MISSING)) truthTab = imageScript.TruthTable(img, imageScript.truthTableMode.missing);
                        else if (img.Contains(imageScript.TRUTHTABLE_WITHOUT)) truthTab = imageScript.TruthTable(img, imageScript.truthTableMode.whitoutTitle);
                        else truthTab = imageScript.TruthTable(img);
                        imageText.text = keysGeneral.ReplaceMathInImage(imageScript.WriteTab(truthTab));
                        break;
                    case imageScript.image_type.formula:
                        imageText.enableWordWrapping = false;
                        imageText.text = keysGeneral.ReplaceMathInImage(imageScript.Formula(img));
                        break;
                    case imageScript.image_type.graphOnly:
                        image.SetActive(false);
                        functionGraph.Show(img);
                        break;
                    case imageScript.image_type.animation:
                        image.SetActive(false);
                        functionSlider.Show(img);
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
        else
        {
            imageText.text = "";
            image.SetActive(false);
        }
    }

    private string Format(string str)
    {
        string newStr = Regex.Replace(str, @"n[0-9]", match => UnityEngine.Random.Range(-9, 10).ToString());
        newStr = Regex.Replace(newStr, @"N[0-9]", match => UnityEngine.Random.Range(2, 10).ToString());
        newStr = Regex.Replace(newStr, @"p[0-9]", match => (UnityEngine.Random.Range(1, 6) * 2).ToString());
        newStr = Regex.Replace(newStr, @"d[0-9]", match => (UnityEngine.Random.Range(0, 5) * 2 + 1).ToString());
        newStr = newStr.Replace("dis/pari", UnityEngine.Random.value >= 0.5f ? "pari" : "dispari");
        newStr = newStr.Replace("grande/piccolo", UnityEngine.Random.value >= 0.5f ? "grande" : "piccolo");
        newStr = newStr.Replace("sin/cos", UnityEngine.Random.value >= 0.5f ? "sin" : "cos");
        if (newStr.Contains("<>"))
        {
            float r = UnityEngine.Random.value;
            if (r < 0.2) newStr = newStr.Replace("<>", "<");
            else if (r < 0.4) newStr = newStr.Replace("<>", "[<=]");
            else if (r < 0.6) newStr = newStr.Replace("<>", ">");
            else if (r < 0.8) newStr = newStr.Replace("<>", "[>=]");
            else newStr = newStr.Replace("<>", "=");
        }
        if (newStr.Contains("_int_"))
        {
            int n1 = UnityEngine.Random.Range(-10, 10);
            int n2 = UnityEngine.Random.Range(2, 10) + n1;
            string str1 = n1.ToString() + ", " + n2.ToString();
            newStr = newStr.Replace("_int_", str1);
        }
        return newStr;
    }

    private Quiz ReadFile(string path)
    {
        string text = Resources.Load<TextAsset>(path).text;
        text = text.Replace('\n', ' ');
        text = text.Replace("\r", "");
        text = text.Replace("  ", " ");
        string[] rows = text.Split('#', StringSplitOptions.RemoveEmptyEntries);
        char[] brackets = { '[', ']', '{', '}' };
        Quiz localQuiz;
        int actualRow;

        switch (keysGeneral.QUIZMODE[rows[0].Trim()])
        {
            case keysGeneral.quizMode.fxd:
                localQuiz = new Quiz();
                localQuiz.numOfQuizzes = int.Parse(rows[1].Trim().Trim(brackets));
                string[] answers = rows[2].Split(';', StringSplitOptions.RemoveEmptyEntries);
                actualRow = 3;
                for (int i = 0; i < localQuiz.numOfQuizzes; i++)
                {
                    string txt = Format(rows[actualRow]);
                    if (txt.Contains(keysGeneral.IMAGE))
                    {
                        string[] strings = txt.Split(keysGeneral.IMAGE);
                        txt = strings[0];
                        string img = strings[1];
                        localQuiz.images.Add(i, img.Trim());
                    }
                    localQuiz.questions.Add(i, txt.Trim());
                    localQuiz.answers.Add(i, answers);
                    localQuiz.correctAnswers.Add(i, int.Parse(rows[actualRow + 1].Trim()));
                    actualRow += 2;
                }
                break;
            case keysGeneral.quizMode.standard:
                localQuiz = new Quiz();
                localQuiz.numOfQuizzes = int.Parse(rows[1].Trim().Trim(brackets));
                actualRow = 2;
                for (int i = 0; i < localQuiz.numOfQuizzes; i++)
                {
                    string txt = Format(rows[actualRow]);
                    if (txt.Contains(keysGeneral.IMAGE))
                    {
                        string[] strings = txt.Split(keysGeneral.IMAGE);
                        txt = strings[0];
                        string img = strings[1];
                        localQuiz.images.Add(i, img.Trim());
                    }
                    localQuiz.questions.Add(i, txt.Trim());

                    string[] ans = rows[actualRow + 1].Split(';', StringSplitOptions.RemoveEmptyEntries);
                    localQuiz.answers.Add(i, ans);
                    localQuiz.correctAnswers.Add(i, int.Parse(rows[actualRow + 2].Trim()));
                    actualRow += 3;
                }
                break;
            case keysGeneral.quizMode.previous:
                localQuiz = new Quiz();
                int numOfFiles = int.Parse(rows[1].Trim().Trim(brackets));
                string[] files = rows[2].Split(';', StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < numOfFiles; i++)
                    localQuiz += ReadFile("Files/q" + PlayerPrefs.GetInt(keysGeneral.TOPIC).ToString() + "_" + files[i].Trim());
                break;
            default:
                return null;
        }

        List<int> keys = new List<int>();
        foreach (int i in localQuiz.questions.Keys)
            keys.Add(i);

        foreach (int i in keys)
        {
            specialQuestions.LogicQuestions(localQuiz, i);
            specialQuestions.FunctionQuestions(localQuiz, i);
            specialQuestions.LimitsQuestions(localQuiz, i);
            specialQuestions.DerivativesQuestions(localQuiz, i);
            specialQuestions.AntiderivativesQuestions(localQuiz, i);
            ReFormat(localQuiz, i);
        }

        return localQuiz;
    }

    private void ReFormat(Quiz localQuiz, int i)
    {
        if (localQuiz.images.ContainsKey(i))
        {
            localQuiz.images[i] = localQuiz.images[i].Replace("+-", "-");
            localQuiz.images[i] = localQuiz.images[i].Replace("1x", "x");
            localQuiz.images[i] = Regex.Replace(localQuiz.images[i], @"0x(<sup>\d+</sup>)?", "0");

        }
        localQuiz.questions[i] = localQuiz.questions[i].Replace("+-", "-");
        localQuiz.questions[i] = localQuiz.questions[i].Replace("1x", "x");
        localQuiz.questions[i] = Regex.Replace(localQuiz.questions[i], @"0x(<sup>\d+</sup>)?", "0");
    }

    private void CreateQuiz()
    {
        answerGiven = new Dictionary<int, int>();
        List<int> alreaySelected = new List<int>();
        numOfPages = 5;
        if (quiz.numOfQuizzes < numOfPages) numOfPages = quiz.numOfQuizzes;
        quizzes = new int[numOfPages];
        for (int i = 0; i < numOfPages; i++)
        {
            int n;
            do n = UnityEngine.Random.Range(0, quiz.numOfQuizzes);
            while (alreaySelected.Contains(n));
            alreaySelected.Add(n);
            quizzes[i] = n;
        }
    }

    private void CreateButtons()
    {
        pageButtons = new GameObject[numOfPages];

        float y = -460;
        float x = 0;
        int num = 0;
        int cnt = 0;
        if (numOfPages % 2 != 0)
        {
            int i = numOfPages / 2;
            pageButtons[i] = (GameObject)Instantiate<GameObject>(buttonPrefab);
            pageButtons[i].transform.SetParent(transform);
            pageButtons[i].transform.localPosition = new Vector3(0f, y, 0f);
            pageButtons[i].GetComponent<Button>().onClick.AddListener(() => ButtonHandler(i));
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
            pageButtons[i] = (GameObject)Instantiate<GameObject>(buttonPrefab);
            pageButtons[i].transform.SetParent(transform);
            pageButtons[i].transform.localPosition = new Vector3(x, y, 0f);
            pageButtons[i].GetComponent<Button>().onClick.AddListener(() => ButtonHandler(i));

            int j = num + cnt;
            pageButtons[j] = (GameObject)Instantiate<GameObject>(buttonPrefab);
            pageButtons[j].transform.SetParent(transform);
            pageButtons[j].transform.localPosition = new Vector3(-x, y, 0f);
            pageButtons[j].GetComponent<Button>().onClick.AddListener(() => ButtonHandler(j));

            num--;
            cnt += 2;
            numOfPages -= 2;
            x -= 60;
        }
        numOfPages = pageButtons.Length;
    }
}