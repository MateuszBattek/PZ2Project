namespace MvcPracownicy.Models;

// Platnosci - Id_platnosci, Id_uzytkownika, Id_oferty, Kwota, Data

public class Payment
{
    public int Id_platnosci { get; set; }
    public int Id_uzytkownika { get; set; }
    public int Id_oferty { get; set; }
    public int Kwota { get; set; }
    public DateTime Data { get; set; }
    public Payment(int id_platnosci, int id_uzytkownika, int id_oferty, int kwota, string data)
    {
        Id_platnosci = id_platnosci;
        Id_uzytkownika = id_uzytkownika;
        Id_oferty = id_oferty;
        Kwota = kwota;
        Data = DateTime.Parse(data);
    }
}
