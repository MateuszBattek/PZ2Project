namespace MvcPracownicy.Models;

//Oferty - Id_oferty, Nazwa, Opis, Cena

public class Offer
{
    public int Id_oferty { get; set; }
    public string Nazwa { get; set; }
    public string Opis { get; set; }
    public int Cena { get; set; }
    public Offer(int id_oferty, string nazwa, string opis, int cena)
    {
        Id_oferty = id_oferty;
        Nazwa = nazwa;
        Opis = opis;
        Cena = cena;
    }
}