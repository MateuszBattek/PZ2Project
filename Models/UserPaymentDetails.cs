namespace MvcPracownicy.Models;

public class UserPaymentDetails
{
    public int Id_platnosci { get; set; }
    public int Id_umowy { get; set; } // Zachowujemy Id_umowy, jeśli jest potrzebne
    public string NazwaUmowy { get; set; } // Nowe pole na nazwę umowy
    public decimal Kwota { get; set; }
    public DateTime DataPlatnosci { get; set; } // Zmieniłem nazwę dla jasności (może być Data)
    public int Id_uzytkownika { get; set; } // Dodane dla spójności
}