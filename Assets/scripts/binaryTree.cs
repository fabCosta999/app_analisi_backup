using Unity.VisualScripting;
using UnityEngine;


public static class binTree
{
    public enum types
    {
        Operand, Operator, Variable, Variable2, DomainDefinition, OutOfDomain, Inactive
    }

    public enum operators
    {
        Subtract = 13, Multiply, Divide, Pow, Sum, Log, Sin, Cos, Tan, Sign, Floor, Ceil, Max, Min, Abs, __Fact__, Asin, Acos, Atan 
    }

    public enum comparator
    {
        Greater, GreaterOrEqual, Lower, LowerOrEqual, Equal, Different
    }

    public class BinaryTreeNode
    {
        public types type; 
        public BinaryTreeNode left;
        public BinaryTreeNode right;
        public BinaryTreeNode top;
        public float value; 
        public int dec; 
        public bool nepero;
        public bool pi;

        public BinaryTreeNode(types _type, float _value)
        {
            type = _type;
            value = _value;
            dec = 0;
            nepero = false;
            pi = false;
            top = null;
            left = null;
            right = null;
        }

        public void Copy(BinaryTreeNode node)
        {
            type = node.type;
            value = node.value;
            top = node.top;
            left = node.left;
            right = node.right;
        }
    }

    public class BinaryTree
    {
        public BinaryTreeNode root;

        public BinaryTree(BinaryTreeNode node)
        {
            root = node;
        }

        public void Canc(BinaryTreeNode node)
        {
            if (node.type == types.Inactive) return;
            if (node.left != null)
            {
                RCanc(node.left);
                node.left = null;
            }
            if (node.right != null)
            {
                RCanc(node.right);
                node.right = null;
            }
            node.type = types.Inactive;
            node.value = 0f;
            node.dec = 0;
            node.nepero = false;
            node.pi = false;
        }

        private void RCanc(BinaryTreeNode node)
        {
            if (node.left != null)
            {
                RCanc(node.left);
                node.left = null;
            }
            if (node.right != null)
            {
                RCanc(node.right);
                node.right = null;
            }
        }

        public BinaryTreeNode Insert(BinaryTreeNode node, BinaryTreeNode actual)
        {
            node.top = actual.top;
            node.left = actual.left;
            node.right = actual.right;
            actual.Copy(node);
            return actual;
        }

        public float Evaluate(float x, float y=0f)
        {
            return REvaluate(root, x, y);
        }

