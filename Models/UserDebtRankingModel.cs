namespace MvcPracownicy.Models;


public class UserDebtRankingModel
{
    public int Id_uzytkownika { get; set; }
    public string Login { get; set; }
    public string Imie { get; set; }
    public string Nazwisko { get; set; }
    public int TotalDebt { get; set; } // Całkowity dług (suma ze wszystkich umów)
}