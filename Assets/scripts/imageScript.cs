using System.Collections.Generic;
using UnityEngine;

public static class imageScript
{
    public enum image_type
    {
        table, image, formula, graph, graphOnly, animation, truthTable, graphTangent, composition
    }

    public enum truthTableMode
    {
        standard, incorrect, missing, whitoutTitle
    }

    public const string TABLE = "t";
    public const string IMAGE = "i";
    public const string FORMULA = "tf";
    public const string GRAPHONLY = "g";
    public const string GRAPH = "ga";
    public const string ANIMATION = "a";
    public const string TRUTHTABLE = "v";
    public const string TANGENT = "gt";
    public const string COMPOSITION = "gaf_g";
    public const string TRUTHTABLE_TRUE = "<color=green>V</color>";
    public const string TRUTHTABLE_FALSE = "<color=red>F</color>";
    public const string TRUTHTABLE_INCORRECT = "<incorrect>";
    public const string TRUTHTABLE_MISSING = "<missing>";
    public const string TRUTHTABLE_WITHOUT = "<whituot>";

    public static Dictionary<string, image_type> IMAGE_TYPE = new Dictionary<string, image_type>()
    {
        { TABLE, image_type.table },
        { IMAGE, image_type.image },
        { FORMULA, image_type.formula },
        { GRAPHONLY, image_type.graphOnly },
        { GRAPH, image_type.graph },
        { ANIMATION, image_type.animation },
        { TRUTHTABLE, image_type.truthTable },
        { TANGENT, image_type.graphTangent },
        { COMPOSITION, image_type.composition }
    };

    public static string[,] ReadTable(string txt)
    {
        string[] strings = txt.Split('#', System.StringSplitOptions.RemoveEmptyEntries);
        string[] nums = strings[0].Split(';', System.StringSplitOptions.RemoveEmptyEntries);
        int rows = int.Parse(nums[0]);
        int columns = int.Parse(nums[1]);
        string[,] tab = new string[rows, columns];
        int k = 1;
        for (int i = 0; i < rows; i++)
            for (int j = 0; j < columns; j++)
                tab[i, j] = strings[k++];
        return tab;
    }

