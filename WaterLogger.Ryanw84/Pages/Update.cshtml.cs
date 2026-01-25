using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WaterLogger.Ryanw84.Models;

namespace WaterLogger.Ryanw84.Pages
{
    public class UpdateModel : PageModel
    {
        private readonly IConfiguration _configuration;

        [BindProperty]
        public DrinkingWaterModel DrinkingWater { get; set; }

        public UpdateModel(IConfiguration configuration)
        {
            _configuration configuration
        }

        public IActionResult OnGet(int id)
        {
            DrinkingWater.GetByID(id);

            return Page();
        }
    }
}
