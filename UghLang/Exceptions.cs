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
public class MissingException(string thing, ASTNode node) : InvalidSpellingException(node, ": missing " + thing) { }
public class IncorrectArgumentsException(Name name) : UghException($"Incorrect arguments or their count called for `{name.Key}`") { }

public class SilentUghException(string mess) : UghException(mess)
{
    public override string? StackTrace => string.Empty;
}
