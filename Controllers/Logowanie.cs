//Klasa oraz metoda modyfikująca standardowy routing 
//Wywołanie: http://localhost:5083/api/IO/index?id1=xxx&id2=yyy
namespace MvcPracownicy.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;
using Microsoft.Data.Sqlite;
using MvcPracownicy.Models;
using System.Text;
public class IO : Controller
{

    //Obsługa metody GET 
    public IActionResult Logowanie()
    {
        if (!HttpContext.Session.Keys.Contains("user_id"))
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

        User? user = DBModel.LogIn(login, haslo);
        bool zalogowany = user != null;
        
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


    public IActionResult Logout()
    {
        HttpContext.Session.SetString("login_status", "Nie zalogowano");
        return RedirectToAction("Logowanie");
    }
}