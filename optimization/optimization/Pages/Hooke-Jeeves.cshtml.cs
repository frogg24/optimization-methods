using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using optimization.Core;
using org.mariuszgromada.math.mxparser;
using System.Text.Json;

namespace optimization.Pages
{
    public class Hooke_JeevesModel : PageModel
    {
        [BindProperty] public string Func { get; set; }
        [BindProperty] public int Dimension { get; set; } = 2;
        [BindProperty] public List<double> X0 { get; set; } = new List<double>();
        [BindProperty] public double Step { get; set; }
        [BindProperty] public double StepRed { get; set; }
        [BindProperty] public double Epsilon { get; set; } = 0.001;
        [BindProperty] public string Mode { get; set; } = "Min";

        public List<double> ResultX { get; set; }
        public double? ResultFx { get; set; }
        public string? PlotJson { get; set; }

        public string? Error { get; set; }
        public void OnGet()
        {
        }

        public IActionResult OnPostCalculateHooke()
        {
            if (!ModelState.IsValid)
                return Page();

            string originalFunc = Func;
            string funcForOpt = Mode == "Max" ? $"-({Func})" : Func;

            Optimizer opt = new Optimizer();
            (List<double> resX, double resFx) = opt.CalcHookeJeeves(funcForOpt, X0, Step, StepRed, Epsilon);
            ResultFx = resFx;
            ResultX = resX;
            if (ResultX.Count == 1)
            {
                PlotJson = BuildPlot2DJson(originalFunc, X0[0], ResultX[0], ResultFx.Value, Mode);
            }
            else if(ResultX.Count == 2)
            {
                PlotJson = BuildPlot3DJson(originalFunc, X0, ResultX, ResultFx.Value, Mode);
            }

            Error = null;
            return Page();
        }

        private static string BuildPlot2DJson(string func, double startX, double xOpt, double fOpt, string mode)
        {
            var x1 = new Argument("x1", 0);
            var exp = new Expression(func, x1);

            if (!exp.checkSyntax())
            {
                throw new ArgumentException("Ошибка в формуле: " + exp.getErrorMessage());
            }

            // Строим диапазон так, чтобы были видны и начальная, и найденная точки
            double minX = Math.Min(startX, xOpt);
            double maxX = Math.Max(startX, xOpt);

            double distance = maxX - minX;

            // Если точки почти совпали, всё равно нужен нормальный диапазон
            double padding = Math.Max(2.0, distance * 0.5);

            double left = minX - padding;
            double right = maxX + padding;

            const int count = 400;

            var xs = new double[count];
            var ys = new double?[count];

            for (int i = 0; i < count; i++)
            {
                double x = left + (right - left) * i / (count - 1);

                x1.setArgumentValue(x);

                double y = exp.calculate();

                xs[i] = x;
                ys[i] = double.IsFinite(y) ? y : null;
            }

            var payload = new
            {
                type = "2d",
                xs,
                ys,

                xOpt,
                fOpt,

                startX,

                optimumName = mode == "Max" ? "Максимум" : "Минимум"
            };

            return JsonSerializer.Serialize(payload);
        }
        private static string BuildPlot3DJson(string func, List<double> startX, List<double> resultX, double fOpt, string mode)
        {
            var x1 = new Argument("x1", 0);
            var x2 = new Argument("x2", 0);
            var exp = new Expression(func, x1, x2);

            if (!exp.checkSyntax())
            {
                throw new ArgumentException("Ошибка в формуле: " + exp.getErrorMessage());
            }

            double xOpt = resultX[0];
            double yOpt = resultX[1];
            double startX1 = startX[0];
            double startX2 = startX[1];

            double minX = Math.Min(startX1, xOpt);
            double maxX = Math.Max(startX1, xOpt);
            double minY = Math.Min(startX2, yOpt);
            double maxY = Math.Max(startX2, yOpt);

            double paddingX = Math.Max(2.0, (maxX - minX) * 0.5);
            double paddingY = Math.Max(2.0, (maxY - minY) * 0.5);

            double leftX = minX - paddingX;
            double rightX = maxX + paddingX;
            double leftY = minY - paddingY;
            double rightY = maxY + paddingY;

            const int count = 70;

            var xs = new double[count];
            var ys = new double[count];

            for (int i = 0; i < count; i++)
            {
                xs[i] = leftX + (rightX - leftX) * i / (count - 1);
                ys[i] = leftY + (rightY - leftY) * i / (count - 1);
            }

            var zs = new double?[count][];

            for (int yIndex = 0; yIndex < count; yIndex++)
            {
                zs[yIndex] = new double?[count];

                for (int xIndex = 0; xIndex < count; xIndex++)
                {
                    x1.setArgumentValue(xs[xIndex]);
                    x2.setArgumentValue(ys[yIndex]);

                    double value = exp.calculate();
                    zs[yIndex][xIndex] = double.IsFinite(value) ? value : null;
                }
            }

            var payload = new
            {
                type = "3d",
                xs,
                ys,
                zs,
                xOpt,
                yOpt,
                fOpt,
                startX = startX1,
                startY = startX2,
                optimumName = mode == "Max" ? "Максимум" : "Минимум"
            };

            return JsonSerializer.Serialize(payload);
        }
    }
}
