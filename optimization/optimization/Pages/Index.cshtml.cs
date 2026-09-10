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

        public string? Error { get; set; }

        public IActionResult OnPostCalculate()
        {
            if (!ModelState.IsValid)
                return Page();

            Optumizer opt = new Optumizer();
            try
            {
                (double fMin, double xMin) = opt.CalcGoldenRatio(Func, Left, Right, Epsilon);
                ResultX = xMin;
                ResultFx = fMin;

                PlotJson = BuildPlotJson(Func, Left, Right, xMin, fMin);
                Error = null;
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                ResultFx = null;
                ResultX = null;
                PlotJson = null;
            }
            
            return Page();
        }

        private static string BuildPlotJson(string func, double a, double b, double xMin, double fMin)
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

            var payload = new { xs, ys, xMin, fMin };
            return JsonSerializer.Serialize(payload);
        }
    }
}
