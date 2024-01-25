using System;
using System.Linq;

namespace Program
{
    class Program
    {
        private static void Main(string[] args)
        {
            var cpp = new Reference(1, 0, 0, 1, 1, "C++");
            var pascal = new Reference(0, 1, 0, 1, 0, "Pascal");
            var php = new Reference(0, 1, 1, 0, 0, "PHP");
            var js = new Reference(1, 1, 1, 0, 0, "JS");
            var assembler = new Reference(0, 0, 0, 1, 1, "Assembler");
            var references = new Reference[] { cpp, pascal, php, js, assembler };
            double lambda = -1;

            Console.WriteLine("Введите значение лямбды, для нахождения расстояния по Минковскому");
            while (lambda < 0)
            {
                try
                {
                    lambda = double.Parse(Console.ReadLine());
                }
                catch
                {
                    Console.WriteLine("Введите вещественное число, используя запятую");
                }
            }

            Console.WriteLine("Введите названия языка для сравнения, имеющиеся эталоны C++, Pascal, PHP, JS, Assembler");
            string languageName = Console.ReadLine();
            Reference language = null;
            while (language == null)
            {
                try
                {
                    int[] values = new int[5];
                    for (int i = 1; i <= 5; i++)
                    {
                        Console.WriteLine($"Введите значение для критерия номер {i} (0 или 1)");
                        int value = int.Parse(Console.ReadLine());
                        values[i - 1] = value;
                        if (value != 0 && value != 1)
                        {
                            throw new Exception();
                        }
                    }
                    language = new Reference(values[0], values[1], values[2], values[3], values[4], languageName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Введите либо 0, либо 1");
                }
            }

            foreach (var reference in references)
            {
                Console.Write($"{reference.LanguageName}\t");
            }
            Console.WriteLine();

            // Вложенные циклы для перебора всех возможных пар языков
            foreach (var distanceFunction in typeof(Distance).GetMethods().Where(m => m.Name.StartsWith("Calculate")))
            {
                Console.WriteLine($"{GetDescription(distanceFunction)}\t");
                foreach (var language1 in references)
                {
                    object[] parameters;
                    if (distanceFunction.Name == "CalculateMinkowskiDistance")
                    {
                        parameters = new object[] { language1, language, lambda }; // Здесь 2.0 - значение параметра p
                    }
                    else
                    {
                        parameters = new object[] { language1, language };
                    }

                    var result = distanceFunction.Invoke(null, parameters);
                    Console.Write($"{result:F2}\t");
                }
                Console.WriteLine();
            }

            foreach (var similarityFunction in typeof(Similarities).GetMethods().Where(m => m.Name.StartsWith("Calculate")))
            {
                Console.WriteLine($"{GetDescription(similarityFunction)}\t");
                foreach (var language1 in references)
                {
                    var result = similarityFunction.Invoke(null, new object[] { language1, language });
                    Console.Write($"{result:F2}\t");
                }
                Console.WriteLine();
            }
        }

        private static string GetDescription(System.Reflection.MethodInfo method)
        {
            var attribute = (System.ComponentModel.DescriptionAttribute)Attribute.GetCustomAttribute(method, typeof(System.ComponentModel.DescriptionAttribute));
            return attribute == null ? method.Name : attribute.Description;
        }
    }

    public class Reference
    {
        public double[] Criterias { get; set; }
        public string LanguageName { get; set; }

        public Reference(int OOP, int AMM, int WebCentric, int Compilability, int SP, string languageName)
        {
            Criterias = new double[] { OOP, AMM, WebCentric, Compilability, SP };
            this.LanguageName = languageName;
        }
    }

    public static class Distance
    {
        //Расстояние по Минковскому
        [System.ComponentModel.Description("Расстояние по Минковскому")]
        public static double CalculateMinkowskiDistance(Reference ref1, Reference ref2, double p)
        {
            double sum = 0;
            for (int i = 0; i < 5; i++)
            {
                sum += Math.Pow(Math.Abs(ref1.Criterias[i] - ref2.Criterias[i]), p);
            }
            return Math.Pow(sum, 1 / p);
        }
        // Расстояние по Евклиду
        [System.ComponentModel.Description("Расстояние по Евклиду")]
        public static double CalculateEuclideanDistance(Reference ref1, Reference ref2)
        {
            double sum = 0;
            for (int i = 0; i < 5; i++)
            {
                sum += Math.Pow(ref1.Criterias[i] - ref2.Criterias[i], 2);
            }
            return Math.Sqrt(sum);
        }
        // Манхэттенское расстояние (сумма модулей разностей)
        [System.ComponentModel.Description("Манхэттенскому расстояние")]
        public static double CalculateManhattanDistance(Reference ref1, Reference ref2)
        {
            double sum = 0;
            for (int i = 0; i < 5; i++)
            {
                sum += Math.Abs(ref1.Criterias[i] - ref2.Criterias[i]);
            }
            return sum;
        }
    }