        private float REvaluate(BinaryTreeNode node, float x, float y=0f)
        {
            float res = 0f;
            if (node.type == types.OutOfDomain) return float.NaN;
            if (node == null || node.type == types.Inactive) return 0;
            if (node.type == types.Operand)
            {
                float val = node.value;
                if (node.pi) val *= Mathf.PI;
                if (node.nepero) val *= Mathf.Exp(1);
                return val;
            }
            if (node.type == types.Variable)
            {
                float val = node.value * x;
                if (node.pi) val *= Mathf.PI;
                if (node.nepero) val *= Mathf.Exp(1);
                return val;
            }
            if (node.type == types.Variable2)
            {
                float val = node.value * y;
                if (node.pi) val *= Mathf.PI;
                if (node.nepero) val *= Mathf.Exp(1);
                return val;
            }
            if (node.type == types.DomainDefinition)
            {
                switch (node.dec)
                {
                    case (int)comparator.Greater:
                        if (x > node.value) return REvaluate(node.left, x, y);
                        else return REvaluate(node.right, x, y);
                    case (int)comparator.GreaterOrEqual:
                        if (x - node.value > 0.00001f) return REvaluate(node.left, x, y);
                        else return REvaluate(node.right, x, y);
                    case (int)comparator.Lower:
                        if (x < node.value) return REvaluate(node.left, x, y);
                        else return REvaluate(node.right, x, y);
                    case (int)comparator.LowerOrEqual:
                        if (x - node.value < 0.00001f) return REvaluate(node.left, x, y);
                        else return REvaluate(node.right, x, y);
                    case (int)comparator.Equal:
                        if (Mathf.Abs(x - node.value) < 0.00001f) return REvaluate(node.left, x , y);
                        else return REvaluate(node.right, x, y);
                    case (int)comparator.Different:
                        if (Mathf.Abs(x - node.value) > 0.00001f) return REvaluate(node.left, x, y);
                        else return REvaluate(node.right, x, y);
                }
            }
            switch ((operators)(int)node.value)
            {
                case operators.Subtract: res = REvaluate(node.left, x, y) - REvaluate(node.right, x, y); break;
                case operators.Multiply: res = REvaluate(node.left, x, y) * REvaluate(node.right, x, y); break;
                case operators.Divide: res = REvaluate(node.left, x, y) / REvaluate(node.right, x, y); break;
                case operators.Pow: 
                    res = REvaluate(node.left, x, y);
                    if (res >= 0) res = Mathf.Pow(res, REvaluate(node.right, x, y));
                    else
                    {
                        float right = REvaluate(node.right, x, y);
                        if (right == (int)right) res = Mathf.Pow(res, right);
                        else
                        {
                            int lastChecked = 1000;
                            float sensibility = 0.00001f;
                            bool found = false;
                            for (int i=2; i < lastChecked; i++)
                            {
                                if (Mathf.Abs(right * i - (int) (right * i)) < sensibility)
                                {
                                    res = i % 2 == 0 ? float.NaN : -Mathf.Pow(-res, right);
                                    found = true;
                                    break;
                                }
                            }
                            if (!found) res = float.NaN;
                        }
                    }
                    break;
                case operators.Sum: res = REvaluate(node.left, x, y) + REvaluate(node.right, x, y); break;
                case operators.Log: res = Mathf.Log(REvaluate(node.right, x, y), REvaluate(node.left, x, y)); break;
                case operators.Sin: res = Mathf.Sin(REvaluate(node.right, x, y)); break;
                case operators.Cos: res = Mathf.Cos(REvaluate(node.right, x, y)); break;
                case operators.Tan: res = Mathf.Tan(REvaluate(node.right, x, y)); break;
                case operators.Sign: 
                    res = REvaluate(node.right, x, y);
                    if (res == 0) res = 0;
                    else if (res > 0) res = 1;
                    else if (res < 0) res = -1;
                    break;
                case operators.Floor: res = Mathf.Floor(REvaluate(node.right, x, y)); break;
                case operators.Ceil: res = Mathf.Ceil(REvaluate(node.right, x, y)); break;
                case operators.Max: res = Mathf.Max(REvaluate(node.left, x, y), REvaluate(node.right, x, y)); break;
                case operators.Min: res = Mathf.Min(REvaluate(node.left, x, y), REvaluate(node.right, x, y)); break;
                case operators.Abs: res = Mathf.Abs(REvaluate(node.right, x, y)); break;
                case operators.Asin: res = Mathf.Asin(REvaluate(node.right, x, y)); break;
                case operators.Acos: res = Mathf.Acos(REvaluate(node.right, x, y));break;
                case operators.Atan: res = Mathf.Atan(REvaluate(node.right, x, y)); break;
                default: res = 0f; break;
            }
            return node.dec == 0? res : -res;
        }

        public string Print(BinaryTreeNode node)
        {
            return InOrderVisit(root, 0, node);
        }

        public string FullPrint()
        {
            return InOrderVisit(root, 1, root);
        }


