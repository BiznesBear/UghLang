﻿namespace UghLang.Modules;

[Module("String")]
public static class StringModule
{
    public static int Lenght(string str) => str.Length;
    public static bool Contains(string str, string value) => str.Contains(value);
}