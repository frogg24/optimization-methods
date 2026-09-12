using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using optimization.Core;
using org.mariuszgromada.math.mxparser;
using System.Text.Json;

namespace optimization.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty] public string Func { get; set; }
        [BindProperty] public double Left { get; set; }
        [BindProperty] public double Right { get; set; }
        [BindProperty] public double Epsilon { get; set; } = 0.001;
        [BindProperty] public string Mode { get; set; } = "Min";

        public double? ResultX { get; set; }
        public double? ResultFx { get; set; }
        public string? PlotJson { get; set; }
        public int? Iterations { get; set; }

        public string? Error { get; set; }

        public IActionResult OnPostCalculate()
        {
            if (!ModelState.IsValid)
                return Page();

            Optimizer opt = new Optimizer();
            try
            {
                string originalFunc = Func;
                string funcForOpt = Mode == "Max" ? $"-({Func})" : Func;

                (double fOpt, double xOpt, int iterations) = opt.CalcGoldenRatio(funcForOpt, Left, Right, Epsilon);
                ResultX = xOpt;
                ResultFx = Mode == "Max" ? -fOpt : fOpt;
                Iterations = iterations;

                PlotJson = BuildPlotJson(originalFunc, Left, Right, xOpt, ResultFx.Value, Mode);
                Error = null;
            }
            catch (ArgumentException ex)
            {
                Error = ex.Message;
                ResultFx = null;
                ResultX = null;
                PlotJson = null;
            }
            catch (Exception)
            {
                Error = "Произошла внутренняя ошибка при выполнении оптимизации";
                ResultFx = null;
                ResultX = null;
                PlotJson = null;
            }

            return Page();
        }

        private static string BuildPlotJson(string func, double a, double b, double xMin, double fMin, string mode)
        {
            var xArg = new Argument("x", a);
            var exp = new Expression(func, xArg);

            double F(double v)
            {
                xArg.setArgumentValue(v);
                return exp.calculate();
            }

            const int n = 500;
            var xs = new double[n];
            var ys = new double?[n];   // nullable: null = разрыв функции

            for (int i = 0; i < n; i++)
            {
                xs[i] = a + (b - a) * i / (n - 1);
                double y = F(xs[i]);
                ys[i] = double.IsFinite(y) ? y : null;
            }

            var payload = new { xs, ys, xMin, fMin, optimumName = mode == "Max" ? "Максимум" : "Минимум" };
            return JsonSerializer.Serialize(payload);
        }
    }
}
