namespace MvcPracownicy.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Data.Sqlite;
using System.Text;

public class DB : Controller{

    public IActionResult WyswietlDane()
    {
        if (!HttpContext.Session.Keys.Contains("login_status") || HttpContext.Session.GetString("login_status") != "Zalogowano")
            return RedirectToAction("Logowanie", "IO");
        var connectionStringBuilder = new SqliteConnectionStringBuilder();
        connectionStringBuilder.DataSource = "./baza.db";
        using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString)){
            connection.Open();
            SqliteCommand selectCmd = connection.CreateCommand();
            selectCmd.CommandText = "SELECT * FROM Dane";
            using (var reader = selectCmd.ExecuteReader())
            {
                var sb = new StringBuilder();
                while (reader.Read())
                {
                    sb.AppendLine(string.Join(", ", Enumerable.Range(0, reader.FieldCount).Select(i => reader.GetValue(i).ToString())));
                }
                ViewData["dane"] = sb.ToString();
            }
        }
        return View();
    }

    public IActionResult DodajUzytkownika()
    {
        if (!HttpContext.Session.Keys.Contains("login_status") || HttpContext.Session.GetString("login_status") != "Zalogowano")
            return RedirectToAction("Logowanie", "IO");
        return View();
    }

    [HttpPost]
    public IActionResult DodajUzytkownika(IFormCollection form)
    {
        string pesel = form["pesel"].ToString();
        string imie = form["imie"].ToString();
        string nazwisko = form["nazwisko"].ToString();
        
        var connectionStringBuilder = new SqliteConnectionStringBuilder();
        connectionStringBuilder.DataSource = "./baza.db";
        using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
        {
            connection.Open();
            SqliteCommand insertCmd = connection.CreateCommand();
            insertCmd.CommandText =
                "INSERT INTO Dane"
                + "(Pesel, Imie, Nazwisko)"
                + "VALUES (\"" + pesel + "\", \"" + imie + "\", \"" + nazwisko + "\");";
            insertCmd.ExecuteNonQuery();
        }
        return View();

    }



}