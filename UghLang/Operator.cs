namespace UghLang;

public enum Operator
{
    Equals,
    NotEquals,
    Plus,
    Minus,
    Multiply,
    Divide,
    PlusEquals,
    MinusEquals,
    MultiplyEquals,
    DivideEquals,

    // BOOLEAN
    Less,
    Higher,
    DoubleEquals,
    LessEquals,
    HigherEquals,
}
public record Pair(dynamic Left,dynamic Right,Operator Operator);
public static class Operation
{
    public static dynamic Operate(dynamic left, dynamic right, Operator opr)
    {
        return opr switch
        {
            Operator.Equals => right,
            Operator.NotEquals => left != right,
            Operator.Plus or Operator.PlusEquals => left + right,
            Operator.Minus or Operator.MinusEquals => left - right,
            Operator.Multiply or Operator.MultiplyEquals => left * right,
            Operator.Divide or Operator.DivideEquals => left / right,
            Operator.DoubleEquals => left == right,
            Operator.Higher => left > right,
            Operator.Less => left < right,
            Operator.HigherEquals => left >= right,
            Operator.LessEquals => left <= right,
            _ => left,
        };
    }
    public static dynamic Operate(this Pair pair) => Operate(pair.Left,pair.Right,pair.Operator);



    //public static dynamic Operate(Operation operation)
    //{
    //    var type = operation.Values[0].GetType();
    //    List<dynamic> results = new();
    //    //

    //    for (int i = 0; i < operation.Values.Count; i++)
    //    {
    //        var current = operation.Values[i];

    //        if (current.Operator is not null)
    //        {
    //            // operate 
    //            if (!TryGetPrevious(out Value left)) throw new NullReferenceException("Cannot find left side of operation");

    //            if (!TryGetNext(out Value right)) throw new NullReferenceException("Cannot find right side of operation");

    //            // TODO: Find other way than setting result
    //            results.Add(Operate(left.Dynamic, right.Dynamic, current.Operator ?? default));

    //            Skip();
    //        }


    //        void Skip(int skips = 1)
    //        {
    //            if (i + skips < operation.Values.Count)
    //                i+=skips;
    //        }
    //        bool TryGetNext(out Value val)
    //        {
    //            if (i + 1 < operation.Values.Count)
    //            {
    //                val = operation.Values[i + 1];
    //                return true;
    //            }
    //            val = Value.NULL;
    //            return false;
    //        }
    //        bool TryGetPrevious(out Value val)
    //        {
    //            if (i - 1 < operation.Values.Count)
    //            {
    //                val = operation.Values[i - 1];
    //                return true;
    //            }
    //            val = Value.NULL;
    //            return false;
    //        }
    //    }
    //    dynamic result = type.IsValueType ? Activator.CreateInstance(type) ?? 0 : 0;
    //    foreach (var value in results) {
    //        result += value;
    //    }
    //    return result;
    //}
    //public dynamic Operate() => Operate(this);


    public static Operator GetOperator(this string opr)
    {
        return opr switch
        {
            "=" => Operator.Equals ,
            "!=" => Operator.NotEquals,
            "+" => Operator.Plus,
            "-" => Operator.Minus,
            "*" => Operator.Multiply,
            "/" => Operator.Divide,
            "+=" => Operator.PlusEquals,
            "-=" => Operator.MinusEquals,
            "*=" => Operator.MultiplyEquals,
            "/=" => Operator.DivideEquals,
            "==" => Operator.DoubleEquals,
            "<" => Operator.Less,
            ">" => Operator.Higher,
            "<=" => Operator.LessEquals,
            ">=" => Operator.HigherEquals,
            _ => throw new("Cannot find operator " + opr),
        };
    }
}
