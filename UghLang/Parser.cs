﻿namespace UghLang;

public class Parser
{
    public readonly AST AST;
    public string treeString = "+--";

    private ASTNode currentNode;
    private Stack<ASTNode> masterBranches = new();

    private string debugTree = string.Empty;
    private string recoveryDebugTree = string.Empty;

    public Parser(Ugh ugh)
    {
        AST = new(ugh);
        currentNode = AST;
    }

    public void AddToken(Token token)
    {
        switch (token.Type)
        {
            case TokenType.Keyword:
                EnterNode((token.Keyword ?? default).GetNode());
                break;
            case TokenType.OpenExpression:
                // add new expression
                EnterNode(new ExpressionNode() { Value = token.Value });
                break;
            case TokenType.CloseExpression:
                // exit and calculate current expression
                QuitNode<ExpressionNode>();
                break;

            case TokenType.Operator:
                CreateNode(new OperatorNode() { Operator = token.Operator });
                break;

            case TokenType.OpenBlock:
                EnterNode(new TagNode());
                CreateMasterBranch();
                break;
            case TokenType.CloseBlock:
                QuitNode<TagNode>();
                RemoveMasterBranch<TagNode>();
                break;
            case TokenType.StringValue or TokenType.IntValue or TokenType.BoolValue:
                // add new value to expression
                CreateNode(new ValueNode() { Value = token.Value });
                break;
            case TokenType.Separator:
                // end branch
                BackToMasterBranch();
                break;
            case TokenType.None:
                if (IsMasterBranch()) EnterNode(new InitVariableNode() { Token = token });
                else CreateNode(new RefrenceNode() { Token = token });
                break;
            default:
                Debug.Print("Somthing is unhandled");
                break;
        }
    }


    #region NodeManagment

    /// <summary>
    /// Adds node to currentNode 
    /// </summary>
    /// <param name="node"></param>
    private void CreateNode(ASTNode node) 
    { 
        currentNode.AddNode(node); 
        Debug.Print(debugTree + node); 
    }

    /// <summary>
    /// Add node to currentNode and becomes currentNode
    /// </summary>
    /// <param name="node"></param>
    private void EnterNode(ASTNode node)
    {
        CreateNode(node);
        debugTree += treeString;
        currentNode = node;
    }

    /// <summary>
    /// If the same type as currentNode then quits to parent of currentNode
    /// </summary>
    /// <typeparam name="T"></typeparam>
    private void QuitNode<T>()
    {
        if (CurrentNodeIs<T>())
        {
            currentNode = currentNode.Parent;
            debugTree = debugTree.Remove(debugTree.Length - treeString.Length,treeString.Length); // FOR TESTING ONLY
        }
    }

    private void CreateMasterBranch()
    {
        masterBranches.Push(currentNode);
        recoveryDebugTree = debugTree;
    }

    private void RemoveMasterBranch<T>()
    {
        if (GetMasterBranch().GetType() != typeof(T)) throw new Exception("This is not your branch"); // TODO: Add beter exception
        masterBranches.Pop();
        debugTree += treeString;
        currentNode = GetMasterBranch();
    }
    private void BackToMasterBranch()
    {
        currentNode = GetMasterBranch();
        debugTree = recoveryDebugTree;

    }


    /// <summary>
    /// 
    /// </summary>
    private ASTNode GetMasterBranch() => masterBranches.Count < 1? AST : masterBranches.Peek();
    private bool IsMasterBranch() => currentNode == GetMasterBranch();
    private bool CurrentNodeIs<T>() => currentNode.GetType() == typeof(T);


    #endregion
    public void Execute()
    {
        AST.Load();
        AST.Execute();
    }
}
