using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
            Console.WriteLine($"Func:{Func}\nDimension:{Dimension}\n");
            foreach(var i in X0)
            {
                Console.WriteLine(i);
            }
            Console.WriteLine($"Step:{Step}\nStepRed:{StepRed}\n");
            return Page();
        }
    }
}
