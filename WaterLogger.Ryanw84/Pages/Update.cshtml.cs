using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using WaterLogger.Ryanw84.Models;

namespace WaterLogger.Ryanw84.Pages;

public class UpdateModel : PageModel
{
    private readonly IConfiguration _configuration;

    public UpdateModel(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [BindProperty]
    public DrinkingWaterModel DrinkingWater { get; set; } = new();

    public async Task<IActionResult> OnGet(int id)
    {
        await CreateTableIfNotExists();
        var record = await GetById(id);
        if (record == null)
        {
            return NotFound();
        }
        DrinkingWater = record;
        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        await CreateTableIfNotExists();

        using var connection = new SqliteConnection(
            _configuration.GetConnectionString("ConnectionString")
        );
        await connection.OpenAsync();
        var command = connection.CreateCommand();
        command.CommandText =
            "UPDATE drinking_water SET date = $date, quantity = $quantity, measure = $measure WHERE Id = $id";
        command.Parameters.AddWithValue("$id", DrinkingWater.Id);
        command.Parameters.AddWithValue("$date", DrinkingWater.Date.ToString("yyyy-MM-dd"));
        command.Parameters.AddWithValue("$quantity", DrinkingWater.Quantity);
        command.Parameters.AddWithValue("$measure", DrinkingWater.Measure);
        await command.ExecuteNonQueryAsync();

        return RedirectToPage("./Index");
    }

    private async Task<DrinkingWaterModel?> GetById(int id)
    {
        using var connection = new SqliteConnection(
            _configuration.GetConnectionString("ConnectionString")
        );
        await connection.OpenAsync();
        var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM drinking_water WHERE Id = $id";
        command.Parameters.AddWithValue("$id", id);
        var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new DrinkingWaterModel
            {
                Id = reader.GetInt32(0),
                Date = DateOnly.Parse(reader.GetString(1)).ToDateTime(TimeOnly.MinValue),
                Quantity = reader.GetInt32(2),
                Measure = reader.IsDBNull(3) ? null : reader.GetString(3),
            };
        }
        return null;
    }

    private async Task CreateTableIfNotExists()
    {
        using var connection = new SqliteConnection(_configuration.GetConnectionString("ConnectionString"));
        await connection.OpenAsync();
        var command = connection.CreateCommand();
        command.CommandText = "CREATE TABLE IF NOT EXISTS drinking_water (Id INTEGER PRIMARY KEY AUTOINCREMENT, date TEXT, quantity INTEGER, measure TEXT)";
        await command.ExecuteNonQueryAsync();

        // Check if measure column exists
        command.CommandText = "PRAGMA table_info(drinking_water)";
        var reader = await command.ExecuteReaderAsync();
        bool hasMeasure = false;
        while (await reader.ReadAsync())
        {
            if (reader.GetString(1) == "measure")
            {
                hasMeasure = true;
                break;
            }
        }
        reader.Close();

        if (!hasMeasure)
        {
            command.CommandText = "ALTER TABLE drinking_water ADD COLUMN measure TEXT";
            await command.ExecuteNonQueryAsync();
        }
    }
}
