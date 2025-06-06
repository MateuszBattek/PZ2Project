namespace MvcPracownicy.Models;

public class UserDealDetails
{
    public int Id_umowy { get; set; }
    public string NazwaOferty { get; set; } // Nowe pole na nazwę oferty
    public int Id_adresu { get; set; }
    public DateTime Data_zawarcia { get; set; }
    public DateTime? Data_zakonczenia { get; set; } // Pamiętaj, że może być null
    public int Id_uzytkownika { get; set; } // Jeśli chcesz mieć ID użytkownika w tym modelu
}
