using System.Data.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WaterLogger.Ryanw84.Models;

namespace WaterLogger.Ryanw84.Pages
{
    public class Create : PageModel
    {
        public class CreateModel : PageModel
        {
            private readonly IConfiguration _configuration;

            public CreateModel(IConfiguration configuration)
            {
                _configuration = configuration;
            }

            public IActionResult OnGet()
            {
                return Page();
            }

            [BindProperty]
            public DrinkingWaterModel DrinkingWater { get; set; }

            public IActionResult OnPost()
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }

                using (var connection = new SqliteConnection(DbConnectionStringBuilder))
                {
                    connection.Open();
                    var tableCmd=connection.CreateCommand();
                    tableCmd.CommandText=$"INSERT INTO drinking_water(date, quantity) VALUES('{DrinkingWater.Date}, {DrinkingWater.Quantity})";

                    tableCmd.ExecuteNonQuery();

                    connection.Close();
                }
            }
        }
    }
}
