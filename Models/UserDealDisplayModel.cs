
namespace MvcPracownicy.Models;

public class UserDealDisplayModel
{
    public int Id_umowy { get; set; }
    public string LoginUzytkownika { get; set; }
    public string NazwaOferty { get; set; }
    public int Id_adresu { get; set; }
    public DateTime Data_zawarcia { get; set; }
    public DateTime? Data_zakonczenia { get; set; }
}

