using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using WaterLogger.Ryanw84.Models;

namespace WaterLogger.Ryanw84.Pages
{
    public class Create(IConfiguration configuration) : PageModel
    {
        private readonly IConfiguration _configuration = configuration;

        [BindProperty]
        public DrinkingWaterModel? DrinkingWater { get; set; }

        public IActionResult OnGet() //
        {
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid || DrinkingWater is null)
            {
                return Page();
            }

            using var connection = new SqliteConnection(
                _configuration.GetConnectionString("ConnectionString")
            );

            connection.Open();

            using var tableCmd = connection.CreateCommand();
            tableCmd.CommandText =
                "INSERT INTO drinking_water(date, quantity, measure) VALUES($date, $quantity, $measure)";
            tableCmd.Parameters.AddWithValue("$date", DrinkingWater.Date);
            tableCmd.Parameters.AddWithValue("$quantity", DrinkingWater.Quantity);
            tableCmd.Parameters.AddWithValue("$measure", DrinkingWater.Measure);

            tableCmd.ExecuteNonQuery();

            return RedirectToPage("./Index");
        }
    }
}
