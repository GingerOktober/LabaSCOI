
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using ScottPlot;
using static System.Net.Mime.MediaTypeNames;

namespace LabaSCOI
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Добро пожаловать в программу обработки изображений!");
            bool continueProcessing = true;
            while (continueProcessing) //Цикл для повторного использования обработки изображений
            {
                continueProcessing = false;
                Console.WriteLine("Выберите операцию:");
                Console.WriteLine("1. Сумма");
                Console.WriteLine("2. Произведение");
                Console.WriteLine("3. Среднее арифметическое");
                Console.WriteLine("4. Минимум");
                Console.WriteLine("5. Максимум");
                Console.WriteLine("6. Наложить маску");
                Console.WriteLine("7. Обработать изображение статической функцией и получить их гистограммы");
                Console.WriteLine("8. Бинаризация изображения");
                int choice = Convert.ToInt32(Console.ReadLine());
                Bitmap resultImage = null;
                if (choice != 7 && choice !=8)
                {
                    Console.WriteLine("Введите путь к первому изображению:");
                    string imagePath1 = Console.ReadLine();

                    Console.WriteLine("Введите путь ко второму изображению:");
                    string imagePath2 = Console.ReadLine();

                    Bitmap image1 = LoadImage(imagePath1);
                    Bitmap image2 = LoadImage(imagePath2);

                    if (image1 == null || image2 == null) //Если фото не добавлены
                    {
                        Console.WriteLine("Ошибка при загрузке изображений.");
                        continueProcessing = true;
                    }
                    else
                    {
                        switch (choice) //Вызов фунцкий в зависимости от ввода числа
                        {

                            case 1:
                                resultImage = AddImages(image1, image2);
                                break;
                            case 2:
                                resultImage = MultiplyImages(image1, image2);
                                break;
                            case 3:
                                resultImage = AverageImages(image1, image2);
                                break;
                            case 4:
                                resultImage = MinImages(image1, image2);
                                break;
                            case 5:
                                resultImage = MaxImages(image1, image2);
                                break;
                            case 6:

                                Console.WriteLine("Выберите форму маски:");
                                Console.WriteLine("1. Круг");
                                Console.WriteLine("2. Квадрат");
                                Console.WriteLine("3. Прямоугольник");
                                int maskShapeChoice = Convert.ToInt32(Console.ReadLine());
                                if (maskShapeChoice == 1 || maskShapeChoice == 2) //Если выбрана фигура квадрат или круг, то вызов функции, работающей по одной стороне фигуры
                                {

                                    Console.WriteLine("Введите радиус или сторону или ширину и высоту маски:");
                                    int radiusOrSize = Convert.ToInt32(Console.ReadLine());

                                    resultImage = ApplyMask(image1, maskShapeChoice, radiusOrSize);

                                    break;
                                }
                                else //Если выбран прямоугольник, то просьба ввести шириину и высоту и вызов соответствующей функции для двусторонней фигуры
                                {
                                    Console.WriteLine("Введите ширину маски:");
                                    int width = Convert.ToInt32(Console.ReadLine());
                                    Console.WriteLine("Введите высоту маски:");
                                    int height = Convert.ToInt32(Console.ReadLine());
                                    resultImage = ApplyMaskRectangle(image1, width, height);

                                    break;
                                }
                            default:
                                Console.WriteLine("Некорректный выбор операции.");
                                continueProcessing = true;
                                break;
                        }
                        Console.WriteLine("Введите путь для сохранения результата:");
                        string resultPath = Console.ReadLine();
                        SaveImage(resultImage, resultPath);
                    }
                }
                else if(choice == 7)
                {
                    Console.WriteLine("Введите путь к изображению:");
                    string imagePath1 = Console.ReadLine();
                    Bitmap image1 = LoadImage(imagePath1);
                    if (image1 == null)
                    {
                        Console.WriteLine("Ошибка при загрузке изображений.");
                    }
                    //Создание его гистограммы
                    int[] histogram1 = HistogramCreate(image1);
                    Console.WriteLine("Введите путь для сохранения гистограммы изначального изображения:");
                    string directoryPath1 = Console.ReadLine();
                    Plot Model1 = HistogramCreatePlotModel(histogram1, 0);
                    HistogramSave(Model1, directoryPath1, image1);
                    Console.WriteLine("Идёт обработка изображения...");
                    //Обработка изображения
                    Bitmap image2 = Function(image1);
                    //Сохранение обработанного изображения
                    Console.WriteLine("Введите путь для сохранения обработанного изображения:");
                    string processedImagePath = Console.ReadLine();
                    string directoryPathImage = SaveResultImage(image2, processedImagePath);
                    Console.WriteLine("Создаётся гистограмма нового изображения...");
                    Bitmap resImage = LoadImage(directoryPathImage);
                    
                    //Создание гистограммы обработанного изображения
                    int[] histogram2 = HistogramCreate(resImage);
                    Plot Model2 = HistogramCreatePlotModel(histogram2, 1);
                    //Сохранение гистограммы обработанного изображения
                    Console.WriteLine("Введите путь для сохранения гистограммы обработанного изображения:");
                    string directoryPath2 = Console.ReadLine();
                    HistogramSave(Model2, directoryPath2, resImage);
                    Console.WriteLine("Гистограммы и обработанное изображение сохранены");
                }
                else
                {
                    Console.WriteLine("Введите путь к изображению:");
                    string inputImagePath = Console.ReadLine();
                    Console.WriteLine("Введите путь куда сохранять изображение:");
                    string outputImagePath = Console.ReadLine();
                    Console.WriteLine("Введите имя файла с расширением:");
                    string outputImageName = Console.ReadLine();
                    outputImagePath = Path.Combine(outputImagePath, outputImageName);
                    Bitmap inputImage = new Bitmap(inputImagePath);
                    if (inputImage == null)
                    {
                        Console.WriteLine("Введён некорректный путь к изображению");
                        continueProcessing = true;
                    }
                    else
                    {
                        if (outputImagePath == null)
                        {
                            Console.WriteLine("Введён некорректный путь для сохранения файла");
                            continueProcessing = true;
                        }
                        else
                        {
                            Bitmap outputImage = GavrilovBinarization(inputImage);
                            outputImage.Save(outputImagePath);
                            Console.WriteLine("Изображение успешно обработано и сохранено.");
                        }
                    }
                }
                if (continueProcessing != true) //Если в процессе обработки не было ошибок, то продолжаем работу, иначе начинаем всё сначала
                {
                    Console.WriteLine("Результат сохранен.");

                    Console.WriteLine("Хотите выполнить еще одну операцию? (да/нет)");
                    string answer = Console.ReadLine();
                    if (answer.ToLower() == "да")
                    {
                        continueProcessing = true;
                    }
                    else
                    {
                        Console.WriteLine("Все файлы сохранены. Программа завершена.");
                    }
                }
            }
            Console.ReadLine();
        }
        static Bitmap GavrilovBinarization(Bitmap inputImage)
        {
            int width = inputImage.Width;
            int height = inputImage.Height;
            int sum = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    System.Drawing.Color pixel = inputImage.GetPixel(x, y);
                    int brightness = (int)(0.3 * pixel.R + 0.59 * pixel.G + 0.11 * pixel.B); //среднее значение каналов - яркость пикселей
                    sum += brightness;// Cумма всех пикселей
                }
            }
            int threshold = sum / (width * height);// Вычисляем порог из суммы пикселей
            Bitmap outputImage = new Bitmap(width, height);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    System.Drawing.Color pixel = inputImage.GetPixel(x, y);
                    int brightness = (int)(0.3 * pixel.R + 0.59 * pixel.G + 0.11 * pixel.B);
                    // Проверяем, превышает ли яркость порог
                    if (brightness > threshold)
                    {
                        outputImage.SetPixel(x, y, System.Drawing.Color.White); //максимальное значение - белый
                    }
                    else
                    {
                        outputImage.SetPixel(x, y, System.Drawing.Color.Black); //минимальное значение - черный
                    }
                }
            }

            return outputImage;
        }
        static int[] HistogramCreate(Bitmap image)
        {
            try
            {
                int[] histogram = new int[256];
                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        System.Drawing.Color pixelColor = image.GetPixel(x, y);
                        int grayValue = (int)(pixelColor.R * 0.3 + pixelColor.G * 0.59 + pixelColor.B * 0.11); // Преобразование в оттенки серого
                        histogram[grayValue]++;
                    }
                }
                return histogram;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                Console.WriteLine($"image = {image}");
                return null;
            }
            
        }
        static Bitmap LoadImage(string path) //Фунцкия загрузки изображения по переданному пути
        {
            try
            {

                return new Bitmap(path);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке изображения: {ex.Message}"); 
                return null;
            }
        }
        static void HistogramSave(Plot histogramModel, string directoryPath, Bitmap image)//Сохранение гистограммы
        {
            Console.WriteLine("Введите имя файла с расширением:");
            string fileName=Console.ReadLine();
            try
            {
                string filePath = Path.Combine(directoryPath, fileName);
                histogramModel.SavePng(filePath, image.Width, image.Height);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        static Bitmap Function(Bitmap image) //Функция, вызывающая обработку изображения и нормализирующая полученный результат
        {
            int width = image.Width;
            int height = image.Height;
            Bitmap image2 = new Bitmap(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    System.Drawing.Color pixelColor = image.GetPixel(x, y);
                    // Нормализация
                    double r = pixelColor.R / 255.0; 
                    double g = pixelColor.G / 255.0;
                    double b = pixelColor.B / 255.0;
                    // Применение функции Func
                    double newR = Func(r); 
                    double newG = Func(g);
                    double newB = Func(b);
                    // Округление до целого числа
                    int newRInt = (int)Math.Round(newR); 
                    int newGInt = (int)Math.Round(newG);
                    int newBInt = (int)Math.Round(newB);
                    // Ограничение от -255 до 255
                    newRInt = Math.Min(newRInt, 255); 
                    newGInt = Math.Min(newGInt, 255);
                    newBInt = Math.Min(newBInt, 255);

                    System.Drawing.Color newColor = System.Drawing.Color.FromArgb(newRInt, newGInt, newBInt); // Создание цвета
                    image2.SetPixel(x, y, newColor);
                }
            }
            return image2;
        }
        static double Func(double x) //Тут статически вбивается функция, которая будет обрабатывать цвет, в данном случае - инверсия
        {
            x = 1.0 - x;
            return 255.0 * x;
        }
        static Plot HistogramCreatePlotModel(int[] histogram, int val) //Рисуем график гистограммы
        {
            var plt = new Plot();
            string text = null;
            if (val == 0)
            {
                text = "Гистограмма исходного изображения";
            }
            else
            {
                text = "Гистограмма обработанного изображения";
            }
            double[] histogramData = histogram.Select(h => (double)h).ToArray();

            plt.Add.Scatter(Enumerable.Range(0, histogram.Length).Select(i => (double)i).ToArray(), histogramData);

            plt.XLabel("Интенсивность");
            plt.YLabel("Пиксели");
            plt.Title($"{text}");
            return plt;
        }
        static void SaveImage(Bitmap image, string path) //Функция сохранения изображения по переданному пути
        {
            try
            {
                using (var stream = new FileStream(path, FileMode.CreateNew))
                {
                    image.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);

                }
            }
            catch (IOException)
            {
                Console.WriteLine("Введите имя файла для сохранения (с расширением):");
                string fileNameWithExtension = Console.ReadLine();
                string newPath = Path.Combine(Path.GetDirectoryName(path), fileNameWithExtension);
                SaveImage(image, newPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении изображения: {ex.Message}");
            }

            finally
            {
                image.Dispose();
            }
        }
        static string SaveResultImage(Bitmap image, string path)
        {
            try
            {
                Console.WriteLine("Введите имя файла для сохранения (с расширением):");
                string fileName = Console.ReadLine();
                string newPath = Path.Combine(Path.GetDirectoryName(path), fileName);
                using (var stream = new FileStream(newPath, FileMode.CreateNew))
                {
                    image.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                    return newPath;
                }
            }
            catch (IOException)
            {
                return SaveResultImage(image, path);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении изображения: {ex.Message}");
            }
            finally
            {
                image.Dispose();
            }
            return null;
        }

        static Bitmap AddImages(Bitmap image1, Bitmap image2)// Функция суммирования изображений
        {
            // Проверяем, что размеры изображений совпадают
            if (image1.Width != image2.Width || image1.Height != image2.Height)
            {
                Console.WriteLine("Размеры изображений не совпадают.");
                return null;
            }

            // Создаем новое изображение для результата с теми же размерами, что и исходные изображения
            Bitmap resultImage = new Bitmap(image1.Width, image1.Height);

            // Проходим по каждому пикселю изображений и складываем их значения
            for (int x = 0; x < image1.Width; x++)
            {
                for (int y = 0; y < image1.Height; y++)
                {
                    System.Drawing.Color pixel1 = image1.GetPixel(x, y);
                    System.Drawing.Color pixel2 = image2.GetPixel(x, y);

                    // Вычисляем новое значение каждого канала цвета
                    int red = Math.Min(255, pixel1.R + pixel2.R);
                    int green = Math.Min(255, pixel1.G + pixel2.G);
                    int blue = Math.Min(255, pixel1.B + pixel2.B);

                    // Устанавливаем новый пиксель на изображение результата
                    resultImage.SetPixel(x, y, System.Drawing.Color.FromArgb(red, green, blue));
                }
            }

            return resultImage;
        }

        
        static Bitmap MultiplyImages(Bitmap image1, Bitmap image2)// Функция произведения изображений
        {
            // Проверяем, что размеры изображений совпадают
            if (image1.Width != image2.Width || image1.Height != image2.Height)
            {
                Console.WriteLine("Размеры изображений не совпадают.");
                return null;
            }

            // Создаем новое изображение для результата с теми же размерами, что и исходные изображения
            Bitmap resultImage = new Bitmap(image1.Width, image1.Height);

            // Проходим по каждому пикселю изображений и умножаем их значения
            for (int x = 0; x < image1.Width; x++)
            {
                for (int y = 0; y < image1.Height; y++)
                {
                    System.Drawing.Color pixel1 = image1.GetPixel(x, y);
                    System.Drawing.Color pixel2 = image2.GetPixel(x, y);

                    // Вычисляем новое значение каждого канала цвета
                    int red = pixel1.R * pixel2.R / 255;
                    int green = pixel1.G * pixel2.G / 255;
                    int blue = pixel1.B * pixel2.B / 255;

                    // Устанавливаем новый пиксель на изображение результата
                    resultImage.SetPixel(x, y, System.Drawing.Color.FromArgb(red, green, blue));
                }
            }
            return resultImage;
        }
        
        static Bitmap AverageImages(Bitmap image1, Bitmap image2)// Функция среднего значения
        {
            if (image1.Width != image2.Width || image1.Height != image2.Height)
            {
                Console.WriteLine("Размеры изображений не совпадают.");
                return null;
            }

            Bitmap resultImage = new Bitmap(image1.Width, image1.Height);

            // Проходим по каждому пикселю изображений и находим среднее значение каждого канала цвета
            for (int x = 0; x < image1.Width; x++)
            {
                for (int y = 0; y < image1.Height; y++)
                {
                    System.Drawing.Color pixel1 = image1.GetPixel(x, y);
                    System.Drawing.Color pixel2 = image2.GetPixel(x, y);

                    int red = (pixel1.R + pixel2.R) / 2;
                    int green = (pixel1.G + pixel2.G) / 2;
                    int blue = (pixel1.B + pixel2.B) / 2;

                    // Устанавливаем новый пиксель на изображение результата
                    resultImage.SetPixel(x, y, System.Drawing.Color.FromArgb(red, green, blue));
                }
            }
            return resultImage;
        }

        
        static Bitmap MinImages(Bitmap image1, Bitmap image2)// Функция минимума
        {
            if (image1.Width != image2.Width || image1.Height != image2.Height)
            {
                Console.WriteLine("Размеры изображений не совпадают.");
                return null;
            }

            Bitmap resultImage = new Bitmap(image1.Width, image1.Height);

            // Проходим по каждому пикселю изображений и находим минимальное значение каждого канала цвета
            for (int x = 0; x < image1.Width; x++)
            {
                for (int y = 0; y < image1.Height; y++)
                {
                    System.Drawing.Color pixel1 = image1.GetPixel(x, y);
                    System.Drawing.Color pixel2 = image2.GetPixel(x, y);

                    int red = Math.Min(pixel1.R, pixel2.R);
                    int green = Math.Min(pixel1.G, pixel2.G);
                    int blue = Math.Min(pixel1.B, pixel2.B);

                    // Устанавливаем новый пиксель на изображение результата
                    resultImage.SetPixel(x, y, System.Drawing.Color.FromArgb(red, green, blue));
                }
            }
            return resultImage;
        }

        
        static Bitmap MaxImages(Bitmap image1, Bitmap image2)// Функция максимума
        {
            if (image1.Width != image2.Width || image1.Height != image2.Height)
            {
                Console.WriteLine("Размеры изображений не совпадают.");
                return null;
            }

            Bitmap resultImage = new Bitmap(image1.Width, image1.Height);

            // Проходим по каждому пикселю изображений и находим максимальное значение каждого канала цвета
            for (int x = 0; x < image1.Width; x++)
            {
                for (int y = 0; y < image1.Height; y++)
                {
                    System.Drawing.Color pixel1 = image1.GetPixel(x, y);
                    System.Drawing.Color pixel2 = image2.GetPixel(x, y);

                    int red = Math.Max(pixel1.R, pixel2.R);
                    int green = Math.Max(pixel1.G, pixel2.G);
                    int blue = Math.Max(pixel1.B, pixel2.B);

                    // Устанавливаем новый пиксель на изображение результата
                    resultImage.SetPixel(x, y, System.Drawing.Color.FromArgb(red, green, blue));
                }
            }
            return resultImage;
        }

        
        static Bitmap ApplyMaskRectangle(Bitmap image, int width, int height)// Функция добавления прямоугольной маски
        {
            Bitmap resultImage = new Bitmap(image.Width, image.Height);
            using (Graphics g = Graphics.FromImage(resultImage))
            {
                // Накладываем маску на изображение
                g.DrawImage(image, 0, 0);
                System.Drawing.Color maskColor = System.Drawing.Color.Black;
                g.FillRectangle(new SolidBrush(maskColor), new Rectangle(image.Width / 2 - width / 2, image.Height / 2 - height / 2, width, height));

            }
            return resultImage;
        }
        static Bitmap ApplyMask(Bitmap image, int shapeChoice, int size) //Функция добавления квадратной или круглой маски
        {
            // Создаем новое изображение для результата с теми же размерами, что и исходное изображение
            Bitmap resultImage = new Bitmap(image.Width, image.Height);

            // Создаем объект Graphics для рисования на изображении результата
            Graphics g = Graphics.FromImage(resultImage);

            // Рисуем исходное изображение на результате
            g.DrawImage(image, 0, 0);

            // Задаем цвет маски - чёрный
            System.Drawing.Color maskColor = System.Drawing.Color.Black;

            // В зависимости от выбора формы маски (1 - круг, 2 - квадрат), накладываем маску на изображение
            if (shapeChoice == 1) // Если выбран круг
            {
                g.FillEllipse(new SolidBrush(maskColor), new Rectangle(image.Width / 2 - size / 2, image.Height / 2 - size / 2, size, size));
            }
            else if (shapeChoice == 2) // Если выбран квадрат
            {
                g.FillRectangle(new SolidBrush(maskColor), new Rectangle(image.Width / 2 - size / 2, image.Height / 2 - size / 2, size, size));
            }
            return resultImage;
        }


    }
}
