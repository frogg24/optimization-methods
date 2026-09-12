
using org.mariuszgromada.math.mxparser;
using System.Drawing;
using System.Runtime.InteropServices;

namespace optimization.Core
{
    public class Optimizer
    {
        // возврат в формате (значение,точка)
        public (double, double, int) CalcGoldenRatio(string func, double a, double b, double eps)
        {
            if (string.IsNullOrWhiteSpace(func))
            {
                throw new ArgumentNullException("Функция не была введена");
            }
            if (!double.IsFinite(a) || !double.IsFinite(b))
            {
                throw new ArgumentException("Границы интервала должны быть конечными числами");
            }
            if (a >= b)
            {
                throw new ArgumentException("Интервал введен некорректно");
            }
            if(eps<= 0)
            {
                throw new ArgumentException("Погрешность должна быть положительной");
            }
            var x = new Argument("x", a);
            var exp = new Expression(func, x);

            double Calculate(double point)
            {
                x.setArgumentValue(point);

                double value = exp.calculate();

                if (!double.IsFinite(value))
                {
                    throw new ArgumentException($"Функция не определена в точке x = {point}");
                }

                return value;
            }

            if (!exp.checkSyntax())
            {
                throw new ArgumentException("Ошибка в формуле: " + exp.getErrorMessage());
            }

            double tau = (Math.Sqrt(5)-1)/2;

            double x1 = a + (1-tau)*(b-a);
            double x2 = a + tau*(b-a);

            double f1 = Calculate(x1);
            double f2 = Calculate(x2);

            int iterations = 0;
            while (b-a > eps)
            {
                iterations++;
                if(f1 < f2)
                {
                    b = x2;

                    x2 = x1;
                    f2=f1;

                    x1 = a + (1-tau)*(b-a);
                    f1 = Calculate(x1);
                }
                else
                {
                    a = x1;

                    x1 = x2;
                    f1=f2;

                    x2 = a + tau*(b-a);
                    f2 = Calculate(x2);
                }
            }

            double xMin = (a + b) / 2.0;
            double fMin = Calculate(xMin);

            return (fMin, xMin, iterations);
        }

        public (List<double> resX, double resFx) CalcHookeJeeves(string func, List<double> X0, double step, double stepRed, double eps)
        {
            var arguments = new List<Argument>();

            for (int i = 0; i < X0.Count; i++)
            {
                arguments.Add(new Argument($"x{i + 1}", 0));
            }
            var exp = new Expression(func, arguments.ToArray());

            double Calculate(List<double> point)
            {
                for (int i = 0; i < point.Count; i++)
                {
                    arguments[i].setArgumentValue(point[i]);
                }

                return exp.calculate();
            }

            var baseX = new List<double>(X0);
            double baseValue = Calculate(baseX);
            double curStep = step;

            int iterations = 0;
            const int maxIterations = 10000;

            while (curStep > eps)
            {
                iterations++;

                if (iterations > maxIterations)
                {
                    throw new InvalidOperationException("Превышено максимальное количество итераций. Возможно, функция не имеет экстремума.");
                }

                var (exploredX, exploredValue) = Explore(baseX, curStep, Calculate);
                if (exploredValue >= baseValue)
                {
                    curStep *= stepRed;
                    continue;
                }

                // Запоминаем старую базовую точку
                var oldBaseX = new List<double>(baseX);

                // Новая лучшая точка становится базовой
                baseX = exploredX;
                baseValue = exploredValue;

                // Движение по образцу
                var patternX = new List<double>();
                for (int i = 0; i < baseX.Count; i++)
                {
                    patternX.Add(baseX[i] + (baseX[i] - oldBaseX[i]));
                }
                var (patternResultX, patternResultValue) = Explore(patternX, curStep, Calculate);

                if (patternResultValue < baseValue)
                {
                    baseX = patternResultX;
                    baseValue = patternResultValue;
                }
            }

            return (baseX, baseValue);
        }

        private (List<double> point, double value) Explore( List<double> startPoint, double step, Func<List<double>, double> calculate)
        {
            var currentX = new List<double>(startPoint);
            double bestValue = calculate(currentX);

            for (int nArg = 0; nArg < currentX.Count; nArg++)
            {
                var plusPoint = new List<double>(currentX);
                plusPoint[nArg] += step;

                double value = calculate(plusPoint);

                if (value < bestValue)
                {
                    currentX = plusPoint;
                    bestValue = value;
                    continue;
                }

                var minusPoint = new List<double>(currentX);
                minusPoint[nArg] -= step;

                value = calculate(minusPoint);

                if (value < bestValue)
                {
                    currentX = minusPoint;
                    bestValue = value;
                }
            }

            return (currentX, bestValue);
        }
    }
}
