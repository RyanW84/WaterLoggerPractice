using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using WaterLogger.Ryanw84.Models;

namespace WaterLogger.Ryanw84.Pages
{
    public class Create : PageModel
    {
        public class CreateModel(IConfiguration configuration) : PageModel
        {
            private readonly IConfiguration _configuration = configuration;

            public IActionResult OnGet()
            {
                return Page();
            }

            [BindProperty]
            public DrinkingWaterModel? DrinkingWater { get; set; }

            public IActionResult OnPost()
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }

                using (
                    var connection = new SqliteConnection(
                        _configuration.GetConnectionString("ConnectionString")
                    )
                )
                {
                    connection.Open();
                    var tableCmd = connection.CreateCommand();
                    tableCmd.CommandText =
                        $"INSERT INTO drinking_water(date, quantity) VALUES('{DrinkingWater.Date}, {DrinkingWater.Quantity})";

                    tableCmd.ExecuteNonQuery();
                }

                return RedirectToPage("./Index");
            }
        }
    }
}
