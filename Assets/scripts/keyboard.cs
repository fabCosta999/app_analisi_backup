using UnityEngine;
using TMPro;

public class keyboard : MonoBehaviour
{
    public TextMeshProUGUI function, lastFunction;
    private binTree.BinaryTreeNode actualNode;
    private binTree.BinaryTree functionTree, lastFunctionTree;


    private void Start()
    {
        lastFunction.text = "y=0";
        functionTree = new binTree.BinaryTree(new binTree.BinaryTreeNode(binTree.types.Inactive, 0));
        lastFunctionTree = new binTree.BinaryTree(new binTree.BinaryTreeNode(binTree.types.Operand, 0));
        actualNode = functionTree.root;
        function.text = "y=" + functionTree.Print(actualNode);
    }

    public void buttonPressed(int num)
    {
        if (num < 10) AddDigit(num);
        else if (num >= 13 && num <= 31)
        {
            switch ((binTree.operators)num)
            {
                case binTree.operators.Subtract: TwoOperator((binTree.operators)num); break;
                case binTree.operators.Multiply: TwoOperator((binTree.operators)num); break;
                case binTree.operators.Divide: TwoOperator((binTree.operators)num); break;
                case binTree.operators.Pow: TwoOperator((binTree.operators)num); break;
                case binTree.operators.Sum: TwoOperator((binTree.operators)num); break;
                case binTree.operators.Log: TwoOperator((binTree.operators)num); break;
                case binTree.operators.Sin: OneOperator((binTree.operators)num); break;
                case binTree.operators.Cos: OneOperator((binTree.operators)num); break;
                case binTree.operators.Tan: OneOperator((binTree.operators)num); break;
                case binTree.operators.Asin: OneOperator((binTree.operators)num); break;
                case binTree.operators.Acos: OneOperator((binTree.operators)num); break;
                case binTree.operators.Atan: OneOperator((binTree.operators)num); break;
                case binTree.operators.Sign: OneOperator((binTree.operators)num); break;
                case binTree.operators.Floor: OneOperator((binTree.operators)num); break;
                case binTree.operators.Ceil: OneOperator((binTree.operators)num); break;
                case binTree.operators.Max: TwoOperator((binTree.operators)num); break;
                case binTree.operators.Min: TwoOperator((binTree.operators)num); break;
                case binTree.operators.Abs: OneOperator((binTree.operators)num); break;
                case binTree.operators.__Fact__: break;
            }
        }
        else
        {
            switch (num)
            {
                case 10: Canc(); break;
                case 11: Enter(); break;
                case 12: ChangeSign(); break;
                case 34: GoLeft(); break;
                case 35: GoRight(); break;
                case 36: AddNepero(); break;
                case 32: AddPi();  break;
                case 33: Decimal();  break;
                case 50: AddVariable();  break; 
                default:
                    Debug.LogError("input imprevisto");
                    break;
            }
        }
        function.text = "y=" + functionTree.Print(actualNode);
    }

    private void Canc()
    {
        if (actualNode.type == binTree.types.Inactive) return;
        functionTree.Canc(actualNode);
    }

    private void Enter()
    {
        lastFunctionTree = null;
        lastFunctionTree = functionTree;
        lastFunction.text = "y=" + functionTree.FullPrint();
        function.text = "y=";
        functionTree = new binTree.BinaryTree(new binTree.BinaryTreeNode(binTree.types.Inactive, 0));
        actualNode = functionTree.root;
    }

    private void ChangeSign()
    {
        if (actualNode.type == binTree.types.Inactive) return;
        if (actualNode.type == binTree.types.Operator) actualNode.dec = actualNode.dec == 0 ? 1 : 0;
        else actualNode.value *= -1;
    }

    private void TwoOperator(binTree.operators op)
    {
        binTree.BinaryTreeNode n = new binTree.BinaryTreeNode(binTree.types.Operator, (float)op);
        n = functionTree.Insert(n, actualNode);
        if (n.left == null) n.left = new binTree.BinaryTreeNode(binTree.types.Inactive, 0f);
        if (n.right == null) n.right = new binTree.BinaryTreeNode(binTree.types.Inactive, 0f);
        n.left.top = n;
        n.right.top = n;
        actualNode = n.left;

    }

    private void OneOperator(binTree.operators op)
    {
        binTree.BinaryTreeNode n = new binTree.BinaryTreeNode(binTree.types.Operator, (float)op);
        n = functionTree.Insert(n, actualNode);
        if (n.right == null) n.right = new binTree.BinaryTreeNode(binTree.types.Inactive, 0f);
        n.right.top = n;
        n.left = null;
        actualNode = n.right;
    }

    private void AddDigit(int digit)
    {
        if (actualNode.type != binTree.types.Operand)
        {
            functionTree.Canc(actualNode);
            functionTree.Insert(new binTree.BinaryTreeNode(binTree.types.Operand, (float)digit), actualNode);
            actualNode.left = null;
            actualNode.right = null;
            return;
        }
        if (actualNode.dec == 0)
        {
            actualNode.value *= 10;
            if (actualNode.value >= 0) actualNode.value += (float)digit;
            else actualNode.value -= (float)digit;
        }
        else
        {
            float val = (float)digit / Mathf.Pow(10, actualNode.dec);
            if (actualNode.value >= 0) actualNode.value += val;
            else actualNode.value -= val;
            actualNode.dec++;
        }
    }

    private void AddVariable()
    {
        if (actualNode.type == binTree.types.Variable) return;
        if (actualNode.type == binTree.types.Operator || actualNode.type == binTree.types.Inactive)
        {
            functionTree.Canc(actualNode);
            functionTree.Insert(new binTree.BinaryTreeNode(binTree.types.Variable, 1f), actualNode);
            actualNode.left = null;
            actualNode.right = null;
            return;
        }
        actualNode.type = binTree.types.Variable;
    }

    private void AddNepero()
    {
        if (actualNode.type != binTree.types.Operand)
        {
            functionTree.Canc(actualNode);
            functionTree.Insert(new binTree.BinaryTreeNode(binTree.types.Operand, 1f), actualNode);
            actualNode.left = null;
            actualNode.right = null;
        }
        actualNode.nepero = true;
    }

    private void AddPi()
    {
        if (actualNode.type != binTree.types.Operand)
        {
            functionTree.Canc(actualNode);
            functionTree.Insert(new binTree.BinaryTreeNode(binTree.types.Operand, 1f), actualNode);
            actualNode.left = null;
            actualNode.right = null;
        }
        actualNode.pi = true;
    }

    private void GoLeft()
    {
        actualNode = functionTree.FindPrec(actualNode);
    }

    private void GoRight()
    {
        actualNode = functionTree.FindSucc(actualNode);
    }

    private void Decimal()
    {
        if (actualNode.type == binTree.types.Operand && actualNode.dec == 0) actualNode.dec++;
    }

    public float Function(float x)
    {
        return lastFunctionTree.Evaluate(x);
    }
}
