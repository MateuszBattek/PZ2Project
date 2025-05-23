//Klasa oraz metoda modyfikująca standardowy routing 
//Wywołanie: http://localhost:5083/api/IO/index?id1=xxx&id2=yyy
namespace MvcPracownicy.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;
using Microsoft.Data.Sqlite;
using System.Text;
public class IO : Controller
{
    
    //Obsługa metody GET 
    public IActionResult Logowanie()
    {
        if(!HttpContext.Session.Keys.Contains("login_status"))
            ViewData["login_status"] = "Nie zalogowano";
        else
            ViewData["login_status"] = HttpContext.Session.GetString("login_status");
        return View();
    }

    //Obsługa metody POST
    [HttpPost] 
    public IActionResult Logowanie(IFormCollection form)
    {
        string login = form["login"].ToString();
        string haslo = form["haslo"].ToString();
        bool zalogowany = false;
        var connectionStringBuilder = new SqliteConnectionStringBuilder();
        connectionStringBuilder.DataSource = "./baza.db";
        using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString)){

            string haslo_hash = "";
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(haslo);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                haslo_hash = Convert.ToHexString(hashBytes);
            }

            connection.Open();
            SqliteCommand selectCmd = connection.CreateCommand();
            selectCmd.CommandText = $"SELECT COUNT(*) FROM Loginy WHERE Login = \"{login}\" AND Haslo = \"{haslo_hash}\"";
            string result = selectCmd.ExecuteScalar().ToString();
            if(result == "1")
                zalogowany = true;
        }

        string login_status;
        if(zalogowany)
            login_status = "Zalogowano";
        else
            login_status = "Błędny login lub hasło";
        HttpContext.Session.SetString("login_status", login_status);
        if (login_status == "Zalogowano")
            return RedirectToAction("SecretData");
        ViewData["login_status"] = login_status;

        return View();
    }

    public IActionResult SecretData()
    {
        if (HttpContext.Session.Keys.Contains("login_status") && HttpContext.Session.GetString("login_status") == "Zalogowano")
            return View();
        return RedirectToAction("Logowanie");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.SetString("login_status", "Nie zalogowano");
        return RedirectToAction("Logowanie");
    }
}