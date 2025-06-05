namespace MvcPracownicy.Models;

//Adresy - Id_adresu, Kraj, Wojewodztwo, Kod_pocztowy, Miasto, Ulica, Numer_domu, Numer_mieszkania

public class Address
{
    public int? Id_adresu { get; set; }
    public string Kraj { get; set; }
    public string Wojewodztwo { get; set; }
    public string Kod_pocztowy { get; set; }
    public string Miasto { get; set; }
    public string Ulica { get; set; }
    public string Numer_domu { get; set; }
    public string Numer_mieszkania { get; set; }
    public Address(string kraj, string wojewodztwo, string kod_pocztowy, string miasto, string ulica, string numer_domu, string numer_mieszkania = "", int? id_adresu = null)
    {
        Id_adresu = id_adresu;
        Kraj = kraj;
        Wojewodztwo = wojewodztwo;
        Kod_pocztowy = kod_pocztowy;
        Miasto = miasto;
        Ulica = ulica;
        Numer_domu = numer_domu;
        Numer_mieszkania = numer_mieszkania;
    }
}