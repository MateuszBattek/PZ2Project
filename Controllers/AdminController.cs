namespace MvcPracownicy.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MvcPracownicy.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Data.Sqlite;
using System.Text;

public class AdminController : Controller{

    public IActionResult ShowPanel()
    {
        if (!HttpContext.Session.Keys.Contains("Rola") || HttpContext.Session.GetString("Rola") != "Admin")
            return RedirectToAction("LogIn", "Authentication");

        return View();
    }

    public IActionResult AddUser()
    {
        if (!HttpContext.Session.Keys.Contains("Rola") || HttpContext.Session.GetString("Rola") != "Admin")
            return RedirectToAction("LogIn", "Authentication");
        return View();
    }

    [HttpPost]
    public IActionResult AddUser(IFormCollection form)
    {
        string login = form["login"].ToString();
        string haslo = form["haslo"].ToString();
        string imie = form["imie"].ToString();
        string nazwisko = form["nazwisko"].ToString();
        string nr_tel = form["nr_tel"].ToString();
        string email = form["email"].ToString();
        string rola = form["rola"].ToString();
        

        DBModel.AddUser(new User(login, imie, nazwisko, nr_tel, email, rola), haslo);
        
        return View();

    }



}