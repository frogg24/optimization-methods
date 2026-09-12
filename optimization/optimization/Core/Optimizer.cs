
using org.mariuszgromada.math.mxparser;

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
    }
}
