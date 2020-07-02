namespace Py_Game.GameTools
{
    public class TGeneric
    {
        public static T Iff<T>(bool expression, T trueValue, T falseValue)
        {
            if (expression)
            {
                return trueValue;
            }
            else
            {
                return falseValue;
            }
        }

    } 
}

