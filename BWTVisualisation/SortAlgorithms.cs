using System.Windows;

namespace BWTVisualisation
{
    public partial class MainWindow : Window
    {
        public void BubbleSort(string[] array, bool withVisualisation)
        {
            bool madeChanges;
            int itemCount = array.Length;

            do
            {
                madeChanges = false;
                itemCount--;

                for (int i = 0; i < itemCount; i++)
                {
                    if (withVisualisation)
                    {
                        //Обозначаем сравниваемые строки оранжевым цветом
                        SetComparingRowColor(i, i + 1);
                    }

                    if (comparer.Compare(array[i], array[i + 1]) > 0)
                    {
                        string temp = array[i + 1];
                        array[i + 1] = array[i];
                        array[i] = temp;
                        madeChanges = true;

                        if (withVisualisation)
                        {
                            //Меняем местами сравниваемые строки
                            Swaplines(i, i + 1);
                        }
                    }

                    if (withVisualisation)
                    {
                        //Обозначаем пройденные строки красным цветом
                        SetPassedColor(i, i + 1);
                    }
                }

                if (withVisualisation)
                {
                    SetFixedColor(itemCount);

                    //если остался только один элемент в неотсортированном массиве
                    if (itemCount == 1)
                    {
                        SetFixedColor(0);
                    }
                }

            } while (madeChanges);
        }

        public void InsertionSort(string[] array)
        {
            string temp;

            for (int i = 1; i < array.Length; i++)
            {
                for (int j = i; j > 0; j--)
                {
                    //Обозначаем сравниваемые строки оранжевым цветом
                    SetComparingRowColor(j, j - 1);
                    if (comparer.Compare(array[j - 1], array[j]) > 0)
                    {
                        //Меняем местами сравниваемые строки
                        swapLines(array, j, j - 1);
                    }

                    //Обозначаем пройденные строки красным цветом
                    SetPassedColor(j, j - 1);

                    if (comparer.Compare(sortedArray[j], array[j]) == 0)
                    {
                        //Обозначаем строки, 
                        //что уже не будут менять своего местоположения синим цветом 
                        SetFixedColor(j);
                    }

                    if (comparer.Compare(sortedArray[j - 1], array[j - 1]) == 0)
                    {
                        //Обозначаем строки, 
                        //что уже не будут менять своего местоположения синим цветом 
                        SetFixedColor(j - 1);
                    }
                }
            }
        }

        private void swapLines(string[] array, int index1, int index2)
        {
            string temp;

            temp = array[index1];
            array[index1] = array[index2];
            array[index2] = temp;

            Swaplines(index1, index2);
        }
        
        private void ShakerSort(string[] array)
        {
            int leftMark = 1;
            int rightMark = array.Length - 1;

            while (leftMark <= rightMark + 1)
            {
                for (int i = leftMark; i <= rightMark; i++)
                {
                    SetComparingRowColor(i - 1, i);

                    if (comparer.Compare(array[i - 1], array[i]) > 0)
                    {
                        swapLines(array, i - 1, i);
                    }

                    SetPassedColor(i - 1, i);
                }

                SetFixedColor(rightMark);

                rightMark--;

                for (int i = rightMark; i >= leftMark; i--)
                {
                    //Обозначаем сравниваемые строки оранжевым цветом
                    SetComparingRowColor(i - 1, i);

                    if (comparer.Compare(array[i - 1], array[i]) > 0)
                    {
                        swapLines(array, i - 1, i);
                    }

                    //Обозначаем пройденные строки красным цветом
                    SetPassedColor(i - 1, i);
                }

                SetFixedColor(leftMark - 1);

                leftMark++;
            }
        }
    }
}
