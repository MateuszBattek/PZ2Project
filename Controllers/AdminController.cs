namespace MvcPracownicy.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MvcPracownicy.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Data.Sqlite;
using System.Text;

public class AdminController : Controller
{

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

    public IActionResult AddOffer()
    {
        if (!HttpContext.Session.Keys.Contains("Rola") || HttpContext.Session.GetString("Rola") != "Admin")
            return RedirectToAction("LogIn", "Authentication");
        return View();
    }
    [HttpPost]
    public IActionResult AddOffer(IFormCollection form)
    {
        string nazwa = form["nazwa"].ToString();
        string opis = form["opis"].ToString();
        int cena = 0;
        if (!int.TryParse(form["cena"], out cena))
            return View();
        DBModel.AddOffer(new Offer(nazwa, opis, cena));
        return View();
    }

    public IActionResult AddAddress()
    {
        if (!HttpContext.Session.Keys.Contains("Rola") || HttpContext.Session.GetString("Rola") != "Admin")
            return RedirectToAction("LogIn", "Authentication");
        return View();
    }
    [HttpPost]
    public IActionResult AddAddress(IFormCollection form)
    {
        string kraj = form["kraj"].ToString();
        string wojewodztwo = form["wojewodztwo"].ToString();
        string kod_pocztowy = form["kod_pocztowy"].ToString();
        string miasto = form["miasto"].ToString();
        string ulica = form["ulica"].ToString();
        string numer_domu = form["numer_domu"].ToString();
        string numer_mieszkania = form["numer_mieszkania"].ToString();
        DBModel.AddAddress(new Address(kraj, wojewodztwo, kod_pocztowy, miasto, ulica, numer_domu, numer_mieszkania));
        return View();
    }

    public IActionResult AddDeal()
    {
        if (!HttpContext.Session.Keys.Contains("Rola") || HttpContext.Session.GetString("Rola") != "Admin")
            return RedirectToAction("LogIn", "Authentication");
        return View();
    }
    [HttpPost]
    public IActionResult AddDeal(IFormCollection form)
    {
        int id_uzytkownika;
        if (!int.TryParse(form["id_pracownika"], out id_uzytkownika))
            return View();
        int id_oferty;
        if (!int.TryParse(form["id_oferty"], out id_oferty))
            return View();
        int id_adresu;
        if (!int.TryParse(form["id_adresu"], out id_adresu))
            return View();
        DateTime data_zawarcia;
        if (!DateTime.TryParse(form["data_zawarcia"], out data_zawarcia))
            return View();
        DBModel.AddDeal(new Deal(id_uzytkownika, id_oferty, id_adresu, data_zawarcia));
        return View();
    }

    public IActionResult ShowUsers()
    {
        if (!HttpContext.Session.Keys.Contains("Rola") || HttpContext.Session.GetString("Rola") != "Admin")
            return RedirectToAction("LogIn", "Authentication");
        return View(DBModel.GetUsers());
        
    }



}