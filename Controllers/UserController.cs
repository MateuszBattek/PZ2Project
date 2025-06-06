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
        if (!HttpContext.Session.Keys.Contains("Rola"))
            return RedirectToAction("LogIn", "Authentication");
        return View();
    }

    [HttpPost]
    public IActionResult ChangeUserData(IFormCollection form)
    {
        int id_uzytkownika = HttpContext.Session.GetInt32("Id_uzytkownika") ?? 0;
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
        if (!HttpContext.Session.Keys.Contains("Rola"))
            return RedirectToAction("LogIn", "Authentication");
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


    public IActionResult ShowUserPayments()
    {
        // if (!HttpContext.Session.Keys.Contains("Rola") || HttpContext.Session.GetString("Rola") != "Admin")
        //     return RedirectToAction("LogIn", "Authentication");
        return View(DBModel.GetUserPayments(HttpContext.Session.GetInt32("Id_uzytkownika") ?? 0));
    }

    public IActionResult ShowOffers()
    {
        return View(DBModel.GetOffers());
    }

    public IActionResult AddDeal()
    {
        // if (!HttpContext.Session.Keys.Contains("Rola") || HttpContext.Session.GetString("Rola") != "Admin")
        //     return RedirectToAction("LogIn", "Authentication");
        ViewBag.Offers = DBModel.GetAllOffers();
        return View();
    }

    [HttpPost]
    public IActionResult AddDeal(IFormCollection form)
    {
        int id_uzytkownika = HttpContext.Session.GetInt32("Id_uzytkownika") ?? 0;
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

    [HttpPost]
    public IActionResult EndDeal(int dealId)
    {
        DBModel.EndDeal(dealId);
        return RedirectToAction("ShowUserDeals");

    }

    [HttpPost]
    public IActionResult AddPayment(int dealId, int amount)
    {
        int userId = HttpContext.Session.GetInt32("Id_uzytkownika") ?? 0;


        if (amount <= 0)
        {
            TempData["ErrorMessage"] = "Kwota płatności musi być większa od zera.";
            return RedirectToAction("ShowUserDeals");
        }

        try
        {
            Payment payment = new Payment
            {
                Id_uzytkownika = userId,
                Id_umowy = dealId,
                Kwota = amount,
                Data = DateTime.Now,

            };
            DBModel.AddPayment(payment);
            TempData["SuccessMessage"] = $"Płatność w kwocie {amount:C2} za umowę ID: {dealId} została pomyślnie dodana.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Wystąpił błąd podczas dodawania płatności: {ex.Message}";
        }

        return RedirectToAction("ShowUserDeals");
    }



}