    public static class Similarities
    {
        // Формула Рассела и Рао
        [System.ComponentModel.Description("Сходство по Расселу и Рао")]
        public static double CalculateRussellRao(Reference ref1, Reference ref2)
        {
            int intersection = 0;
            int union = 0;

            for (int i = 0; i < 5; i++)
            {
                if (ref1.Criterias[i] == 1 && ref2.Criterias[i] == 1)
                {
                    intersection++;
                }
                union += (int)ref1.Criterias[i] + (int)ref2.Criterias[i];
            }

            return (double)intersection / union;
        }

        // Функция сходства Жокара и Нидмена
        [System.ComponentModel.Description("Сходство по Жокару и Нидмену")]
        public static double CalculateJaccardSimilarity(Reference ref1, Reference ref2)
        {
            int intersection = 0;
            int union = 0;

            for (int i = 0; i < 5; i++)
            {
                if (ref1.Criterias[i] == 1 && ref2.Criterias[i] == 1)
                {
                    intersection++;
                }
                if (ref1.Criterias[i] == 1 || ref2.Criterias[i] == 1)
                {
                    union++;
                }
            }

            return (double)intersection / union;
        }

        // Функция сходства Дайса
        [System.ComponentModel.Description("Сходство по Дайсу")]
        public static double CalculateDiceSimilarity(Reference ref1, Reference ref2)
        {
            int intersection = 0;

            for (int i = 0; i < 5; i++)
            {
                if (ref1.Criterias[i] == 1 && ref2.Criterias[i] == 1)
                {
                    intersection++;
                }
            }

            int total = (int)ref1.Criterias.Sum() + (int)ref2.Criterias.Sum();
            return (2.0 * intersection) / total;
        }

        // Функция сходства Сокаля и Снифа
        [System.ComponentModel.Description("Сходство по Сокалю и Снифа")]
        public static double CalculateSokalAndSneathSimilarity(Reference ref1, Reference ref2)
        {
            int matchingPairs = 0;
            int nonMatchingPairs = 0;

            for (int i = 0; i < 5; i++)
            {
                if ((ref1.Criterias[i] == 1 && ref2.Criterias[i] == 1) || (ref1.Criterias[i] == 0 && ref2.Criterias[i] == 0))
                {
                    matchingPairs++;
                }
                else
                {
                    nonMatchingPairs++;
                }
            }

            return (double)matchingPairs / (matchingPairs + 2 * nonMatchingPairs);
        }

        // Функция сходства Сокаля и Миншера
        [System.ComponentModel.Description("Сходство по Сокалю и Миншеры")]
        public static double CalculateSokalAndMichenerSimilarity(Reference ref1, Reference ref2)
        {
            int matchingPairs = 0;
            int nonMatchingPairs = 0;

            for (int i = 0; i < 5; i++)
            {
                if ((ref1.Criterias[i] == 1 && ref2.Criterias[i] == 1) || (ref1.Criterias[i] == 0 && ref2.Criterias[i] == 0))
                {
                    matchingPairs++;
                }
                else
                {
                    nonMatchingPairs++;
                }
            }

            return (double)matchingPairs / (matchingPairs + nonMatchingPairs);
        }

        // Функция сходства Кульжинского
        [System.ComponentModel.Description("Сходство по Кульжинскому")]
        public static double CalculateKulczynskiSimilarity(Reference ref1, Reference ref2)
        {
            int matchingPairs = 0;

            for (int i = 0; i < 5; i++)
            {
                if (ref1.Criterias[i] == 1 && ref2.Criterias[i] == 1)
                {
                    matchingPairs++;
                }
            }

            int total = (int)ref1.Criterias.Sum() + (int)ref2.Criterias.Sum();
            return (double)matchingPairs / (total / 2);
        }

        // Функция сходства Юла
        [System.ComponentModel.Description("Cходство по Юле")]
        public static double CalculateYuleSimilarity(Reference ref1, Reference ref2)
        {
            int matchingPairs = 0;
            int nonMatchingPairs = 0;

            for (int i = 0; i < 5; i++)
            {
                if ((ref1.Criterias[i] == 1 && ref2.Criterias[i] == 1) || (ref1.Criterias[i] == 0 && ref2.Criterias[i] == 0))
                {
                    matchingPairs++;
                }
                else
                {
                    nonMatchingPairs++;
                }
            }

            return (2.0 * matchingPairs * nonMatchingPairs) / ((matchingPairs + nonMatchingPairs) * (matchingPairs + nonMatchingPairs));
        }
    }
}
