namespace MvcPracownicy.Models;

//Umowy - Id_umowy, Id_uzytkownika, Id_oferty, Id_adresu, Data_zawarcia

public class Contract
{
    public int Id_umowy { get; set; }
    public int Id_uzytkownika { get; set; }
    public int Id_oferty { get; set; }
    public int Id_adresu { get; set; }
    public DateTime Data_zawarcia { get; set; }
    public Contract(int id_umowy, int id_uzytkownika, int id_oferty, int id_adresu, string data_zawarcia)
    {
        Id_umowy = id_umowy;
        Id_uzytkownika = id_uzytkownika;
        Id_oferty = id_oferty;
        Id_adresu = id_adresu;
        Data_zawarcia = DateTime.Parse(data_zawarcia);
    }
}