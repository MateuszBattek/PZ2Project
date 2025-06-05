namespace MvcPracownicy.Models;

// Uzytkownicy - Id_uzytkownika, Login, Imie, Nazwisko, Nr_tel, Email, Rola

public class User
{
    public int? Id_uzytkownika { get; set; }
    public string Login { get; set; }
    public string Imie { get; set; }
    public string Nazwisko { get; set; }
    public string Nr_tel { get; set; }
    public string Email { get; set; }
    public string Rola { get; set; }

    public User(string login, string imie, string nazwisko, string nr_tel, string email, string rola, int? id_uzytkownika = null)
    {
        Login = login;
        Imie = imie;
        Nazwisko = nazwisko;
        Nr_tel = nr_tel;
        Email = email;
        Rola = rola;
        Id_uzytkownika = id_uzytkownika;
    }
}