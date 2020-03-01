using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace BWTVisualisation
{
    public partial class MainWindow : Window
    {
        private const int BULB_SORT = 0;
        private const int INSERTION_SORT = 1;
        private const int SHAKE_SORT = 2;

        private int sortType = BULB_SORT;
        private string inputText;
        private TextBlock[,] textBlocks;
        private int visualizationDelay = 0;
        private string[] sortedArray;
        private bool endVisualisation = false;
        private Rectangle highligntedArrayElement;

        private char[,] matrix;
        private int rowInit;
        private char[,] tempMatrix;
        private string[] tempArray;
        private int stepSort = 0;
        private string column;
        private string[] initialArray;
        private static StringComparer comparer = StringComparer.Ordinal;

        public MainWindow()
        {
            InitializeComponent();

            ButtonAddColumn.IsEnabled = false;
            InitialSortButton.IsEnabled = false;
            ButtonInversTrans.IsEnabled = false;
            ButtonEndVisualization.IsEnabled = false;
        }

        private void InitButton_Click(object sender, MouseButtonEventArgs e)
        {
            OutputTextBox.Text = "";
            InitRow.Text = "";
            IversionOutput.Text = "";
            endVisualisation = false;
            ButtonEndVisualization.IsEnabled = false;

            inputText = GetTextFromTextFields();
            InitializeTextBlocks(inputText, true);
            stepSort = 0;
            matrix = Utils.TextBlockArrayToCharArray(textBlocks);
            initialArray = Utils.ConvertToStringArray(matrix);
            BubbleSort(initialArray, false);
            sortedArray = initialArray;
            initialArray = Utils.ConvertToStringArray(matrix);
            
            InitialSortButton.IsEnabled = true;
        }

        private void InitialSortButton_Click(object sender, MouseButtonEventArgs e)
        {
            InitButton.IsEnabled = false;
            ButtonEndVisualization.IsEnabled = true;

            string[] sortArr = Utils.ConvertToStringArray(matrix);

            GetSortType();

            switch (sortType)
            {
                case BULB_SORT:
                    BubbleSort(sortArr, true);
                    break;
                case INSERTION_SORT:
                    InsertionSort(sortArr);
                    break;
                case SHAKE_SORT:
                    ShakerSort(sortArr);
                    break;
            }

            this.Dispatcher.BeginInvoke(DispatcherPriority.SystemIdle, (ThreadStart)delegate ()
            {
                matrix = Utils.ConvertToCharArray(sortArr);
                column = string.Empty;

                for (int i = 0; i < inputText.Length; i++)
                {
                    column += matrix[i, inputText.Length - 1].ToString();
                }
                    
                tempArray = new string[inputText.Length];
                OutputTextBox.Text = column;
                rowInit = Array.FindIndex(sortArr, s => s == inputText);
                InitRow.Text = (rowInit + 1).ToString();
                HighlightColumnOrRow(sortArr.GetUpperBound(0), true);
                HighlightColumnOrRow(rowInit, false);

                InitButton.IsEnabled = true;
                ButtonAddColumn.IsEnabled = true;
            });
        }

        private void ReversSort_Click(object sender, RoutedEventArgs e)
        {
            ButtonAddColumn.IsEnabled = true;
            ButtonInversTrans.IsEnabled = false;
            ButtonEndVisualization.IsEnabled = true;

            if (highligntedArrayElement != null)
            {
                if (stepSort != inputText.Length)
                {
                    for (int j = 0; j < textBlocks.GetUpperBound(0) + 1; j++)
                    {
                        textBlocks[j, 0].Foreground = new SolidColorBrush(Colors.Black);
                    }
                }

                DrawingCanvas.Children.Remove(highligntedArrayElement);
            }

            GetSortType();

            if (stepSort < inputText.Length)
            {
                switch (sortType)
                {
                    case BULB_SORT:
                        BubbleSort(tempArray, true); ;
                        break;
                    case INSERTION_SORT:
                        InsertionSort(tempArray);
                        break;
                    case SHAKE_SORT:
                        ShakerSort(tempArray);
                        break;
                }
                stepSort++;
            }
            else
            {
                IversionOutput.Text = tempArray[rowInit];
                HighlightColumnOrRow(rowInit, false);
                InitButton.IsEnabled = true;
                ButtonAddColumn.IsEnabled = false;
                ButtonInversTrans.IsEnabled = false;
            }
        }

        private void AddColumn_Click(object sender, RoutedEventArgs e)
        {
            ButtonEndVisualization.IsEnabled = false;
            endVisualisation = false;
            InitialSortButton.IsEnabled = false;
            InitButton.IsEnabled = false;
            ButtonAddColumn.IsEnabled = false;
            ButtonInversTrans.IsEnabled = true;
            tempMatrix = new char[inputText.Length, stepSort];
            InitializeTextBlocks("", false);
        }

        private void ButtonEndVisualization_Click(object sender, RoutedEventArgs e)
        {
            endVisualisation = true;

            if (tempArray != null && tempArray[tempArray.GetUpperBound(0)] != null)
            {
                FillMatrixWithTextBlocks(Utils.ConvertToCharArray(tempArray));
                ButtonInversTrans.IsEnabled = false;
                ButtonAddColumn.IsEnabled = true;
            }
            else
            {
                FillMatrixWithTextBlocks(Utils.ConvertToCharArray(sortedArray));
                InitialSortButton.IsEnabled = false;
            }

            visualizationDelay = 0;
            VisualizationDelay.Text = visualizationDelay.ToString();
        }

        private string GetTextFromTextFields()
        {
            string input = InputTextBox.Text;

            if (input.Equals(""))
            {
                input = "Текст по умолчанию";
                InputTextBox.Text = input;
            }
            
            if (VisualizationDelay.Text.Equals(""))
            {
                visualizationDelay = 0;
                VisualizationDelay.Text = (visualizationDelay).ToString();
            }
            else
            {
                visualizationDelay = int.Parse(VisualizationDelay.Text);
            }

            return input;
        }

        private void InitializeTextBlocks(string input, bool init)
        {
            char[,] array;

            if (init)
            {
                array = Utils.GetCiclMatrix(input);

                //Выводим на экран матрицу всех возможных циклических перестановок полученного блока данных
                FillMatrixWithTextBlocks(array);
            }
            else
            {
                if (stepSort < inputText.Length)
                {
                    int i = 0;

                    //добавляем в матрицу новый столбец (трансформированное сообщение)
                    foreach (var c in column)
                    {
                        tempArray[i] = c + tempArray[i];
                        i++;
                    }
                    
                    tempMatrix = Utils.ConvertToCharArray(tempArray);
                    FillMatrixWithTextBlocks(tempMatrix);

                    //Выделяем добавленный столбец
                    HighlightColumnOrRow(0, true);
                }
            }
        }

        public void FillMatrixWithTextBlocks(char[,] array)
        {
            DrawingCanvas.Children.Clear();
            textBlocks = new TextBlock[array.GetUpperBound(0) + 1, array.GetUpperBound(1) + 1];

            for (int i = 0; i < array.GetUpperBound(0) + 1; i++)
            {
                for (int j = 0; j < array.GetUpperBound(1) + 1; j++)
                {
                    TextBlock textBlock = new TextBlock
                    {
                        Text = array[i, j].ToString(),
                        FontSize = 17
                    };

                    Canvas.SetTop(textBlock, 20 * i);
                    Canvas.SetLeft(textBlock, 20 * j);
                    Panel.SetZIndex(textBlock, 1);
                    textBlocks[i, j] = textBlock;
                    DrawingCanvas.Children.Add(textBlock);
                }
            }

            double topOfLastAddedElement = Canvas.GetTop(textBlocks[array.GetUpperBound(0), array.GetUpperBound(1)]);
            double leftOfLastAddedElement = Canvas.GetLeft(textBlocks[array.GetUpperBound(0), array.GetUpperBound(1)]);

            DrawingCanvas.Height = topOfLastAddedElement + 20;
            DrawingCanvas.Width = leftOfLastAddedElement + 20;
        }

        private void GetSortType()
        {
            if (FirstRadioButton.IsChecked == true)
                sortType = BULB_SORT;

            if (SecondRadioButton.IsChecked == true)
                sortType = INSERTION_SORT;

            if (ThirdRadioButton.IsChecked == true)
                sortType = SHAKE_SORT;
        }
        
        private void SetPassedColor(int index1, int index2)
        {
            DrawingCanvas.Dispatcher.BeginInvoke(DispatcherPriority.SystemIdle, (ThreadStart)delegate ()
            {
                if (!endVisualisation)
                {
                    for (int j = 0; j < textBlocks.GetUpperBound(1) + 1; j++)
                    {
                        textBlocks[index2, j].Foreground = new SolidColorBrush(Colors.White);
                        textBlocks[index1, j].Foreground = new SolidColorBrush(Colors.White);
                    }

                    Rectangle rectangle1 = new Rectangle();
                    rectangle1.RadiusX = 5;
                    rectangle1.RadiusY = 5;
                    SolidColorBrush myBrush = new SolidColorBrush(Colors.LightCoral);
                    rectangle1.Fill = myBrush;
                    rectangle1.Height = 19;
                    rectangle1.Width = Canvas.GetLeft(textBlocks[0, textBlocks.GetUpperBound(1)]) + 15;
                    Panel.SetZIndex(rectangle1, -1);
                    rectangle1.Margin = new Thickness(Canvas.GetLeft(textBlocks[index1, 0]),
                        Canvas.GetTop(textBlocks[index1, 0]), 0, 0);

                    Rectangle rectangle2 = new Rectangle();
                    rectangle2.RadiusX = 5;
                    rectangle2.RadiusY = 5;
                    rectangle2.Fill = myBrush;
                    rectangle2.Height = 19;
                    rectangle2.Width = Canvas.GetLeft(textBlocks[0, textBlocks.GetUpperBound(1)]) + 15;
                    Panel.SetZIndex(rectangle2, -1);
                    rectangle2.Margin = new Thickness(Canvas.GetLeft(textBlocks[index2, 0]),
                        Canvas.GetTop(textBlocks[index2, 0]), 0, 0);
                    DrawingCanvas.Children.Add(rectangle1);
                    DrawingCanvas.Children.Add(rectangle2);

                    Thread.Sleep(visualizationDelay);
                }
            });
        }

        private void SetFixedColor(int index1)
        {
            DrawingCanvas.Dispatcher.BeginInvoke(DispatcherPriority.SystemIdle, (ThreadStart)delegate ()
            {
                if (!endVisualisation)
                {
                    for (int j = 0; j < textBlocks.GetUpperBound(1) + 1; j++)
                    {
                        textBlocks[index1, j].Foreground = new SolidColorBrush(Colors.White);
                    }

                    Rectangle rectangle = new Rectangle();
                    rectangle.RadiusX = 5;
                    rectangle.RadiusY = 5;
                    SolidColorBrush myBrush = new SolidColorBrush(Colors.CornflowerBlue);
                    rectangle.Fill = myBrush;
                    rectangle.Height = 19;
                    rectangle.Width = Canvas.GetLeft(textBlocks[0, textBlocks.GetUpperBound(1)]) + 15;
                    Panel.SetZIndex(rectangle, -1);
                    rectangle.Margin = new Thickness(Canvas.GetLeft(textBlocks[index1, 0]),
                        Canvas.GetTop(textBlocks[index1, 0]), 0, 0);
                    DrawingCanvas.Children.Add(rectangle);

                    Thread.Sleep(visualizationDelay);
                }
            });
        }

        private void HighlightColumnOrRow(int index, bool isColumn)
        {
            DrawingCanvas.Dispatcher.BeginInvoke(DispatcherPriority.SystemIdle, (ThreadStart)delegate ()
            {
                if (isColumn)
                {
                    for (int j = 0; j < textBlocks.GetUpperBound(0) + 1; j++)
                    {
                        textBlocks[j, index].Foreground = new SolidColorBrush(Colors.White);
                    }

                    highligntedArrayElement = new Rectangle
                    {
                        RadiusX = 5,
                        RadiusY = 5,
                        Fill = new SolidColorBrush(Colors.Red),
                        Height = Canvas.GetTop(textBlocks[textBlocks.GetUpperBound(0), index]) + 20,
                        Width = 15
                    };
                    Panel.SetZIndex(highligntedArrayElement, -1);
                    highligntedArrayElement.Margin = new Thickness(Canvas.GetLeft(textBlocks[0, index]),
                        Canvas.GetTop(textBlocks[0, index]), 0, 0);
                }
                else
                {
                    for (int j = 0; j < textBlocks.GetUpperBound(0) + 1; j++)
                    {
                        textBlocks[index, j].Foreground = new SolidColorBrush(Colors.White);
                    }

                    highligntedArrayElement = new Rectangle
                    {
                        RadiusX = 5,
                        RadiusY = 5,
                        Fill = new SolidColorBrush(Colors.Red),
                        Height = 20,
                        Width = Canvas.GetLeft(textBlocks[index, textBlocks.GetUpperBound(1)]) + 15
                    };
                    Panel.SetZIndex(highligntedArrayElement, -1);
                    highligntedArrayElement.Margin = new Thickness(Canvas.GetLeft(textBlocks[index, 0]),
                        Canvas.GetTop(textBlocks[index, 0]), 0, 0);
                }


                DrawingCanvas.Children.Add(highligntedArrayElement);
            });
        }

        private void SetComparingRowColor(int index1, int index2)
        {
            DrawingCanvas.Dispatcher.BeginInvoke(DispatcherPriority.SystemIdle, (ThreadStart)delegate ()
            {
                if (!endVisualisation)
                {
                    for (int j = 0; j < textBlocks.GetUpperBound(1) + 1; j++)
                    {
                        textBlocks[index2, j].Foreground = new SolidColorBrush(Colors.White);
                        textBlocks[index1, j].Foreground = new SolidColorBrush(Colors.White);
                    }

                    Rectangle rectangle1 = new Rectangle();
                    rectangle1.RadiusX = 5;
                    rectangle1.RadiusY = 5;
                    SolidColorBrush myBrush = new SolidColorBrush(Colors.DarkOrange);
                    rectangle1.Fill = myBrush;
                    rectangle1.Height = 19;
                    rectangle1.Width = Canvas.GetLeft(textBlocks[0, textBlocks.GetUpperBound(1)]) + 15;
                    Panel.SetZIndex(rectangle1, -1);
                    rectangle1.Margin = new Thickness(Canvas.GetLeft(textBlocks[index1, 0]),
                        Canvas.GetTop(textBlocks[index1, 0]), 0, 0);

                    Rectangle rectangle2 = new Rectangle();
                    rectangle2.RadiusX = 5;
                    rectangle2.RadiusY = 5;
                    rectangle2.Fill = myBrush;
                    rectangle2.Height = 19;
                    rectangle2.Width = Canvas.GetLeft(textBlocks[0, textBlocks.GetUpperBound(1)]) + 15;
                    Panel.SetZIndex(rectangle2, -1);
                    rectangle2.Margin = new Thickness(Canvas.GetLeft(textBlocks[index2, 0]),
                        Canvas.GetTop(textBlocks[index2, 0]), 0, 0);
                    DrawingCanvas.Children.Add(rectangle1);
                    DrawingCanvas.Children.Add(rectangle2);

                    Thread.Sleep(visualizationDelay);
                }
            });
        }

        private void Swaplines(int index1, int index2)
        {
            TextBlock temp;

            int topFirst, topSecond;
            TextBlock temp1;
            TextBlock temp2;

            DrawingCanvas.Dispatcher.BeginInvoke(DispatcherPriority.SystemIdle, (ThreadStart)delegate ()
            {
                if (!endVisualisation)
                {
                    for (int j = 0; j < textBlocks.GetUpperBound(1) + 1; j++)
                    {
                        temp2 = textBlocks[index2, j];
                        temp1 = textBlocks[index1, j];

                        topSecond = (int)Canvas.GetTop(temp2);
                        topFirst = (int)Canvas.GetTop(temp1);

                        Canvas.SetTop(temp1, topSecond);
                        Canvas.SetTop(temp2, topFirst);
                        temp = textBlocks[index1, j];
                        textBlocks[index1, j] = textBlocks[index2, j];
                        textBlocks[index2, j] = temp;
                    }

                    Thread.Sleep(visualizationDelay);
                }
            });
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void TurnOffImage_OnMouseEnter(object sender, MouseEventArgs e)
        {
            TurnOffImage.Source = new BitmapImage(
                new Uri("turn_off_with_shadow.png", UriKind.Relative));
        }

        private void TurnOffImage_OnMouseLeave(object sender, MouseEventArgs e)
        {
            TurnOffImage.Source = new BitmapImage(
                new Uri("turn_off.png", UriKind.Relative));
        }
    }
}
