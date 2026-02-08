using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using WaterLogger.Ryanw84.Models;

namespace WaterLogger.Ryanw84.Pages
{
    public class UpdateModel(IConfiguration configuration) : PageModel
    {
        private readonly IConfiguration _configuration = configuration;

        [BindProperty]
        public required DrinkingWaterModel DrinkingWater { get; set; }

        public IActionResult OnGet(int id)
        {
            DrinkingWaterModel? drinkingWater = GetById(id);
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
            SqliteCommand tableCmd = connection.CreateCommand();
            tableCmd.CommandText = $"SELECT * FROM drinking_water WHERE Id = {id}";

            using SqliteDataReader reader = tableCmd.ExecuteReader();

            if (reader.Read())
            {
                drinkingWaterRecord = new DrinkingWaterModel();
                drinkingWaterRecord.Id = (int)reader.GetInt32(0);
                drinkingWaterRecord.Date = DateTime.Parse(
                    reader.GetString(1),
                    CultureInfo.CurrentUICulture.DateTimeFormat
                );
                drinkingWaterRecord.Quantity = reader.GetFloat(2);
            }
            return drinkingWaterRecord;
        }

        public IActionResult OnPost(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            using var connection = new SqliteConnection(
                _configuration.GetConnectionString("ConnectionString")
            );
            connection.Open();
            SqliteCommand tableCmd = connection.CreateCommand();

            tableCmd.CommandText =
                $"UPDATE drinking_water SET date = '{DrinkingWater.Date}', quantity = {DrinkingWater.Quantity} WHERE Id = {DrinkingWater.Id}";

            tableCmd.ExecuteNonQuery();

            return RedirectToPage("./Index");
        }
    }
}
