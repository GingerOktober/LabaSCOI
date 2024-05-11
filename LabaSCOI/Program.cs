
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace LabaSCOI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Добро пожаловать в программу обработки изображений!");
            bool continueProcessing = true;
            while (continueProcessing) //Цикл для повторного использования обработки изображений
            {
                continueProcessing = false;
                Console.WriteLine("Введите путь к первому изображению:");
                string imagePath1 =

                Console.ReadLine();

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
                    Console.WriteLine("Выберите операцию:");
                    Console.WriteLine("1. Сумма");
                    Console.WriteLine("2. Произведение");
                    Console.WriteLine("3. Среднее арифметическое");
                    Console.WriteLine("4. Минимум");
                    Console.WriteLine("5. Максимум");
                    Console.WriteLine("6. Наложить маску");

                    int choice = Convert.ToInt32(Console.ReadLine());

                    Bitmap resultImage = null;

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
                            if(maskShapeChoice == 1 || maskShapeChoice == 2) //Если выбрана фигура квадрат или круг, то вызов функции, работающей по одной стороне фигуры
                            {

                                Console.WriteLine("Введите радиус или сторону или ширину и высоту маски:");
                                int radiusOrSize = Convert.ToInt32(Console.ReadLine());

                                resultImage = ApplyMask(image1, image2, maskShapeChoice, radiusOrSize);

                                break;
                            }
                            else //Если выбран прямоугольник, то просьба ввести шириину и высоту и вызов соответствующей функции для двусторонней фигуры
                            {
                                Console.WriteLine("Введите ширину маски:");
                                int width = Convert.ToInt32(Console.ReadLine());
                                Console.WriteLine("Введите высоту маски:");
                                int height = Convert.ToInt32(Console.ReadLine());
                                resultImage = ApplyMaskRectangle(image1, image2, width, height);

                                break;
                            }
                        default:
                            Console.WriteLine("Некорректный выбор операции.");
                            continueProcessing = true;
                            break;
                    }
                    if (continueProcessing != true) //Если в процессе обработки не было ошибок, то продолжаем работу, иначе начинаем всё сначала
                    {
                        Console.WriteLine("Введите путь для сохранения результата:");
                        string resultPath = Console.ReadLine();

                        SaveImage(resultImage, resultPath);

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

        static void SaveImage(Bitmap image, string path) //Функция сохранения изображения по переданному пути
        {
            try
            {
                using (var stream = new FileStream(path, FileMode.CreateNew))
                {
                    image.Save(stream, ImageFormat.Jpeg);

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
                    Color pixel1 = image1.GetPixel(x, y);
                    Color pixel2 = image2.GetPixel(x, y);

                    // Вычисляем новое значение каждого канала цвета
                    int red = Math.Min(255, pixel1.R + pixel2.R);
                    int green = Math.Min(255, pixel1.G + pixel2.G);
                    int blue = Math.Min(255, pixel1.B + pixel2.B);

                    // Устанавливаем новый пиксель на изображение результата
                    resultImage.SetPixel(x, y, Color.FromArgb(red, green, blue));
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
                    Color pixel1 = image1.GetPixel(x, y);
                    Color pixel2 = image2.GetPixel(x, y);

                    // Вычисляем новое значение каждого канала цвета
                    int red = pixel1.R * pixel2.R / 255;
                    int green = pixel1.G * pixel2.G / 255;
                    int blue = pixel1.B * pixel2.B / 255;

                    // Устанавливаем новый пиксель на изображение результата
                    resultImage.SetPixel(x, y, Color.FromArgb(red, green, blue));
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
                    Color pixel1 = image1.GetPixel(x, y);
                    Color pixel2 = image2.GetPixel(x, y);

                    int red = (pixel1.R + pixel2.R) / 2;
                    int green = (pixel1.G + pixel2.G) / 2;
                    int blue = (pixel1.B + pixel2.B) / 2;

                    // Устанавливаем новый пиксель на изображение результата
                    resultImage.SetPixel(x, y, Color.FromArgb(red, green, blue));
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
                    Color pixel1 = image1.GetPixel(x, y);
                    Color pixel2 = image2.GetPixel(x, y);

                    int red = Math.Min(pixel1.R, pixel2.R);
                    int green = Math.Min(pixel1.G, pixel2.G);
                    int blue = Math.Min(pixel1.B, pixel2.B);

                    // Устанавливаем новый пиксель на изображение результата
                    resultImage.SetPixel(x, y, Color.FromArgb(red, green, blue));
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
                    Color pixel1 = image1.GetPixel(x, y);
                    Color pixel2 = image2.GetPixel(x, y);

                    int red = Math.Max(pixel1.R, pixel2.R);
                    int green = Math.Max(pixel1.G, pixel2.G);
                    int blue = Math.Max(pixel1.B, pixel2.B);

                    // Устанавливаем новый пиксель на изображение результата
                    resultImage.SetPixel(x, y, Color.FromArgb(red, green, blue));
                }
            }
            return resultImage;
        }

        
        static Bitmap ApplyMaskRectangle(Bitmap image, Bitmap mask, int width, int height)// Функция добавления прямоугольной маски
        {
            Bitmap resultImage = new Bitmap(image.Width, image.Height);
            using (Graphics g = Graphics.FromImage(resultImage))
            {
                // Накладываем маску на изображение
                g.DrawImage(image, 0, 0);
                Color maskColor = Color.Black;
                g.FillRectangle(new SolidBrush(maskColor), new Rectangle(image.Width / 2 - width / 2, image.Height / 2 - height / 2, width, height));

                // Проходим по каждому пикселю изображений и применяем маску
                for (int x = 0; x < image.Width; x++)
                {
                    for (int y = 0; y < image.Height; y++)
                    {
                        Color maskPixel = mask.GetPixel(x, y);
                    }
                }
            }
            return resultImage;
        }
        static Bitmap ApplyMask(Bitmap image, Bitmap mask, int shapeChoice, int size) //Функция добавления квадратной или круглой маски
        {
            // Создаем новое изображение для результата с теми же размерами, что и исходное изображение
            Bitmap resultImage = new Bitmap(image.Width, image.Height);

            // Создаем объект Graphics для рисования на изображении результата
            Graphics g = Graphics.FromImage(resultImage);

            // Рисуем исходное изображение на результате
            g.DrawImage(image, 0, 0);

            // Задаем цвет маски - чёрный
            Color maskColor = Color.Black;

            // В зависимости от выбора формы маски (1 - круг, 2 - квадрат), накладываем маску на изображение
            if (shapeChoice == 1) // Если выбран круг
            {
                g.FillEllipse(new SolidBrush(maskColor), new Rectangle(image.Width / 2 - size / 2, image.Height / 2 - size / 2, size, size));
            }
            else if (shapeChoice == 2) // Если выбран квадрат
            {
                g.FillRectangle(new SolidBrush(maskColor), new Rectangle(image.Width / 2 - size / 2, image.Height / 2 - size / 2, size, size));
            }

            // Проходим по каждому пикселю изображений и применяем маску
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    Color pixel = image.GetPixel(x, y);
                    Color maskPixel = mask.GetPixel(x, y);
                }
            }
            return resultImage;
        }


    }
}