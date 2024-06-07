using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public static class specialQuestions
{
    private static float ComputeDerivativeCoefficient(float coefficient, int numOfDerivatives, int degree)
    {
        while (numOfDerivatives > 0)
        {
            coefficient *= degree;
            degree--;
            numOfDerivatives--;
        }
        return coefficient;
    }

    private static string GeneratePolinomial(int min, int max, bool minForced)
    {
        List<string> monomials = new List<string>();
        if (minForced)
        {
            monomials.Add(keysGeneral.terms[min]);
            for (int i=min+1; i<=max; i++) if (UnityEngine.Random.value < 0.35f) monomials.Add(keysGeneral.terms[i]);
        }
        else
        {
            monomials.Add(keysGeneral.terms[max]);
            for (int i = max - 1; i > max; i--) if (UnityEngine.Random.value < 0.35f) monomials.Add(keysGeneral.terms[i]);
        }
        return string.Join(UnityEngine.Random.value < 0.5f ? '+' : '-', monomials);
    }

    public static void LogicQuestions(quizScript.Quiz q, int i)
    {
        if (q.images.ContainsKey(i)){
            if (q.images[i].Contains("{log}"))
            {
                string newStr = imageScript.TRUTHTABLE + "; 3; p; q; ";
                float val = UnityEngine.Random.value;
                if (val < 0.17) newStr += "non p";
                else if (val < 0.33) newStr += "non q";
                else if (val < 0.5) newStr += "p e q";
                else if (val < 0.67) newStr += "p o q";
                else if (val < 0.83) newStr += "p im q";
                else newStr += "p eq q";

                if (UnityEngine.Random.value < 0.5)
                {
                    newStr += imageScript.TRUTHTABLE_INCORRECT;
                    float incorrectVal = UnityEngine.Random.value;
                    if (incorrectVal < 0.25) newStr += "0";
                    else if (incorrectVal < 0.5) newStr += "1";
                    else if (incorrectVal < 0.75) newStr += "2";
                    else newStr += "3";
                    q.correctAnswers[i] = 1;
                }
                else q.correctAnswers[i] = 0;

                q.images[i] = q.images[i].Replace("{log}", newStr);
            }

            else if (q.images[i].Contains("{log-}"))
            {
                string newStr = imageScript.TRUTHTABLE + "; 3; p; q; ";
                float val = UnityEngine.Random.value;
                if (val < 0.2) newStr += "non p";
                else if (val < 0.4) newStr += "p e q";
                else if (val < 0.6) newStr += "p o q";
                else if (val < 0.8) newStr += "p im q";
                else newStr += "p eq q";

                newStr += imageScript.TRUTHTABLE_MISSING;
                float missingVal = UnityEngine.Random.value;
                if (missingVal < 0.25)
                {
                    newStr += "0";
                    q.correctAnswers[i] = val < 0.2 ? 1 : 0;
                }
                else if (missingVal < 0.5)
                {
                    newStr += "1";
                    q.correctAnswers[i] = val >= 0.4 && val < 0.6 ? 0 : 1;
                }
                else if (missingVal < 0.75)
                {
                    newStr += "2";
                    q.correctAnswers[i] = (val >= 0.2 && val < 0.4) || val >= 0.8 ? 1 : 0;
                }
                else
                {
                    newStr += "3";
                    q.correctAnswers[i] = val >= 0.2 && val < 0.6 ? 1 : 0;
                }
                q.images[i] = q.images[i].Replace("{log-}", newStr);
            }

            else if (q.images[i].Contains("{log--}"))
            {
                string newStr = imageScript.TRUTHTABLE + "; 3; p; q; ";
                float val = UnityEngine.Random.value;
                if (val < 0.2)
                {
                    newStr += "non p";
                    q.correctAnswers[i] = 2;
                }
                else if (val < 0.4)
                {
                    newStr += "p e q";
                    q.correctAnswers[i] = 0;
                }
                else if (val < 0.6)
                {
                    newStr += "p o q";
                    q.correctAnswers[i] = 1;
                }
                else if (val < 0.8)
                {
                    newStr += "p im q";
                    q.correctAnswers[i] = 3;
                }
                else
                {
                    newStr += "p eq q";
                    q.correctAnswers[i] = 4;
                }

                newStr += imageScript.TRUTHTABLE_WITHOUT;
                q.images[i] = q.images[i].Replace("{log--}", newStr);
            }
        }
    }

    public static void FunctionQuestions(quizScript.Quiz q, int i)
    {
        if (q.answers[i].Length == 1){
            string ans = q.answers[i][0];
            if (ans.Contains("{fun}"))
            {
                int xValue = UnityEngine.Random.Range(-9, 10);
                q.questions[i] = q.questions[i].Replace("f(r)", "f(" + xValue.ToString() + ")");
                Dictionary<string, int> prms = new Dictionary<string, int>();
                string[] values = q.images[i].Split("[val]", StringSplitOptions.RemoveEmptyEntries);
                for (int j = 1; j < 10; j++)
                {
                    string param = "m" + j.ToString();
                    if (q.images[i].Contains(param))
                    {
                        prms.Add(param, UnityEngine.Random.Range(-9, 10));
                        values[0] = values[0].Replace(param, prms[param].ToString());
                        values[1] = values[1].Replace(param, prms[param].ToString());
                    }
                }
                binTree.BinaryTree functionTree = imageScript.Function(values[1]);
                q.images[i] = values[0];
                int answer = Mathf.RoundToInt(functionTree.Evaluate((float)xValue));
                int pos = UnityEngine.Random.Range(0, 5);
                q.correctAnswers[i] = pos;
                string[] answers = new string[5];
                string newAns;
                for (int j = 0; j < 5; j++)
                {
                    if (j == pos) answers[j] = answer.ToString();
                    else
                    {
                        do
                        {
                            int res;
                            do res = answer + UnityEngine.Random.Range(-9, 10);
                            while (res == answer);
                            newAns = res.ToString();
                        } while (answers.Contains<string>(newAns));
                        answers[j] = newAns;
                    }
                }
                q.answers[i] = answers;
            }
            else if (ans.Contains("{fun-}"))
            {
                float r = UnityEngine.Random.value;
                string img = "g;";
                int x;
                float realX;
                string xVal;
                if (r < 0.33)
                {
                    img += "+;^;x;";
                    img += UnityEngine.Random.Range(1, 4).ToString() + ";";
                    img += UnityEngine.Random.Range(-2, 3).ToString();
                    x = UnityEngine.Random.Range(-2, 3);
                    xVal = x.ToString();
                    realX = (float)x;
                }
                else if (r < 0.66)
                {
                    img += UnityEngine.Random.value < 0.5 ? "sin;" : "cos;";
                    img += UnityEngine.Random.Range(-2, 3).ToString() + "x";
                    x = UnityEngine.Random.Range(-2, 3);
                    if (x != 0) xVal = x.ToString() + "pi";
                    else xVal = x.ToString();
                    realX = x * Mathf.PI;
                }
                else
                {
                    img += "abs;";
                    img += UnityEngine.Random.Range(1, 6).ToString() + "x";
                    x = UnityEngine.Random.Range(-2, 3);
                    xVal = x.ToString();
                    realX = (float)x;
                }
                q.images[i] = img;
                q.questions[i] = q.questions[i].Replace("f(r)", "f(" + xVal.Replace("pi", "[pi]").Replace("1[pi]", "[pi]") + ")");
                binTree.BinaryTree functionTree = imageScript.Function(img.Substring(img.IndexOf(';') + 1));

                int answer = Mathf.RoundToInt(functionTree.Evaluate(realX));
                int pos = UnityEngine.Random.Range(0, 5);
                q.correctAnswers[i] = pos;
                string[] answers = new string[5];
                string newAns;
                for (int j = 0; j < 5; j++)
                {
                    if (j == pos) answers[j] = answer.ToString();
                    else
                    {
                        do
                        {
                            int res;
                            do res = answer + UnityEngine.Random.Range(-9, 10);
                            while (Mathf.Abs(res) > 10 || res == answer);
                            newAns = res.ToString();
                        } while (answers.Contains<string>(newAns));
                        answers[j] = newAns;
                    }
                }
                q.answers[i] = answers;

            }
            else if (ans.Contains("{fun--}"))
            {
                float r = UnityEngine.Random.value;
                string img = "a;";
                int x;
                float realX;
                string xVal;
                if (r < 0.33)
                {
                    img += "+;^;x;";
                    img += UnityEngine.Random.Range(1, 4).ToString() + ";";
                    img += UnityEngine.Random.Range(-2, 3).ToString();
                    x = UnityEngine.Random.Range(-2, 3);
                    xVal = x.ToString();
                    realX = (float)x;
                }
                else if (r < 0.66)
                {
                    img += UnityEngine.Random.value < 0.5 ? "sin;" : "cos;";
                    img += UnityEngine.Random.Range(-2, 3).ToString() + "x";
                    x = UnityEngine.Random.Range(-2, 3);
                    if (x != 0) xVal = x.ToString() + "pi";
                    else xVal = x.ToString();
                    realX = x * Mathf.PI;
                }
                else
                {
                    img += "abs;";
                    img += UnityEngine.Random.Range(1, 6).ToString() + "x";
                    x = UnityEngine.Random.Range(-2, 3);
                    xVal = x.ToString();
                    realX = (float)x;
                }
                q.images[i] = img;
                q.questions[i] = q.questions[i].Replace("f(r)", "f(" + xVal.Replace("pi", "[pi]").Replace("1[pi]", "[pi]") + ")");
                binTree.BinaryTree functionTree = imageScript.Function(img.Substring(img.IndexOf(';') + 1));

                int answer = Mathf.RoundToInt(functionTree.Evaluate(realX));
                int pos = UnityEngine.Random.Range(0, 5);
                q.correctAnswers[i] = pos;
                string[] answers = new string[5];
                string newAns;
                for (int j = 0; j < 5; j++)
                {
                    if (j == pos) answers[j] = answer.ToString();
                    else
                    {
                        do
                        {
                            int res;
                            do res = answer + UnityEngine.Random.Range(-9, 10);
                            while (Mathf.Abs(res) > 10 || res == answer);
                            newAns = res.ToString();
                        } while (answers.Contains<string>(newAns));
                        answers[j] = newAns;
                    }
                }
                q.answers[i] = answers;
            }
            else if (ans.Contains("{fun---}"))
            {
                string quest = q.questions[i];
                int xValue = UnityEngine.Random.Range(-9, 10);
                float floatXvalue = UnityEngine.Random.value < 0.5 ? UnityEngine.Random.value * 3 : -UnityEngine.Random.value * 3;
                floatXvalue = Mathf.Round(floatXvalue * 100f) / 100f;
                if (quest.Contains("[mantissa]") || quest.Contains("[floor]"))
                    q.questions[i] = "Data la sguente funzione quanto fa f(" + floatXvalue.ToString() + ")?";
                else
                    q.questions[i] = "Data la sguente funzione quanto fa f(" + xValue.ToString() + ")?";
                Dictionary<string, int> prms = new Dictionary<string, int>();
                if (quest.Contains("[floor]"))
                    q.images[i] = "tf;1;f(x)=ParteIntera(m1x+m2)[val]floor;+;m1x;m2";
                else if (quest.Contains("[mantissa]"))
                    q.images[i] = "tf;1;f(x)=mantissa(m1x+m2)[val]-;+;m1x;m2;floor;+;m1x;m2";
                else if (quest.Contains("[abs]"))
                    q.images[i] = "tf;1;f(x)=|m1x+m2|[val]abs;+;m1x;m2";
                else if (quest.Contains("PartePositiva"))
                    q.images[i] = "tf;1;f(x)=PartePositiva(m1x+m2)[val]max;0;+;m1x;m2";
                else if (quest.Contains("ParteNegativa"))
                    q.images[i] = "tf;1;f(x)=ParteNegativa(m1x+m2)[val]max;0;*;-1;+;m1x;m2";
                string[] values = q.images[i].Split("[val]", StringSplitOptions.RemoveEmptyEntries);
                for (int j = 1; j < 10; j++)
                {
                    string param = "m" + j.ToString();
                    if (q.images[i].Contains(param))
                    {
                        prms.Add(param, UnityEngine.Random.Range(-9, 10));
                        values[0] = values[0].Replace(param, prms[param].ToString());
                        values[1] = values[1].Replace(param, prms[param].ToString());
                    }
                }
                binTree.BinaryTree functionTree = imageScript.Function(values[1]);
                q.images[i] = values[0];
                if (q.images[i].Contains("mantissa"))
                {
                    float answer = Mathf.Round(functionTree.Evaluate(floatXvalue) * 100f) / 100f;
                    int pos = UnityEngine.Random.Range(0, 5);
                    q.correctAnswers[i] = pos;
                    string[] answers = new string[5];
                    string newAns;
                    for (int j = 0; j < 5; j++)
                    {
                        if (j == pos) answers[j] = answer.ToString();
                        else
                        {
                            do
                            {
                                float res;
                                do res = Mathf.Round(UnityEngine.Random.value * 100f) / 100f;
                                while (Mathf.Abs(res - answer) < 0.05);
                                newAns = res.ToString();
                            } while (answers.Contains<string>(newAns));
                            answers[j] = newAns;
                        }
                    }
                    q.answers[i] = answers;
                }
                else if (q.images[i].Contains("ParteIntera"))
                {
                    int answer = Mathf.RoundToInt(functionTree.Evaluate(floatXvalue));
                    int pos = UnityEngine.Random.Range(0, 5);
                    q.correctAnswers[i] = pos;
                    string[] answers = new string[5];
                    string newAns;
                    for (int j = 0; j < 5; j++)
                    {
                        if (j == pos) answers[j] = answer.ToString();
                        else
                        {
                            do
                            {
                                int res;
                                do res = answer + UnityEngine.Random.Range(-9, 10);
                                while (res == answer);
                                newAns = res.ToString();
                            } while (answers.Contains<string>(newAns));
                            answers[j] = newAns;
                        }
                    }
                    q.answers[i] = answers;
                }
                else
                {
                    int answer = Mathf.RoundToInt(functionTree.Evaluate((float)xValue));
                    int pos = UnityEngine.Random.Range(0, 5);
                    q.correctAnswers[i] = pos;
                    string[] answers = new string[5];
                    string newAns;
                    for (int j = 0; j < 5; j++)
                    {
                        if (j == pos) answers[j] = answer.ToString();
                        else
                        {
                            do
                            {
                                int res;
                                do res = answer + UnityEngine.Random.Range(-9, 10);
                                while (res == answer);
                                newAns = res.ToString();
                            } while (answers.Contains<string>(newAns));
                            answers[j] = newAns;
                        }
                    }
                    q.answers[i] = answers;
                }
            }
        }
    }


    public static void LimitsQuestions(quizScript.Quiz q, int i)
    {
        string ans = q.answers[i][0];
        if (ans.Contains("{lim}"))
        {
            Dictionary<string, int> prms = new Dictionary<string, int>();
            for (int j = 1; j < 10; j++)
            {
                string param = "m" + j.ToString();
                if (q.questions[i].Contains(param))
                {
                    prms.Add(param, UnityEngine.Random.Range(-9, 10));
                    q.questions[i] = q.questions[i].Replace(param, prms[param].ToString());
                }
            }
            int answer = prms["m1"] * prms["m2"] + prms["m3"];
            int pos = UnityEngine.Random.Range(0, 5);
            q.correctAnswers[i] = pos;
            string[] answers = new string[5];
            string newAns;
            for (int j = 0; j < 5; j++)
            {
                if (j == pos) answers[j] = answer.ToString();
                else
                {
                    do
                    {
                        int res;
                        do res = answer + UnityEngine.Random.Range(-9, 10);
                        while (res == answer);
                        newAns = res.ToString();
                    } while (answers.Contains<string>(newAns));
                    answers[j] = newAns;
                }
            }
            q.answers[i] = answers;
        }
        else if (q.questions[i].Contains("{lim-}"))
        {
            q.questions[i] = q.questions[i].Substring(0, q.questions[i].IndexOf("{lim-}"));
            for (int j = 1; j < 10; j++)
            {
                string param = "m" + j.ToString();
                if (q.questions[i].Contains(param))
                {
                    string n = UnityEngine.Random.Range(-9, 10).ToString();
                    q.questions[i] = q.questions[i].Replace(param, n);
                    for (int k = 0; k < q.answers[i].Length; k++)
                        q.answers[i][k] = q.answers[i][k].Replace(param, n).Replace("--", "+");
                }
            }
        }
        else if (ans.Contains("{lim--}"))
        {
            Dictionary<string, string> prms = new Dictionary<string, string>();
            for (int j = 1; j < 10; j++)
            {
                string param = "m" + j.ToString();
                if (q.images[i].Contains(param))
                {
                    string n;
                    float r = UnityEngine.Random.value;
                    if (r < 0.33)
                        n = "-inf";
                    else if (r < 0.66)
                        n = "+inf";
                    else
                        n = UnityEngine.Random.Range(-9, 10).ToString();

                    prms.Add(param, n);
                    q.images[i] = q.images[i].Replace(param, n);
                }
            }
            string answer = "lim[x[->]" + prms["m1"].Replace("inf", "[inf]") + "](f(x)+g(x))".Replace("inf", "[inf]");

            string[] answers = new string[5]
            { answer + "=" + prms["m1"],
              answer + "=+[inf]",
              answer + "=-[inf]",
              answer + "...",
              answer + " non si può determinare con queste informazioni" };

            if (prms["m2"] == "+inf")
            {
                if (prms["m3"] == "-inf")
                {
                    answer += " non si può determinare con queste informazioni";
                    q.correctAnswers[i] = 4;
                }
                else
                {
                    answer += "=+[inf]";
                    q.correctAnswers[i] = 1;
                }
            }
            else if (prms["m3"] == "+inf")
            {
                if (prms["m2"] == "-inf")
                {
                    answer += " non si può determinare con queste informazioni";
                    q.correctAnswers[i] = 4;
                }
                else
                {
                    answer += "=+[inf]";
                    q.correctAnswers[i] = 1;
                }
            }
            else if (prms["m2"] == "-inf")
            {
                if (prms["m3"] == "+inf")
                {
                    answer += " non si può determinare con queste informazioni";
                    q.correctAnswers[i] = 4;
                }
                else
                {
                    answer += "=-[inf]";
                    q.correctAnswers[i] = 2;
                }
            }
            else if (prms["m3"] == "-inf")
            {
                if (prms["m2"] == "+inf")
                {
                    answer += " non si può determinare con queste informazioni";
                    q.correctAnswers[i] = 4;
                }
                else
                {
                    answer += "=-[inf]";
                    q.correctAnswers[i] = 2;
                }
            }
            else
            {
                answer += "=" + (int.Parse(prms["m2"]) + int.Parse(prms["m3"])).ToString();
                if (!prms["m1"].Contains("inf"))
                    if (int.Parse(prms["m2"]) + int.Parse(prms["m3"]) == int.Parse(prms["m1"])) q.correctAnswers[i] = 0;
                    else q.correctAnswers[i] = 3;
            }

            if (q.correctAnswers[i] == 3)
                answers[3] = answer;
            else
            {
                string newAns;
                do newAns = (UnityEngine.Random.Range(-9, 10)).ToString();
                while (answers.Contains<string>(newAns));
                answers[3] = "lim[x[->]" + prms["m1"].Replace("inf", "[inf]") + "](f(x)+g(x))=" + newAns;
            }

            if (answers[0].Contains("[inf]"))
            {
                string newAns;
                do newAns = (UnityEngine.Random.Range(-9, 10)).ToString();
                while (answers.Contains<string>(newAns));
                answers[0] = "lim[x[->]" + prms["m1"].Replace("inf", "[inf]") + "](f(x)+g(x))=" + newAns;
            }

            q.answers[i] = answers;
        }
        else if (ans.Contains("{lim---}"))
        {
            string[] values = q.images[i].Split("[val]", StringSplitOptions.RemoveEmptyEntries);
            for (int j = 1; j < 10; j++)
            {
                string param = "m" + j.ToString();
                if (q.images[i].Contains(param))
                {
                    int newVal;
                    do newVal = UnityEngine.Random.Range(-10, 9);
                    while (newVal == 0);

                    values[0] = values[0].Replace(param, newVal.ToString());
                    values[1] = values[1].Replace(param, newVal.ToString());
                }
            }
            binTree.BinaryTree functionTree = imageScript.Function(values[1]);
            q.images[i] = values[0];
            int answer = Mathf.RoundToInt(functionTree.Evaluate(0));
            int pos = UnityEngine.Random.Range(0, 5);
            q.correctAnswers[i] = pos;
            string[] answers = new string[5];
            string newAns;
            for (int j = 0; j < 5; j++)
            {
                if (j == pos) answers[j] = answer.ToString();
                else
                {
                    do
                    {
                        int res;
                        do res = answer + UnityEngine.Random.Range(-9, 10);
                        while (res == answer);
                        newAns = res.ToString();
                    } while (answers.Contains<string>(newAns));
                    answers[j] = newAns;
                }
                q.answers[i] = answers;
            }
        }
        else if (ans.Contains("{lim----}"))
        {
            Dictionary<string, string> prms = new Dictionary<string, string>();
            for (int j = 1; j < 10; j++)
            {
                string param = "m" + j.ToString();
                if (q.images[i].Contains(param))
                {
                    string n;
                    float r = UnityEngine.Random.value;
                    if (r < 0.25)
                        n = "-inf";
                    else if (r < 0.5)
                        n = "+inf";
                    else if (r < 0.75)
                        n = UnityEngine.Random.Range(-9, 10).ToString();
                    else
                        n = "0";

                    prms.Add(param, n);
                    q.images[i] = q.images[i].Replace(param, n);
                }
            }
            string answer = "lim[x[->]" + prms["m1"].Replace("inf", "[inf]") + "](f(x)[*]g(x))";

            string[] answers = new string[5]
            { answer + "=0",
              answer + "=+[inf]",
              answer + "=-[inf]",
              answer + "...",
              answer + " non si può determinare con queste informazioni" };

            if (prms["m2"] == "+inf")
            {
                if (prms["m3"] == "0")
                {
                    answer += " non si può determinare con queste informazioni";
                    q.correctAnswers[i] = 4;
                }
                else
                {
                    answer += "=";
                    answer += prms["m3"].Contains("-") ? "-[inf]" : "+[inf]";
                    q.correctAnswers[i] = prms["m3"].Contains("-") ? 2 : 1;
                }
            }
            else if (prms["m3"] == "+inf")
            {
                if (prms["m2"] == "0")
                {
                    answer += " non si può determinare con queste informazioni";
                    q.correctAnswers[i] = 4;
                }
                else
                {
                    answer += "=";
                    answer += prms["m2"].Contains("-") ? "-[inf]" : "+[inf]";
                    q.correctAnswers[i] = prms["m2"].Contains("-") ? 2 : 1;
                }
            }
            else if (prms["m2"] == "-inf")
            {
                if (prms["m3"] == "0")
                {
                    answer += " non si può determinare con queste informazioni";
                    q.correctAnswers[i] = 4;
                }
                else
                {
                    answer += "=";
                    answer += prms["m3"].Contains("-") ? "+[inf]" : "-[inf]";
                    q.correctAnswers[i] = prms["m3"].Contains("-") ? 1 : 2;
                }
            }
            else if (prms["m3"] == "-inf")
            {
                if (prms["m2"] == "0")
                {
                    answer += " non si può determinare con queste informazioni";
                    q.correctAnswers[i] = 4;
                }
                else
                {
                    answer += "=";
                    answer += prms["m2"].Contains("-") ? "+[inf]" : "-[inf]";
                    q.correctAnswers[i] = prms["m2"].Contains("-") ? 1 : 2;
                }
            }
            else
            {
                answer += "=" + (int.Parse(prms["m2"]) * int.Parse(prms["m3"])).ToString();
                if (int.Parse(prms["m2"]) * int.Parse(prms["m3"]) == 0) q.correctAnswers[i] = 0;
                else q.correctAnswers[i] = 3;
            }


            if (q.correctAnswers[i] == 3)
                answers[3] = answer;
            else
            {
                string newAns;
                do newAns = (UnityEngine.Random.Range(-9, 10)).ToString();
                while (answers.Contains<string>(newAns));
                answers[3] = "lim[x[->]" + prms["m1"].Replace("inf", "[inf]") + "](f(x)[*]g(x))=" + newAns;
            }

            q.answers[i] = answers;
        }
        else if (ans.Contains("{lim-----}"))
        {
            Dictionary<string, string> prms = new Dictionary<string, string>();
            for (int j = 1; j < 10; j++)
            {
                string param = "m" + j.ToString();
                if (q.images[i].Contains(param))
                {
                    string n;
                    float r = UnityEngine.Random.value;
                    if (r < 0.2)
                        n = "-inf";
                    else if (r < 0.4)
                        n = "+inf";
                    else if (r < 0.6)
                        n = UnityEngine.Random.Range(-9, 10).ToString();
                    else if (r < 0.8)
                        n = "0";
                    else
                        n = "1";

                    prms.Add(param, n);
                    q.images[i] = q.images[i].Replace(param, n);
                }
            }
            string answer = "lim[x[->]" + prms["m1"].Replace("inf", "[inf]") + "]f(x)<sup>g(x)</sup>";

            string[] answers = new string[5]
            { answer + "=0",
              answer + "=+[inf]",
              answer + "=-[inf]",
              answer + "...",
              answer + " non si può determinare con queste informazioni" };

            if (prms["m2"] == "0")
            {
                if (prms["m3"] == "0")
                {
                    answer += " non si può determinare con queste informazioni";
                    q.correctAnswers[i] = 4;
                }
                else
                    answer += "=0";
            }
            else if (prms["m2"] == "1")
            {
                if (prms["m2"].Contains("inf"))
                {
                    answer += " non si può determinare con queste informazioni";
                    q.correctAnswers[i] = 4;
                }
                else
                    answer += "=1";
            }
            else if (prms["m2"] == "+inf")
            {
                if (prms["m3"] == "0")
                {
                    answer += " non si può determinare con queste informazioni";
                    q.correctAnswers[i] = 4;
                }
                else
                {
                    answer += "=";
                    answer += prms["m3"].Contains("-") ? "0" : "+[inf]";
                    q.correctAnswers[i] = prms["m3"].Contains("-") ? 0 : 1;
                }
            }
            else if (prms["m2"] == "-inf")
            {
                if (prms["m3"] == "0")
                {
                    answer += " non si può determinare con queste informazioni";
                    q.correctAnswers[i] = 4;
                }
                else
                {
                    answer += "=";
                    answer += prms["m3"].Contains("-") ? "0" : "-[inf]";
                    q.correctAnswers[i] = prms["m3"].Contains("-") ? 0 : 2;
                }
            }
            else
            {
                answer += "=" + (Mathf.Pow(int.Parse(prms["m2"]), int.Parse(prms["m3"]))).ToString();
                if (Mathf.Pow(int.Parse(prms["m2"]), int.Parse(prms["m3"])) == 0) q.correctAnswers[i] = 0;
                else q.correctAnswers[i] = 3;
            }


            if (q.correctAnswers[i] == 3)
                answers[3] = answer;
            else
            {
                string newAns;
                do newAns = (UnityEngine.Random.Range(-9, 10)).ToString();
                while (answers.Contains<string>(newAns));
                answers[3] = "lim[x[->]" + prms["m1"].Replace("inf", "[inf]") + "]f(x)<sup>g(x)</sup>=" + newAns;
            }

            q.answers[i] = answers;
        }
    }

    public static void DerivativesQuestions(quizScript.Quiz q, int i)
    {
        if (q.answers[i].Length == 1)
        {
            string ans = q.answers[i][0];
            if (ans.Contains("{der}"))
            {
                int degree = UnityEngine.Random.Range(1, 4);
                int[] coefficients = new int[degree + 1];
                for (int j = 0; j <= degree; j++)
                    coefficients[j] = UnityEngine.Random.Range(-9, 10);
                string imgTxt = "tf;1;f(x)=";
                List<string> monomials = new List<string>();
                List<string> terms = new List<string>();
                for (int j = 0; j <= degree; j++)
                {
                    if (coefficients[j] == 0) continue;
                    if (j == 0) 
                    {
                        monomials.Add(coefficients[j].ToString());
                        continue;
                    }
                    if (j == 1) monomials.Add(coefficients[j].ToString() + "x");
                    else monomials.Add(coefficients[j].ToString() + "x<sup>" + j.ToString() + "</sup>");
                    terms.Add("*;" + (coefficients[j] * j).ToString() + ";^;x;" + (j-1).ToString());
                }
                string polinomial;
                string functionString;
                if (monomials.Count == 0)
                    polinomial = "0";
                else
                    polinomial = string.Join('+', monomials);
                if (terms.Count == 0)
                    functionString = "0";
                else
                {
                    functionString = string.Join(";+;", terms);
                    functionString = "+;" + functionString;
                    functionString = functionString.Remove(functionString.LastIndexOf("+;"), 2);
                }
                q.images[i] = imgTxt + polinomial;

                binTree.BinaryTree functionTree = imageScript.Function(functionString);

                int r = UnityEngine.Random.Range(-9, 10);
                q.questions[i] = q.questions[i].Replace("f'(r)", "f'(" + r.ToString() + ")");

                int answer = Mathf.RoundToInt(functionTree.Evaluate((float)r));
                int pos = UnityEngine.Random.Range(0, 5);
                q.correctAnswers[i] = pos;
                string[] answers = new string[5];
                string newAns;
                for (int j = 0; j < 5; j++)
                {
                    if (j == pos) answers[j] = answer.ToString();
                    else
                    {
                        do
                        {
                            int res;
                            do res = answer + UnityEngine.Random.Range(-9, 10);
                            while (res == answer);
                            newAns = res.ToString();
                        } while (answers.Contains<string>(newAns));
                        answers[j] = newAns;
                    }
                }
                q.answers[i] = answers;
            }
            else if (ans.Contains("{der-}"))
            {
                int degree = UnityEngine.Random.Range(2, 5);
                int numOfDerivatives = UnityEngine.Random.Range(1, 4);
                int[] coefficients = new int[degree + 1];
                for (int j = 0; j <= degree; j++)
                    coefficients[j] = UnityEngine.Random.Range(-5, 6);
                string imgTxt = "tf;1;f(x)=";
                List<string> monomials = new List<string>();
                List<string> terms = new List<string>();
                for (int j = 0; j <= degree; j++)
                {
                    if (coefficients[j] == 0) continue;
                    if (j == 0)
                    {
                        monomials.Add(coefficients[j].ToString());
                        continue;
                    }
                    if (j == 1) monomials.Add(coefficients[j].ToString() + "x");
                    else monomials.Add(coefficients[j].ToString() + "x<sup>" + j.ToString() + "</sup>");
                }
                for (int j=numOfDerivatives; j<=degree; j++)
                    terms.Add("*;" + ComputeDerivativeCoefficient(coefficients[j], numOfDerivatives, j).ToString() + ";^;x;" + (j - numOfDerivatives).ToString());
                string polinomial;
                string functionString;
                if (monomials.Count == 0)
                    polinomial = "0";
                else
                    polinomial = string.Join('+', monomials);
                if (terms.Count == 0)
                    functionString = "0";
                else
                {
                    functionString = string.Join(";+;", terms);
                    functionString = "+;" + functionString;
                    functionString = functionString.Remove(functionString.LastIndexOf("+;"), 2);
                }
                q.images[i] = imgTxt + polinomial;

                binTree.BinaryTree functionTree = imageScript.Function(functionString);

                int r = UnityEngine.Random.Range(-5, 6);
                q.questions[i] = q.questions[i].Replace("f<sup>(m1)</sup>(m2)", "f<sup>(" + numOfDerivatives.ToString() + ")</sup>(" + r.ToString() + ")");

                int answer = Mathf.RoundToInt(functionTree.Evaluate((float)r));
                int pos = UnityEngine.Random.Range(0, 5);
                q.correctAnswers[i] = pos;
                string[] answers = new string[5];
                string newAns;
                for (int j = 0; j < 5; j++)
                {
                    if (j == pos) answers[j] = answer.ToString();
                    else
                    {
                        do
                        {
                            int res;
                            do res = answer + UnityEngine.Random.Range(-9, 10);
                            while (res == answer);
                            newAns = res.ToString();
                        } while (answers.Contains<string>(newAns));
                        answers[j] = newAns;
                    }
                }
                q.answers[i] = answers;
            }
        }
    }

    public static void AntiderivativesQuestions(quizScript.Quiz q, int i)
    {
        if (q.answers[i].Length == 1)
        {
            string ans = q.answers[i][0];
            if (ans.Contains("{int}"))
            {
                int degree = UnityEngine.Random.Range(0, 3);
                int[] coefficients = new int[degree + 1];
                for (int j = 0; j <= degree; j++)
                    coefficients[j] = UnityEngine.Random.Range(-5, 6);
                string imgTxt = "tf;1;f(x)=";
                List<string> monomials = new List<string>();
                List<string> terms = new List<string>();
                for (int j = 0; j <= degree; j++)
                {
                    if (coefficients[j] == 0) continue;
                    if (j == 0) monomials.Add(coefficients[j].ToString());
                    else if (j == 1) monomials.Add(coefficients[j].ToString() + "x");
                    else monomials.Add(coefficients[j].ToString() + "x<sup>" + j.ToString() + "</sup>");
                    terms.Add("*;" + ((float)coefficients[j] / (float)(j + 1)).ToString() + ";^;x;" + (j + 1).ToString());
                }
                string polinomial;
                string functionString;
                if (monomials.Count == 0)
                    polinomial = "0";
                else
                    polinomial = string.Join('+', monomials);
                if (terms.Count == 0)
                    functionString = "0";
                else
                {
                    functionString = string.Join(";+;", terms);
                    functionString = "+;" + functionString;
                    functionString = functionString.Remove(functionString.LastIndexOf("+;"), 2);
                }
                q.images[i] = imgTxt + polinomial;

                binTree.BinaryTree functionTree = imageScript.Function(functionString);

                int m2 = UnityEngine.Random.Range(-9, 10);
                int m1 = m2 + UnityEngine.Random.Range(1, 6);
                q.questions[i] = q.questions[i].Replace("F(m1)", "F(" + m1.ToString() + ")");
                q.questions[i] = q.questions[i].Replace("F(m2)", "F(" + m2.ToString() + ")");


                float answer = Mathf.Round((functionTree.Evaluate((float)m1) - functionTree.Evaluate((float)m2)) * 100f) / 100f;
                int pos = UnityEngine.Random.Range(0, 5);
                q.correctAnswers[i] = pos;
                string[] answers = new string[5];
                string newAns;
                for (int j = 0; j < 5; j++)
                {
                    if (j == pos) answers[j] = answer.ToString();
                    else
                    {
                        do
                        {
                            float res;
                            do res = Mathf.Round((answer + (UnityEngine.Random.value - 0.5f) * 10f) * 100f) / 100f;
                            while (res == answer);
                            newAns = res.ToString();
                        } while (answers.Contains<string>(newAns));
                        answers[j] = newAns;
                    }
                }
                q.answers[i] = answers;
            }
            else if (ans.Contains("{int-}"))
            {
                int degree = UnityEngine.Random.Range(0, 3);
                int[] coefficients = new int[degree + 1];
                for (int j = 0; j <= degree; j++)
                    coefficients[j] = UnityEngine.Random.Range(-5, 6);
                string imgTxt = "tf;1;f(x)=int[";
                List<string> monomials = new List<string>();
                List<string> terms = new List<string>();
                for (int j = 0; j <= degree; j++)
                {
                    if (coefficients[j] == 0) continue;
                    if (j == 0) monomials.Add(coefficients[j].ToString());
                    else if (j == 1) monomials.Add(coefficients[j].ToString() + "x");
                    else monomials.Add(coefficients[j].ToString() + "x<sup>" + j.ToString() + "</sup>");
                    terms.Add("*;" + ((float)coefficients[j] / (float)(j + 1)).ToString() + ";^;x;" + (j + 1).ToString());
                }
                string polinomial;
                string functionString;
                if (monomials.Count == 0)
                    polinomial = "0";
                else
                    polinomial = string.Join('+', monomials);
                if (terms.Count == 0)
                    functionString = "0";
                else
                {
                    functionString = string.Join(";+;", terms);
                    functionString = "+;" + functionString;
                    functionString = functionString.Remove(functionString.LastIndexOf("+;"), 2);
                }

                binTree.BinaryTree functionTree = imageScript.Function(functionString);

                int m2 = UnityEngine.Random.Range(-9, 10);
                int m1 = m2 + UnityEngine.Random.Range(1, 6);
                q.questions[i] = q.questions[i].Replace("F(m1)", "F(" + m1.ToString() + ")");
                q.questions[i] = q.questions[i].Replace("F(m2)", "F(" + m2.ToString() + ")");

                q.images[i] = imgTxt + m2.ToString() + ", " + m1.ToString() + "](" + polinomial + " dx)";

                float answer = Mathf.Round((functionTree.Evaluate((float)m1) - functionTree.Evaluate((float)m2)) * 100f) / 100f;
                int pos = UnityEngine.Random.Range(0, 5);
                q.correctAnswers[i] = pos;
                string[] answers = new string[5];
                string newAns;
                for (int j = 0; j < 5; j++)
                {
                    if (j == pos) answers[j] = answer.ToString();
                    else
                    {
                        do
                        {
                            float res;
                            do res = Mathf.Round((answer + (UnityEngine.Random.value - 0.5f) * 10f) * 100f) / 100f;
                            while (res == answer);
                            newAns = res.ToString();
                        } while (answers.Contains<string>(newAns));
                        answers[j] = newAns;
                    }
                }
                q.answers[i] = answers;
            }
            else if (ans.Contains("{int--}"))
            {
                int degree = UnityEngine.Random.Range(0, 3);
                int[] coefficients = new int[degree + 1];
                for (int j = 0; j <= degree; j++)
                    coefficients[j] = UnityEngine.Random.Range(-5, 6);
                string imgTxt = "tf;1;f(x)=";
                List<string> monomials = new List<string>();
                List<string> terms = new List<string>();
                for (int j = 0; j <= degree; j++)
                {
                    if (coefficients[j] == 0) continue;
                    if (j == 0) monomials.Add(coefficients[j].ToString());
                    else if (j == 1) monomials.Add(coefficients[j].ToString() + "x");
                    else monomials.Add(coefficients[j].ToString() + "x<sup>" + j.ToString() + "</sup>");
                    terms.Add("*;" + ((float)coefficients[j] / (float)(j + 1)).ToString() + ";^;x;" + (j + 1).ToString());
                }
                string polinomial;
                string functionString;
                if (monomials.Count == 0)
                    polinomial = "0";
                else
                    polinomial = string.Join('+', monomials);
                if (terms.Count == 0)
                    functionString = "0";
                else
                {
                    functionString = string.Join(";+;", terms);
                    functionString = "+;" + functionString;
                    functionString = functionString.Remove(functionString.LastIndexOf("+;"), 2);
                }

                binTree.BinaryTree functionTree = imageScript.Function(functionString);

                int m2 = UnityEngine.Random.Range(-9, 10);
                int m1 = m2 + UnityEngine.Random.Range(1, 6);
                q.questions[i] = q.questions[i].Replace("[m1]", "[" + m2.ToString() + ", " + m1.ToString() + "]");

                q.images[i] = imgTxt + polinomial;

                float answer = Mathf.Round(((functionTree.Evaluate((float)m1) - functionTree.Evaluate((float)m2)) / (m1 - m2)) * 100f) / 100f;
                int pos = UnityEngine.Random.Range(0, 5);
                q.correctAnswers[i] = pos;
                string[] answers = new string[5];
                string newAns;
                for (int j = 0; j < 5; j++)
                {
                    if (j == pos) answers[j] = answer.ToString();
                    else
                    {
                        do
                        {
                            float res;
                            do res = Mathf.Round((answer + (UnityEngine.Random.value - 0.5f) * 10f) * 100f) / 100f;
                            while (res == answer);
                            newAns = res.ToString();
                        } while (answers.Contains<string>(newAns));
                        answers[j] = newAns;
                    }
                }
                q.answers[i] = answers;
            }
        }
        else if (q.images.ContainsKey(i) && q.images[i].Contains("{int---}"))
        {
            string num, den;
            int answer;
            if (UnityEngine.Random.value < 0.5f)
            {
                float res = UnityEngine.Random.value < 0.5f ? float.PositiveInfinity : 1f;
                res *= UnityEngine.Random.value < 0.5f ? 1f : -1f;
                int minDen;
                int minNum;
                if (float.IsFinite(res))
                {
                    answer = res < 0 ? 3 : 2;
                    minDen = UnityEngine.Random.Range(1, 13);
                    minNum = UnityEngine.Random.Range(Mathf.Max(0, minDen - 3), minDen);
                }
                else
                {
                    answer = res < 0 ? 1 : 0;
                    minDen = UnityEngine.Random.Range(4, 13);
                    minNum = UnityEngine.Random.Range(0, minDen - 3);
                }
                den = GeneratePolinomial(minDen, 12, true);
                num = GeneratePolinomial(minNum, 12, true);
                q.images[i] = "tf;1;int[0, 0.01](";
                if (res < 0)
                {
                    if (UnityEngine.Random.value < 0.5f) q.images[i] += "(-" + num + ")/(" + den + ")";
                    else q.images[i] += "(" + num + ")/(-" + den + ")";
                }
                else
                {
                    if (UnityEngine.Random.value < 0.5f) q.images[i] += "(" + num + ")/(" + den + ")";
                    else q.images[i] += "(-" + num + ")/(-" + den + ")";
                }
            }
            else
            {
                float res = UnityEngine.Random.value < 0.5f ? float.PositiveInfinity : 1f;
                res *= UnityEngine.Random.value < 0.5f ? 1f : -1f;
                int maxDen;
                int maxNum;
                if (float.IsFinite(res))
                {
                    answer = res < 0 ? 3 : 2;
                    maxDen = UnityEngine.Random.Range(5, 13);
                    maxNum = UnityEngine.Random.Range(0, Mathf.Max(1, maxDen - 4));
                }
                else
                {
                    answer = res < 0 ? 1 : 0;
                    maxDen = UnityEngine.Random.Range(1, 13);
                    maxNum = UnityEngine.Random.Range(Mathf.Max(maxDen - 4, 0), maxDen);
                }
                den = GeneratePolinomial(0, maxDen, false);
                num = GeneratePolinomial(0, maxNum, false);
                q.images[i] = "tf;1;int[1000, +inf](";
                if (res < 0)
                {
                    if (UnityEngine.Random.value < 0.5f) q.images[i] += "(-" + num + ")/(" + den + ")dx)";
                    else q.images[i] += "(" + num + ")/(-" + den + ")dx)";
                }
                else
                {
                    if (UnityEngine.Random.value < 0.5f) q.images[i] += "(" + num + ")/(" + den + ")dx)";
                    else q.images[i] += "(-" + num + ")/(-" + den + ")dx)";
                }
            }
            q.correctAnswers[i] = answer;
        }
    }
}