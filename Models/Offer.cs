namespace MvcPracownicy.Models;

// Oferty - Id_oferty, Nazwa, Opis, Cena

public class Offer
{
    public int? Id_oferty { get; set; }
    public string Nazwa { get; set; }
    public string Opis { get; set; }
    public int Cena { get; set; }                   // Cena jest w groszach 4000 = 40.00 PLN
    public Offer(string nazwa, string opis, int cena, int? id_oferty = null)
    {
        Nazwa = nazwa;
        Opis = opis;
        Cena = cena;
        Id_oferty = id_oferty;
    }
}