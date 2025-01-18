﻿using UghLang.Nodes;
namespace UghLang;


[Serializable]
public class UghException : Exception
{
    public override string? StackTrace => Debug.EnabledMessages? base.StackTrace : string.Empty;
    public UghException(string mess) => Debug.Error(mess);
}
public class UndefinedInstructions() : UghException("Undefined instructions");
public class InvalidSpellingException(ASTNode node, string mess = "") : UghException($"Invalid spelling of {node}{mess}") { }
public class ExpectedException(string thing, ASTNode node) : InvalidSpellingException(node, ": expected " + thing) { }
public class IncorrectArgumentsException(Name name) : UghException($"Incorrect arguments or their count called for `{name.Key}`") { }