    public static string[,] TruthTable(string txt, truthTableMode mode = truthTableMode.standard)
    {
        string TRUE = TRUTHTABLE_TRUE;
        string FALSE = TRUTHTABLE_FALSE;
        int incorrectValue = -1;
        if (mode == truthTableMode.incorrect)
        {
            string[] inc = txt.Split(TRUTHTABLE_INCORRECT, System.StringSplitOptions.RemoveEmptyEntries);
            txt = inc[0];
            incorrectValue = int.Parse(inc[1].Trim()) + 1;
        }
        if (mode == truthTableMode.missing)
        {
            string[] inc = txt.Split(TRUTHTABLE_MISSING, System.StringSplitOptions.RemoveEmptyEntries);
            txt = inc[0];
            incorrectValue = int.Parse(inc[1].Trim()) + 1;
        }

        if (mode == truthTableMode.whitoutTitle)
        {
            txt = txt.Substring(0, txt.IndexOf(TRUTHTABLE_WITHOUT));
        }

        string[] strings = txt.Split(';', System.StringSplitOptions.RemoveEmptyEntries);
        int n = int.Parse(strings[0]);
        int n_var = 0;
        for (int i = 0; i < n; i++) strings[i] = strings[i + 1];
        Dictionary<string, int> keys = new Dictionary<string, int>();
        Dictionary<int, string> stringMeanings = new Dictionary<int, string>();
        Dictionary<int, int> ids = new Dictionary<int, int>();
        for (int i = 0; i < n; i++)
        {
            strings[i] = strings[i].Trim();
            if (strings[i].Contains('='))
            {
                string[] res = strings[i].Split('=', System.StringSplitOptions.RemoveEmptyEntries);
                keys.Add(res[0], i);
                stringMeanings.Add(i, res[1]);
            }
            if (strings[i].Length == 1)
            {
                ids.Add(i, n_var++);
                keys.Add(strings[i], i);
            }
        }
        int length = (int)Mathf.Pow(2, n_var) + 1;
        string[,] tab = new string[length, n];
        for (int i = 0; i < n; i++)
        {
            tab[0, i] = strings[i];
            if (ids.ContainsKey(i))
            {
                int col = 1;
                for (int k = 0; k < Mathf.Pow(2, ids[i]); k++)
                {
                    for (int j = 0; j < (length - 1) / (2 * Mathf.Pow(2, ids[i])); j++)
                        tab[col++, i] = TRUE;
                    for (int j = 0; j < (length - 1) / (2 * Mathf.Pow(2, ids[i])); j++)
                        tab[col++, i] = FALSE;
                }
            }
            else if (stringMeanings.ContainsKey(i))
                strings[i] = stringMeanings[i];

            if (strings[i].Contains("non"))
            {
                string op = strings[i].Substring(strings[i].IndexOf('n') + 4, 1);
                int n1 = keys[op];
                for (int j = 1; j < length; j++)
                    tab[j, i] = tab[j, n1] == TRUE ? FALSE : TRUE;
            }

            else if (strings[i].Contains(" e "))
            {
                string[] op = strings[i].Split('e', System.StringSplitOptions.RemoveEmptyEntries);
                int n1 = keys[op[0].Trim()];
                int n2 = keys[op[1].Trim()];
                for (int j = 1; j < length; j++)
                    tab[j, i] = tab[j, n1] == TRUE && tab[j, n2] == TRUE ? TRUE : FALSE;
            }

            else if (strings[i].Contains(" o "))
            {
                string[] op = strings[i].Split('o', System.StringSplitOptions.RemoveEmptyEntries);
                int n1 = keys[op[0].Trim()];
                int n2 = keys[op[1].Trim()];
                for (int j = 1; j < length; j++)
                    tab[j, i] = tab[j, n1] == TRUE || tab[j, n2] == TRUE ? TRUE : FALSE;
            }

            else if (strings[i].Contains("im"))
            {
                string[] op = strings[i].Replace("im", "|").Split('|', System.StringSplitOptions.RemoveEmptyEntries);
                int n1 = keys[op[0].Trim()];
                int n2 = keys[op[1].Trim()];
                for (int j = 1; j < length; j++)
                    tab[j, i] = tab[j, n1] == TRUE && tab[j, n2] == FALSE ? FALSE : TRUE;
            }

            else if (strings[i].Contains("eq"))
            {
                string[] op = strings[i].Replace("eq", "|").Split('|', System.StringSplitOptions.RemoveEmptyEntries);
                int n1 = keys[op[0].Trim()];
                int n2 = keys[op[1].Trim()];
                for (int j = 1; j < length; j++)
                    tab[j, i] = tab[j, n1] == tab[j, n2] ? TRUE : FALSE;
            }
        }

        if (mode == truthTableMode.incorrect)
            tab[incorrectValue, tab.GetLength(1) - 1] = tab[incorrectValue, tab.GetLength(1) - 1] == TRUE ? FALSE : TRUE;

        if (mode == truthTableMode.missing)
            tab[incorrectValue, tab.GetLength(1) - 1] = "";

        if (mode == truthTableMode.whitoutTitle)
            tab[0, tab.GetLength(1) - 1] = "";

        return tab;
    }

