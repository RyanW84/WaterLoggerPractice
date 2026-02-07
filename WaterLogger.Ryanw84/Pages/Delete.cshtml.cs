using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using WaterLogger.Ryanw84.Models;

namespace WaterLogger.Ryanw84.Pages
{
    public class DeleteModel(IConfiguration configuration) : PageModel
    {
        private readonly IConfiguration _configuration = configuration;

        [BindProperty]
        public DrinkingWaterModel? DrinkingWater { get; set; }

        public IActionResult OnGet(int id)
        {
            var drinkingWater = GetById(id);
            if (drinkingWater is null)
            {
                return NotFound();
            }
            DrinkingWater = drinkingWater;
            return Page();
        }

        private DrinkingWaterModel? GetById(int id)
        {
            DrinkingWaterModel? drinkingWaterRecord = null;

            using var connection = new SqliteConnection(
                _configuration.GetConnectionString("ConnectionString")
            );
            connection.Open();
            var tableCmd = connection.CreateCommand();
            tableCmd.CommandText = $"SELECT * FROM drinking_water WHERE Id = {id}";

            using var reader = tableCmd.ExecuteReader();

            while (reader.Read())
            {
                drinkingWaterRecord = new DrinkingWaterModel
                {
                    Id = reader.GetInt32(0),
                    Date = DateTime.Parse(
                        reader.GetString(1),
                        CultureInfo.CurrentUICulture.DateTimeFormat
                    ),
                    Quantity = reader.GetInt32(2),
                };
            }
            return drinkingWaterRecord;
        }

        public IActionResult OnPost(int id)
        {
            using var connection = new SqliteConnection(
                _configuration.GetConnectionString("ConnectionString")
            );
            connection.Open();
            var tableCmd = connection.CreateCommand();

            tableCmd.CommandText = $"DELETE from drinking_water WHERE Id = {id}";

            tableCmd.ExecuteNonQuery();

            return RedirectToPage("./Index");
        }
    }
}
