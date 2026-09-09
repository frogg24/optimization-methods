using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace optimization.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty] public string Func { get; set; }
        [BindProperty] public double Left { get; set; }
        [BindProperty] public double Right { get; set; }
        [BindProperty] public double Epsilon { get; set; } = 0.001;
        [BindProperty] public int MaxIterations { get; set; } = 100;
        [BindProperty] public string Mode { get; set; } = "Min";

        public double? ResultX { get; set; }
        public double? ResultFx { get; set; }
        public int IterationsDone { get; set; }

        public IActionResult OnPostCalculate()
        {
            if (!ModelState.IsValid)
                return Page();

            //bool minimize = Mode == "Min";
            //double x = Optimize(Left, Right, Epsilon, MaxIterations, minimize, out double fx, out int iters);

            ResultX = Right;
            //ResultFx = fx;
            //IterationsDone = iters;

            return Page();
        }
    }
}
