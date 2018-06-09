using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Monte_Carlo
{
    class Program
    {
        static Stack<double> areaList = new Stack<double>();
        static List<EventWaitHandle> handles = new List<EventWaitHandle>();
        static double a, b, c, d;
        static int x1, x2;
        static int y1 = Int32.MaxValue, y2 = Int32.MinValue;
        static int balance, part;

        static void Main(string[] args)
        {

            FuncForm myForm = new FuncForm();
            Chart myChart = myForm.chart1;
            
            Console.WriteLine("Нахождение площади функций вида y = ax^3 + bx^2 + cx + d методом Монте-Карло.");
            Console.Write("Введите а: "); if (!Double.TryParse(Console.ReadLine(), out a)) a = 0;
            Console.Write("Введите b: "); if (!Double.TryParse(Console.ReadLine(), out b)) b = 0;
            Console.Write("Введите c: "); if (!Double.TryParse(Console.ReadLine(), out c)) c = 0;
            Console.Write("Введите d: "); if (!Double.TryParse(Console.ReadLine(), out d)) d = 0;

            if (a == b && b == c && c == d && d == 0)
            {
                Console.WriteLine("Нечего искать. Нажмите любую кнопку, чтобы выйти.");
                Console.ReadKey();
                return;
            }
            Console.WriteLine("На каком отрезке искать площадь?");
            Console.Write("X1: "); if (!Int32.TryParse(Console.ReadLine(), out x1)) x1 = new Random().Next(-10, 5);
            Console.Write("X2: "); if (!Int32.TryParse(Console.ReadLine(), out x2) || x2 < x1) x2 = new Random().Next(x1, x1 + 5);

            int inter;
            for (int i = x1; i <= x2; i++)           // Находим верхнюю и нижнюю границы
            {
                inter = (int)getFuncPoint(i);
                myChart.Series[0].Points.AddXY(i, inter);
                if (y1 > inter)
                    y1 = inter;
                else if (y2 < inter)
                    y2 = inter;
            }
            if (y1 > 0)
                y1 = 0;
            if (y2 < 0)
                y2 = 0;
            
            Console.Write("Сколько раз смоделируется расчет (чем больше, тем точнее): ");
            int threadCount;
            if (!int.TryParse(Console.ReadLine(), out threadCount) || threadCount < 1)
            {
                threadCount = new Random().Next(1, 3);
                Console.WriteLine("Принятое число расчетов: " + threadCount);
            }
            else if (threadCount > 64)              // Делим задачи между потоками
            {
                part = threadCount / 64;
                balance = threadCount % 64;
                threadCount = 64;
            }

            Console.Write("Сколько точек: ");
            int pointNumber;
            if (!int.TryParse(Console.ReadLine(), out pointNumber) || pointNumber < 1)
            {
                pointNumber = new Random().Next(15000, 45000);
                Console.WriteLine("Принятое число точек: " + pointNumber);
            }
            

            for (int i = 0; i < threadCount; i++) 
            {
                EventWaitHandle handle = new AutoResetEvent(false);
                new Thread(delegate () {MonteCarloMethod(pointNumber, handle, (i < balance ? i + 1 : i)); }).Start();
                handles.Add(handle);
            }
            WaitHandle.WaitAll(handles.ToArray());

            myChart.Series[0].BorderWidth = 3;
            myChart.Series.Add("Bounds");
            myChart.Series[1].Points.AddXY(x2, 0);
            myChart.Series[1].Points.AddXY(x1, 0);
            myChart.Series.Add("Area = " + areaList.Average()).Color = System.Drawing.Color.DarkBlue;
            
            Application.Run(myForm);

            Console.WriteLine("Нажмите любую кнопку, чтобы выйти.");
            Console.ReadLine();
        }


        static void MonteCarloMethod(int pointNumber, object handle, int numOfCount)
        {
            Random r = new Random();
            AutoResetEvent wh = (AutoResetEvent)handle;

            int points = 0;
            double[] subAns;
            subAns = new double[part + 1];

            for (int wave = 0; wave <= part; wave++)
            {
                points = 0;

                for (int i = 0; i < pointNumber; i++)
                {
                    if (IsFunc((double)r.Next(x1, x2) + r.NextDouble(), (double)r.Next(y1, y2) + r.NextDouble()))
                    {
                        points++;
                    }
                }
                subAns[wave] = (x2 - x1) * (y2 - y1) * ((points) / (double)pointNumber);
            }
            areaList.Push(subAns.Average());
            
            wh.Set();
        }

        static bool IsFunc(double x, double y)
        {
            double funcPoint = getFuncPoint(x);
            if (funcPoint >= 0)
            {
                return funcPoint >= y && y >= 0;
            }
            else
            {
                return funcPoint < y && y <= 0;
            }
        }

        static double getFuncPoint(double x)
        {
            return a * x * x * x + b * x * x + c * x + d;
        }
    }

}

