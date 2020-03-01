using System;
using System.Windows.Controls;

namespace BWTVisualisation
{
    class Utils
    {
        public static char[,] GetCiclMatrix(string text)
        {
            char[,] ciclMatrix = new char[text.Length, text.Length];
            int c = 0;

            foreach (var ch in text)
            {
                ciclMatrix[0, c] = ch;
                c++;
            }

            int indexLast = text.Length - 1;

            for (int i = 1; i < text.Length; i++)
            {
                for (int j = 0; j < text.Length; j++)
                {
                    ciclMatrix[i, j] = j == indexLast ? ciclMatrix[i - 1, 0] : ciclMatrix[i - 1, j + 1];
                }
            }

            return ciclMatrix;
        }

        public static char[,] TextBlockArrayToCharArray(TextBlock[,] textBlock)
        {
            char[,] result = new char[textBlock.GetUpperBound(0) + 1, textBlock.GetUpperBound(1) + 1];

            for (int i = 0; i < textBlock.GetUpperBound(0) + 1; i++)
            {
                for (int j = 0; j < textBlock.GetUpperBound(1) + 1; j++)
                {
                    result[i, j] = textBlock[i, j].Text.ToCharArray()[0];
                }
            }

            return result;
        }

        public static char[,] ConvertToCharArray(string[] array)
        {
            var charArray = new char[array.Length, array[0].Length];
            int i = 0;

            foreach (var row in array)
            {
                int j = 0;
                foreach (var cells in row)
                {
                    charArray[i, j] = cells;
                    j++;
                }
                i++;
            }

            return charArray;
        }

        public static string[] ConvertToStringArray(char[,] matrixCicl)
        {
            var array = new string[matrixCicl.GetUpperBound(0) + 1];

            for (int i = 0; i < matrixCicl.GetUpperBound(0) + 1; i++)
            {
                string row = String.Empty;

                for (int j = 0; j < matrixCicl.GetUpperBound(1) + 1; j++)
                {
                    row += matrixCicl[i, j].ToString();
                }

                array[i] = row;
            }

            return array;
        }
    }
}
