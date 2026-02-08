using System.Globalization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using WaterLogger.Ryanw84.Models;

namespace WaterLogger.Ryanw84.Pages;

public class IndexModel(IConfiguration configuration) : PageModel
{
    private readonly IConfiguration _configuration = configuration;
    public List<DrinkingWaterModel>? Records { get; set; }

    public void OnGet()
    {
        CreateTableIfNotExists();
        Records = GetAllRecords();
        ViewData["Total"] = Records.AsEnumerable().Sum(x => x.Quantity);
    }

    private List<DrinkingWaterModel> GetAllRecords()
    {
        using var connection = new SqliteConnection(
            _configuration.GetConnectionString("ConnectionString")
        );
        connection.Open();
        var tableCmd = connection.CreateCommand();
        tableCmd.CommandText = $"SELECT * FROM drinking_water";

        var tableData = new List<DrinkingWaterModel>();
        SqliteDataReader reader = tableCmd.ExecuteReader();

        while (reader.Read())
        {
            tableData.Add(
                new DrinkingWaterModel
                {
                    Id = reader.GetInt32(0),
                    Date = DateTime.Parse(
                        reader.GetString(1),
                        CultureInfo.CurrentUICulture.DateTimeFormat
                    ),
                    Quantity = reader.GetFloat(2),
                    Measure = reader.IsDBNull(3) ? null : reader.GetString(3),
                }
            );
        }

        return tableData;
    }

    private void CreateTableIfNotExists()
    {
        using var connection = new SqliteConnection(
            _configuration.GetConnectionString("ConnectionString")
        );
        connection.Open();
        var tableCmd = connection.CreateCommand();
        tableCmd.CommandText =
            @"
            CREATE TABLE IF NOT EXISTS drinking_water (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                date TEXT,
                quantity FLOAT,
                measure TEXT
            )
        ";
        tableCmd.ExecuteNonQuery();

        // Add measure column if it doesn't exist (for existing databases)
        var alterCmd = connection.CreateCommand();
        alterCmd.CommandText = "ALTER TABLE drinking_water ADD COLUMN measure TEXT";
        try
        {
            alterCmd.ExecuteNonQuery();
        }
        catch (SqliteException)
        {
            // Column already exists, ignore
        }
    }
}
