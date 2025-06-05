namespace MvcPracownicy.Models;

// Platnosci - Id_platnosci, Id_uzytkownika, Id_umowy, Kwota, Data

public class Payment
{
    public int? Id_platnosci { get; set; }
    public int Id_uzytkownika { get; set; }
    public int Id_umowy { get; set; }
    public int Kwota { get; set; }              // Kwota jest w groszach 4000 = 40.00 PLN
    public DateTime Data { get; set; }
    public Payment(int id_uzytkownika, int id_umowy, int kwota, string data, int? id_platnosci = null)
    {
        Id_uzytkownika = id_uzytkownika;
        Id_umowy = id_umowy;
        Kwota = kwota;
        Data = DateTime.Parse(data);
        Id_platnosci = id_platnosci;
    }
}
