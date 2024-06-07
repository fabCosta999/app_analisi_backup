using System.Collections.Generic;

public static class keysGeneral
{
    public enum lightMode
    {
        light, dark
    }

    public enum topics 
    { 
        logic, functions, limits, derivatives, antiderivatives, differentials
    }

    public enum quizMode
    {
        fxd, standard, previous, final
    }


    public const string LIGHTMODE = "lightMode";
    public const string TOPIC = "topic";
    public const string LESSON = "lesson";
    public const string TEXT = "[Testo]";
    public const string POPUP = "[Popup]";
    public const string IMAGE = "[Immagine]";
   

    public static Dictionary<string, quizMode> QUIZMODE = new Dictionary<string, quizMode>()
    { 
        { "answerFixed", quizMode.fxd }, 
        { "standard", quizMode.standard }, 
        { "previous", quizMode.previous }
    };

    public static Dictionary<string, binTree.operators> operatporsStrings = new Dictionary<string, binTree.operators>()
    {
        { "-", binTree.operators.Subtract },
        { "*", binTree.operators.Multiply },
        { "/", binTree.operators.Divide },
        { "^", binTree.operators.Pow },
        { "+", binTree.operators.Sum },
        { "log", binTree.operators.Log },
        { "sin", binTree.operators.Sin },
        { "cos", binTree.operators.Cos },
        { "tan", binTree.operators.Tan },
        { "sign", binTree.operators.Sign },
        { "floor", binTree.operators.Floor },
        { "ceil", binTree.operators.Ceil },
        { "max", binTree.operators.Max },
        { "min", binTree.operators.Min },
        { "abs", binTree.operators.Abs },
        { "asin", binTree.operators.Asin },
        { "acos", binTree.operators.Acos },
        { "atan", binTree.operators.Atan }
    };

    public static List<binTree.operators> oneChildOperators = new List<binTree.operators>()
    { binTree.operators.Sin, binTree.operators.Cos, binTree.operators.Tan, binTree.operators.Asin, binTree.operators.Acos, binTree.operators.Atan, binTree.operators.Abs, binTree.operators.Floor, binTree.operators.Ceil, binTree.operators.Sign };

    public static string[] terms = new string[] { "1", "x<sup>1/4</sup>", "x<sup>1/3</sup>", "x<sup>1/2</sup>", "x", "x<sup>5/4</sup>", "x<sup>4/3</sup>", "x<sup>3/2</sup>", "x<sup>2</sup>", "x<sup>9/4</sup>", "x<sup>7/3</sup>", "x<sup>5/2</sup>", "x<sup>3</sup>" };

    public static string ReplaceMath(string res)
    {
        res = res.Replace("[im]", "\u21d2");
        res = res.Replace("[eq]", "\u21d4");
        res = res.Replace("[e]", "\u2227");
        res = res.Replace("[o]", "\u2228");
        res = res.Replace("[non]", "\u00ac");
        res = res.Replace("[inf]", "\u221e");
        res = res.Replace("[->]", "\u2192");
        res = res.Replace("[int]", "\u222b");
        res = res.Replace("[*]", "\u22c5");
        res = res.Replace("[in]", "\u2208");
        res = res.Replace("[nonin]", "\u2209");
        res = res.Replace("[incluso]", "\u2286");
        res = res.Replace("[qualsiasi]", "\u2200");
        res = res.Replace("[esiste]", "\u2203");
        res = res.Replace("[N]", "\u2115");
        res = res.Replace("[Z]", "\u2124");
        res = res.Replace("[Q]", "\u211a");
        res = res.Replace("[R]", "\u211d");
        res = res.Replace("[vuoto]", "\u2205");
        res = res.Replace("[intersecato]", "\u2229");
        res = res.Replace("[unito]", "\u222A");
        res = res.Replace("[pi]", "\u03c0");
        res = res.Replace("[Delta]", "\u0394");
        res = res.Replace("[delta]", "\u03b4");
        res = res.Replace("[epsilon]", "\u03b5");
        res = res.Replace("[>=]", "\u2265");
        res = res.Replace("[<=]", "\u2264");
        res = res.Replace("[!=]", "\u2260");
        res = res.Replace("[+-]", "\u00b1");
        res = res.Replace("[times]", "\u00d7");
        res = res.Replace("[\\n]", "\n");
        return res;
    }

    public static string ReplaceMathInImage(string res)
    {
        res = res.Replace(" im ", " \u21d2 ");
        res = res.Replace(" eq ", " \u21d4 ");
        res = res.Replace(" e ", " \u2227 ");
        res = res.Replace(" o ", " \u2228 ");
        res = res.Replace("non ", "\u00ac");
        res = res.Replace("inf", "\u221e");
        res = res.Replace("->", "\u2192");
        res = res.Replace("int", "\u222b");
        res = res.Replace("*", "\u22c5");
        res = res.Replace("pi", "\u03c0");
        res = res.Replace("Delta", "\u0394");
        res = res.Replace("delta", "\u03b4");
        res = res.Replace("epsilon", "\u03b5");
        res = res.Replace("times", "\u00d7");
        res = res.Replace(">=", "\u2265");
        res = res.Replace("<=", "\u2264");
        res = res.Replace("!=", "\u2260");

        return res;
    }
}
