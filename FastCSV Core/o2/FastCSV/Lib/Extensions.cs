
namespace o2.FastCSV.Extensions
{
    public static class Extensions
    {

        public static bool IsBetween(this int number, int min, int max, bool inclusive = true)
        {
            return inclusive
                ? number >= min && number <= max
                : number > min && number < max;
        }

        public static bool IsBetween(this double number, double min, double max, bool inclusive = true)
        {
            return inclusive
                ? number >= min && number <= max
                : number > min && number < max;
        }
    }


}
