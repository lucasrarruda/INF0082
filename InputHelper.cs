namespace LabMod5
{
    class InputHelper
    {
        private static Random _random = new Random();

        public static int[] GenerateRandomIntArray(int size, int minValue = 1, int maxValue = 100)
        {
            int[] numbers = new int[size];
            for (int i = 0; i < size; i++)
            {
                numbers[i] = _random.Next(minValue, maxValue);
            }
            return numbers;
        }

        public static string[] GenerateRandomStringArray(int size, int stringLength = 8)
        {
            string[] strings = new string[size];
            for (int i = 0; i < size; i++)
            {
                strings[i] = GenerateRandomString(stringLength);
            }
            return strings;
        }

        private static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            char[] result = new char[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = chars[_random.Next(chars.Length)];
            }
            return new string(result);
        }

        public static List<List<int>> GenerateRandomNestedLists(int outerSize, int innerSize, int minValue = 1, int maxValue = 100)
        {
            List<List<int>> nestedLists = new List<List<int>>();
            for (int i = 0; i < outerSize; i++)
            {
                List<int> innerList = new List<int>();
                for (int j = 0; j < innerSize; j++)
                {
                    innerList.Add(_random.Next(minValue, maxValue));
                }
                nestedLists.Add(innerList);
            }
            return nestedLists;
        }
    }
}