    public static string WriteTab(string[,] tab)
    {
        string TRUE = TRUTHTABLE_TRUE;
        string FALSE = TRUTHTABLE_FALSE;
        string txt = "";
        int rows = tab.GetLength(0);
        int cols = tab.GetLength(1);
        int[] columnWidths = new int[cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                int length = tab[i, j].Length;
                if (tab[i, j] == TRUE || tab[i, j] == FALSE) length = 1;
                if (length > columnWidths[j])
                    columnWidths[j] = length;
            }
        }

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (tab[i, j] == TRUE | tab[i, j] == FALSE)
                    txt += tab[i, j] + new string(' ', (columnWidths[j] + 1) * 2);
                else
                    txt += tab[i, j] + new string(' ', (columnWidths[j] + 2 - tab[i, j].Length) * 2);
            }
            txt += '\n';
        }
        return keysGeneral.ReplaceMathInImage(txt).Trim();
    }


    public static binTree.BinaryTree Function(string img)
    {
        binTree.BinaryTree tree = new binTree.BinaryTree(new binTree.BinaryTreeNode(binTree.types.Inactive, 0f));
        FillTree(tree, tree.root, img);
        return tree;
    }

    private static string FillTree(binTree.BinaryTree tree, binTree.BinaryTreeNode actualNode, string img)
    {
        string op;
        if (img.Contains(';')) op = img.Substring(0, img.IndexOf(';'));
        else op = img;
        if (keysGeneral.operatporsStrings.ContainsKey(op))
        {
            tree.Insert(new binTree.BinaryTreeNode(binTree.types.Operator, (float)keysGeneral.operatporsStrings[op]), actualNode);
            if (keysGeneral.oneChildOperators.Contains(keysGeneral.operatporsStrings[op]))
                actualNode.right = new binTree.BinaryTreeNode(binTree.types.Inactive, 0f);
            else
            {
                actualNode.left = new binTree.BinaryTreeNode(binTree.types.Inactive, 0f);
                actualNode.right = new binTree.BinaryTreeNode(binTree.types.Inactive, 0f);
            }
        }
        else if (op.Contains("se")) // esempio se>={4}
        {
            tree.Insert(new binTree.BinaryTreeNode(binTree.types.DomainDefinition, float.Parse((op.Substring(op.IndexOf('{') + 1)).Trim('}'))), actualNode);
            if (op.Contains(">=")) actualNode.dec = (int) binTree.comparator.GreaterOrEqual;
            else if(op.Contains("<=")) actualNode.dec = (int)binTree.comparator.LowerOrEqual;
            else if (op.Contains("!=")) actualNode.dec = (int)binTree.comparator.Different;
            else if (op.Contains('=')) actualNode.dec = (int)binTree.comparator.Equal;
            else if (op.Contains('>')) actualNode.dec = (int)binTree.comparator.Greater;
            else if (op.Contains('<')) actualNode.dec = (int)binTree.comparator.Lower;

            actualNode.left = new binTree.BinaryTreeNode(binTree.types.OutOfDomain, 0f);
            actualNode.right = new binTree.BinaryTreeNode(binTree.types.OutOfDomain, 0f);
        }
        else if (op.Contains("pi"))
        {
            if (op.Trim().Equals("pi")) tree.Insert(new binTree.BinaryTreeNode(binTree.types.Operand, 1f), actualNode);
            else if (op.Trim().Equals("-pi")) tree.Insert(new binTree.BinaryTreeNode(binTree.types.Operand, -1f), actualNode);
            else tree.Insert(new binTree.BinaryTreeNode(binTree.types.Operand, float.Parse(op.Substring(0, op.IndexOf("pi")))), actualNode);
            actualNode.pi = true;
        }
        else if (op.Contains("e"))
        {

            if (op.Trim().Equals("e")) tree.Insert(new binTree.BinaryTreeNode(binTree.types.Operand, 1f), actualNode);
            else if (op.Trim().Equals("-e")) tree.Insert(new binTree.BinaryTreeNode(binTree.types.Operand, -1f), actualNode);
            else tree.Insert(new binTree.BinaryTreeNode(binTree.types.Operand, float.Parse(op.Substring(0, op.IndexOf('e')))), actualNode);
            actualNode.nepero = true;
        }
        else if (op.Contains('x'))
        {
            if (op.Trim().Equals("x")) tree.Insert(new binTree.BinaryTreeNode(binTree.types.Variable, 1f), actualNode);
            else if (op.Trim().Equals("-x")) tree.Insert(new binTree.BinaryTreeNode(binTree.types.Variable, -1f), actualNode);
            else tree.Insert(new binTree.BinaryTreeNode(binTree.types.Variable, float.Parse(op.Substring(0, op.IndexOf('x')))), actualNode);
        }
        else if (op.Contains('y'))
        {
            if (op.Trim().Equals("y")) tree.Insert(new binTree.BinaryTreeNode(binTree.types.Variable2, 1f), actualNode);
            else if (op.Trim().Equals("-y")) tree.Insert(new binTree.BinaryTreeNode(binTree.types.Variable2, -1f), actualNode);
            else tree.Insert(new binTree.BinaryTreeNode(binTree.types.Variable2, float.Parse(op.Substring(0, op.IndexOf('y')))), actualNode);
        }
        else if (op.Contains("NaN"))
            tree.Insert(new binTree.BinaryTreeNode(binTree.types.OutOfDomain, 0f), actualNode);
        else
            tree.Insert(new binTree.BinaryTreeNode(binTree.types.Operand, float.Parse(op.Trim())), actualNode);

        if (actualNode.left != null)
            img = FillTree(tree, actualNode.left, img.Substring(img.IndexOf(';') + 1));
            
        if (actualNode.right != null)
            img = FillTree(tree, actualNode.right, img.Substring(img.IndexOf(';') + 1));

        return img;
    }

    public static string Formula(string img)
    {
        int rows = int.Parse(img.Substring(0, img.IndexOf(';')));
        string[] formulas = img.Substring(img.IndexOf(';') + 1).Split(';', System.StringSplitOptions.RemoveEmptyEntries);
        string res = "";
        for (int i = 0; i < rows; i++)
            res += formulas[i].Trim() + "\n";
        return keysGeneral.ReplaceMathInImage(res).Trim();
    }
}