        // da aggiornare se si da all'utente la possibilità di definire a tratti
        private string InOrderVisit(BinaryTreeNode node, int mode, BinaryTreeNode actual)
        {
            string a;
            if (node == null) return "";
            if (node.type == types.Inactive)
            {
                if (mode == 1) return "0";
                if (mode == 0 && node == actual) return "<color=blue>_</color>";
                return "";
            }
            switch (node.type)
            {
                case types.Operator:
                    if (mode == 0 && node == actual) a = "<color=blue>";
                    else a = "";
                    if (node.dec == 1) a += "-";
                    if ((operators)node.value == operators.Log) a += "log";
                    else if ((operators)node.value == operators.Max) a += "max{";
                    else if ((operators)node.value == operators.Min) a += "min{";
                    else if ((operators)node.value == operators.Sum ||
                             (operators)node.value == operators.Subtract ||
                             (operators)node.value == operators.Multiply ||
                             (operators)node.value == operators.Divide) a += "(";
                    else if ((operators)node.value == operators.Pow) a += "((";
                    if (mode == 0 && node == actual) a += "</color>";

                    a += InOrderVisit(node.left, mode, actual);
                    if (mode == 0 && node == actual) a += "<color=blue>";
                    switch ((operators)node.value)
                    {
                        case operators.Subtract: a += "-"; break;
                        case operators.Multiply: a += "*"; break;
                        case operators.Divide: a += "/"; break;
                        case operators.Pow: a += ")^"; break;
                        case operators.Sum: a += "+"; break;
                        case operators.Log: a += "("; break;
                        case operators.Sin: a += "sin("; break;
                        case operators.Cos: a += "cos("; break;
                        case operators.Tan: a += "tan("; break;
                        case operators.Sign: a += "sign("; break;
                        case operators.Min: a += ";"; break;
                        case operators.Max: a += ";"; break;
                        case operators.Floor: a += "floor("; break;
                        case operators.Ceil: a += "ceil("; break;
                        case operators.Abs: a += "|"; break;
                        case operators.__Fact__: a += "("; break;
                        case operators.Asin: a += "asin("; break;
                        case operators.Acos: a += "acos("; break;
                        case operators.Atan: a += "atan("; break;
                    }
                    if (mode == 0 && node == actual) a += "</color>";
                    a += InOrderVisit(node.right, mode, actual);
                    if (mode == 0 && node == actual) a += "<color=blue>";
                    switch ((operators)node.value)
                    {
                        case operators.Subtract: a += ")"; break;
                        case operators.Multiply: a += ")"; break;
                        case operators.Divide: a += ")"; break;
                        case operators.Pow: a += ")"; break;
                        case operators.Sum: a += ")"; break;
                        case operators.Log: a += ")"; break;
                        case operators.Sin: a += ")"; break;
                        case operators.Cos: a += ")"; break;
                        case operators.Tan: a += ")"; break;
                        case operators.Sign: a += ")"; break;
                        case operators.Floor: a += ")"; break;
                        case operators.Ceil: a += ")"; break;
                        case operators.Max: a += "}"; break;
                        case operators.Min: a += "}"; break;
                        case operators.Abs: a += "|"; break;
                        case operators.__Fact__: a += ")!"; break;
                        case operators.Asin: a += ")"; break;
                        case operators.Acos: a += ")"; break;
                        case operators.Atan: a += ")"; break;
                    }
                    if (mode == 0 && node == actual) a += "</color>";
                    break;
                case types.Operand:
                    if (mode == 0 && node == actual) a = "<color=blue>" + node.value.ToString();
                    else a = node.value.ToString();
                    if (mode == 0 && node.dec > 0 && node.value == (float)((int)node.value)) a += ",";
                    if (node.pi) a += "pi";
                    if (node.nepero) a += "e";
                    if (mode == 0 && node == actual) a += "</color>";
                    break;
                case types.Variable:
                    if (mode == 0 && node == actual) a = "<color=blue>";
                    else a = "";
                    a += node.value.ToString();
                    if (node.pi) a += "pi";
                    if (node.nepero) a += "e";
                    a += "x";
                    if (mode == 0 && node == actual) a += "</color>";
                    break;
                default:
                    if (mode == 1) a = "0";
                    else if (mode == 0 && node == actual) a = "<color=blue>_</color>";
                    else a = "";
                    break;
            }
            a = a.Replace("1e", "e");
            a = a.Replace("1pi", "pi");
            a = a.Replace("1x", "x");
            a = a.Replace("1ex", "ex");
            a = a.Replace("1pix", "pix");
            a = a.Replace("1pie", "pie");
            a = a.Replace("1piex", "piex");
            a = a.Replace("loge", "ln");
            return a;
        }

        public BinaryTreeNode FindPrec(BinaryTreeNode n)
        {
            BinaryTreeNode n1 = n;
            BinaryTreeNode p;
            if (n == null) return null;
            if (n.left != null) return n.left;
            while (true)
            {
                p = n.top;
                if (p == null) return n1;
                if (p.right == n) return p;
                n = p;
            }
        }

        public BinaryTreeNode FindSucc(BinaryTreeNode n)
        {
            BinaryTreeNode n1 = n;
            BinaryTreeNode p;
            if (n == null) return null;
            if (n.right != null) return n.right;
            while (true)
            {
                p = n.top;
                if (p == null || p.type == types.Inactive) return n1;
                if (p.left == n) return p;
                n = p;
            }
        }
    }
}
