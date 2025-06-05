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
public class AuthenticationController : Controller
{

    //Obsługa metody GET 
    public IActionResult LogIn()
    {
        if (!HttpContext.Session.Keys.Contains("Id_uzytkownika"))
            ViewData["login_status"] = "Nie zalogowano";
        else
            ViewData["login_status"] = "Zalogowano jako " + HttpContext.Session.GetString("Rola");
        return View();

    }

    //Obsługa metody POST
    [HttpPost]
    public IActionResult LogIn(IFormCollection form)
    {
        string login = form["login"].ToString();
        string haslo = form["haslo"].ToString();

        User? user = DBModel.LogIn(login, haslo);
        bool zalogowany = user != null;

        string login_status;
        if (zalogowany)
        {
            login_status = "Zalogowano jako " + (user.Rola ?? "");
            HttpContext.Session.SetInt32("Id_uzytkownika", user.Id_uzytkownika ?? 0);
            HttpContext.Session.SetString("Rola", user.Rola ?? "");
        }
        else
            login_status = "Nie zalogowano";

        ViewData["login_status"] = login_status;

        return View();
    }


    public IActionResult LogOut()
    {
        if (HttpContext.Session.Keys.Contains("Id_uzytkownika"))
            HttpContext.Session.Remove("Id_uzytkownika");
        if (HttpContext.Session.Keys.Contains("Rola"))
            HttpContext.Session.Remove("Rola");
        return RedirectToAction("LogIn", "Authentication");
    }
}