namespace MvcPracownicy.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MvcPracownicy.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Data.Sqlite;
using System.Text;

public class UserController : Controller
{

    // public IActionResult ShowPanel()
    // {
    //     if (!HttpContext.Session.Keys.Contains("Rola") || HttpContext.Session.GetString("Rola") != "Admin")
    //         return RedirectToAction("LogIn", "Authentication");

    //     return View();
    // }

    public IActionResult ChangeUserData()
    {
        // if (!HttpContext.Session.Keys.Contains("Rola") || HttpContext.Session.GetString("Rola") != "Admin")
        //     return RedirectToAction("Ch", "Authentication");
        return View();
    }

    [HttpPost]
    public IActionResult ChangeUserData(IFormCollection form)
    {
        int id_uzytkownika = HttpContext.Session.GetInt32("Id_uzytkownika") ?? 0;
        Console.WriteLine(id_uzytkownika);
        string imie = form["imie"].ToString();
        string nazwisko = form["nazwisko"].ToString();
        string nr_tel = form["nr_tel"].ToString();
        string email = form["email"].ToString();
        string rola = HttpContext.Session.GetString("Rola");


        DBModel.ChangeUserData(new User("", imie, nazwisko, nr_tel, email, rola), id_uzytkownika);

        return View();

    }

    public IActionResult MyPayments()
    {
        return View();
    }

    public IActionResult ShowUserDeals()
    {
        // if (!HttpContext.Session.Keys.Contains("Rola") || HttpContext.Session.GetString("Rola") != "Admin")
        //     return RedirectToAction("LogIn", "Authentication");
        return View(DBModel.GetUserDeals(HttpContext.Session.GetInt32("Id_uzytkownika") ?? 0));
    }

    public IActionResult ShowUserDebts()
    {
        return View(DBModel.GetUserDebt(HttpContext.Session.GetInt32("Id_uzytkownika") ?? 0));
    }



}