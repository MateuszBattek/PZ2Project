namespace MvcPracownicy.Models;

// Umowy - Id_umowy, Id_uzytkownika, Id_oferty, Id_adresu, Data_zawarcia, Data_zakonczenia (null jeśli jeszcze trwa)

public class Deal
{
    public int? Id_umowy { get; set; }
    public int Id_uzytkownika { get; set; }
    public int Id_oferty { get; set; }
    public int Id_adresu { get; set; }
    public DateTime Data_zawarcia { get; set; }
    public DateTime? Data_zakonczenia { get; set; }
    public Deal(int id_uzytkownika, int id_oferty, int id_adresu, DateTime data_zawarcia, int? id_umowy = null, DateTime? data_zakonczenia = null)
    {
        Id_uzytkownika = id_uzytkownika;
        Id_oferty = id_oferty;
        Id_adresu = id_adresu;
        Data_zawarcia = data_zawarcia;
        Data_zakonczenia = data_zakonczenia;
        Id_umowy = id_umowy;
    }
}