
using org.mariuszgromada.math.mxparser;

namespace optimization.Core
{
    public class Optumizer
    {
        // возврат в формате (значение,точка)
        public (double, double) CalcGoldenRatio(string func, double a, double b, double eps)
        {
            if (string.IsNullOrEmpty(func))
            {
                throw new ArgumentNullException("Функция не была введена");
            }
            if(a >= b)
            {
                throw new ArgumentException("Интервал введен некорректно");
            }
            if(eps<= 0)
            {
                throw new ArgumentException("Погрешность должна быть положительной");
            }
            var x = new Argument("x", a);
            var exp = new Expression(func, x);
            if (!exp.checkSyntax())
            {
                throw new ArgumentException("Ошибка в формуле: " + exp.getErrorMessage());
            }

            double tau = (Math.Sqrt(5)-1)/2;

            double x1 = a + (1-tau)*(b-a);
            double x2 = a + tau*(b-a);

            x.setArgumentValue(x1);
            double f1 = exp.calculate();
            x.setArgumentValue(x2);
            double f2 = exp.calculate();
            while (b-a > eps)
            {
                if(f1 < f2)
                {
                    b = x2;

                    x2 = x1;
                    f2=f1;

                    x1 = a + (1-tau)*(b-a);
                    x.setArgumentValue(x1);
                    f1 = exp.calculate();
                }
                else
                {
                    a = x1;

                    x1 = x2;
                    f1=f2;

                    x2 = a + tau*(b-a);
                    x.setArgumentValue(x2);
                    f2 = exp.calculate();
                }
            }

            double xMin = (a + b) / 2.0;
            x.setArgumentValue(xMin);
            double fMin = exp.calculate();

            return (fMin, xMin);
        }
    }
}
