using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using optimization.Core;

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
        //public int IterationsDone { get; set; }

        public IActionResult OnPostCalculate()
        {
            if (!ModelState.IsValid)
                return Page();

            Optumizer opt = new Optumizer();
            (double, double) res = opt.CalcGoldenRatio(Func, Left, Right, Epsilon);
            ResultX = res.Item2;
            ResultFx = res.Item1;

            return Page();
        }
    }
}
