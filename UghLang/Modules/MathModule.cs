﻿namespace UghLang.Modules;

[Module("Math")]
public static class MathModule
{
    public const double PI = Math.PI;
    public const double TAU = Math.Tau;
    public const double E = Math.E;

    public static double Clamp(double val, double min, double max) => Math.Clamp(val, min, max);
    public static double Min(double a, double b) => Math.Min(a, b);
    public static double Max(double a, double b) => Math.Max(a, b);
    public static double Cos(double d) => Math.Cos(d);
    public static double Sin(double a) => Math.Sin(a);
    public static double Abs(double x) => Math.Abs(x);
    public static double Rev(double x) => -x;
}
