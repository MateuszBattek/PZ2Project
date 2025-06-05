namespace MvcPracownicy.Models;

using Microsoft.Data.Sqlite;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Metoda dodania uzytkownika, tworzy sie hash SHA256 i zapisuje do bazy
/// </summary>
public class DBModel
{
    private static readonly string connectionString = "Data Source=Models/baza.db";
    public static void AddUser(User user, string password)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            string password_hash = "";
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                password_hash = Convert.ToHexString(hashBytes);
            }
            connection.Open();
            SqliteCommand insertCmd = connection.CreateCommand();
            insertCmd.CommandText = @"
                INSERT INTO Uzytkownicy (Login, Haslo, Imie, Nazwisko, Nr_tel, Email, Rola)
                VALUES (@login, @haslo, @imie, @nazwisko, @nrTel, @email, @rola)";
            insertCmd.Parameters.AddWithValue("@login", user.Login);
            insertCmd.Parameters.AddWithValue("@haslo", password);
            insertCmd.Parameters.AddWithValue("@imie", user.Imie);
            insertCmd.Parameters.AddWithValue("@nazwisko", user.Nazwisko);
            insertCmd.Parameters.AddWithValue("@nrTel", user.Nr_tel);
            insertCmd.Parameters.AddWithValue("@email", user.Email);
            insertCmd.Parameters.AddWithValue("@rola", user.Rola);

            insertCmd.ExecuteNonQuery();
        }
    }

    public static User? GetUser(string user_id)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            SqliteCommand selectCmd = connection.CreateCommand();
            selectCmd.CommandText = "SELECT Id_uzytkownika, Login, Imie, Nazwisko, Nr_tel, Email, Rola FROM Uzytkownicy WHERE Id_uzytkownika = @id_uzytkownika";
            selectCmd.Parameters.AddWithValue("@id_uzytkownika", user_id);
            using (SqliteDataReader reader = selectCmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    int Id_uzytkownika = reader.GetInt32(0);
                    string Login = reader.GetString(1);
                    string Imie = reader.GetString(2);
                    string Nazwisko = reader.GetString(3);
                    string Nr_tel = reader.GetString(4);
                    string Email = reader.GetString(5);
                    string Rola = reader.GetString(6);
                    User user = new User(Login, Imie, Nazwisko, Nr_tel, Email, Rola, Id_uzytkownika);
                    return user;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Usuwa uzytkownika
    /// </summary>
    /// <param name="login"></param>
    public static void DeleteUser(string login)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            SqliteCommand deleteCmd = connection.CreateCommand();
            deleteCmd.CommandText = "DELETE FROM Uzytkownicy WHERE Login = @login";
            deleteCmd.Parameters.AddWithValue("@login", login);
            deleteCmd.ExecuteNonQuery();
        }
    }

    /// <summary>
    /// Sprawdza czy użytkownik się zalogował i jego role
    /// </summary>
    /// <param name="login"></param>
    /// <param name="haslo"></param>
    /// <returns>
    /// User lub null jeśli go nie ma/jest złe hasło
    /// </returns>
    public static User? LogIn(string login, string haslo)
    {
        string hashZBazy = "";
        User? user = null;
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = "SELECT Haslo, Imie, Nazwisko, Nr_tel, Email, Rola, Id_uzytkownika FROM Uzytkownicy WHERE Login = @login";
            selectCmd.Parameters.AddWithValue("@login", login);
            using (var reader = selectCmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    hashZBazy = reader.GetString(0);
                    string imie = reader.GetString(1);
                    string nazwisko = reader.GetString(2);
                    string nr_tel = reader.GetString(3);
                    string email = reader.GetString(4);
                    string rola = reader.GetString(5);
                    int id_uzytkownika = reader.GetInt32(6);
                    user = new User(login, imie, nazwisko, nr_tel, email, rola, id_uzytkownika);
                }
                else
                    return null;
            }
        }
        string hashWpisanego = "";
        using (SHA256 sha256 = SHA256.Create()) // lub MD5, jeśli wcześniej użyłeś MD5
        {
            byte[] bytes = Encoding.UTF8.GetBytes(haslo);
            byte[] hashBytes = sha256.ComputeHash(bytes);
            hashWpisanego = Convert.ToHexString(hashBytes);
        }
        if (hashWpisanego.Equals(hashZBazy, StringComparison.OrdinalIgnoreCase))
            return user;
        else
            return null;
    }

    /// <summary>
    /// Pobiera wszystkich uzytkowników
    /// </summary>
    /// <returns>
    /// zwraca List<User> z wszystkimi użytkownikami
    /// </returns>
    public static List<User> GetUsers()
    {
        List<User> users = new List<User>();
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = "SELECT Login, Imie, Nazwisko, Nr_tel, Email, Rola, Id_uzytkownika FROM Uzytkownicy";
            using (var reader = selectCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    User? user = null;
                    string login = reader.GetString(0);
                    string imie = reader.GetString(1);
                    string nazwisko = reader.GetString(2);
                    string nr_tel = reader.GetString(3);
                    string email = reader.GetString(4);
                    string rola = reader.GetString(5);
                    int id_uzytkownika = reader.GetInt32(6);
                    user = new User(login, imie, nazwisko, nr_tel, email, rola, id_uzytkownika);
                    users.Add(user);
                }
            }
        }
        return users;
    }

    /// <summary>
    /// Metoda zwraca dług użytkownika (tyle ile musi zapłacić)
    /// uznaje, ze trzeba zapłacić, jeśli zmieni się miesiąc
    /// (nawet zawarcie umowy 2020.05.31 do data bieżąca 2020.06.01)
    /// </summary>
    /// <param name="user"></param>
    /// <returns>
    /// zwraca Dictionary<int, int> z id_umowy jako klucz i kwotą jako wartość, JEŚLI KWOTA JEST UJEMNA to użytkownik nadpłacił
    /// </returns>
    public static Dictionary<int, int> GetUserDebt(User user)
    {
        Dictionary<int, int> debt = new Dictionary<int, int>();
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = @"
            SELECT
                U.Id_umowy,
                ((strftime('%Y', 'now') - strftime('%Y', U.Data_zawarcia)) * 12 +
                (strftime('%m', 'now') - strftime('%m', U.Data_zawarcia))) * O.Kwota AS Suma
            FROM Umowy U
            JOIN Oferty O on U.Id_oferty = O.Id_oferty
            JOIN Uzytkownicy U2 on U.Id_uzytkownika = U2.Id_uzytkownika
            WHERE U2.Id_uzytkownika = @user_id and U.Data_zakonczenia IS NULL
            GROUP BY U.Id_umowy
            ";
            selectCmd.Parameters.AddWithValue("@user_id", user.Id_uzytkownika);

            using (var reader = selectCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    debt.Add(reader.GetInt32(0), reader.GetInt32(1));           //kwerenda licząca ile musi zapłacić    
                }
            }

            selectCmd.CommandText = @"
            SELECT 
                U.Id_umowy, sum(P.Kwota)
            FROM Umowy U
            JOIN Platnosci P on U.Id_umowy = P.Id_umowy
            JOIN Uzytkownicy U2 on U.Id_uzytkownika = U2.Id_uzytkownika
            WHERE U2.Id_uzytkownika = @user_id and U.Data_zakonczenia IS NULL;
            GROUP BY U.Id_umowy
            ";
            selectCmd.Parameters.AddWithValue("@user_id", user.Id_uzytkownika);

            using (var reader = selectCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    debt[reader.GetInt32(0)] -= reader.GetInt32(1);            //kwerenda licząca ile zapłacił
                }
            }
        }
        return debt;
    }

    public static List<Deal> GetUserActiveDeals(User user)
    {
        List<Deal> deals = new List<Deal>();
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = @"
            SELECT Id_umowy, Id_uzytkownika, Id_oferty, Id_adresu, Data_zawarcia
            FROM Umowy
            WHERE Id_uzytkownika = @user_id and Data_zakonczenia IS NULL
            ";
            selectCmd.Parameters.AddWithValue("@user_id", user.Id_uzytkownika);
            using (var reader = selectCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id_umowy = reader.GetInt32(0);
                    int id_uzytkownika = reader.GetInt32(1);
                    int id_oferty = reader.GetInt32(2);
                    int id_adresu = reader.GetInt32(3);
                    DateTime data_zawarcia = DateTime.Parse(reader.GetString(4));
                    Deal deal = new Deal(id_uzytkownika, id_oferty, id_adresu, data_zawarcia, id_umowy);
                    deals.Add(deal);
                }
            }
        }
        return deals;
    }


    public static void AddPayment(Payment payment)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var insertCmd = connection.CreateCommand();
            insertCmd.CommandText = @"
            INSERT INTO Platnosci (Id_platnosci, Id_uzytkownika, Id_umowy, Kwota, Data)
            VALUES (@id_platnosci, @id_uzytkownika, @id_umowy, @kwota, @data)
            ";
            insertCmd.Parameters.AddWithValue("@id_platnosci", payment.Id_platnosci);
            insertCmd.Parameters.AddWithValue("@id_uzytkownika", payment.Id_uzytkownika);
            insertCmd.Parameters.AddWithValue("@id_umowy", payment.Id_umowy);
            insertCmd.Parameters.AddWithValue("@kwota", payment.Kwota);
            insertCmd.Parameters.AddWithValue("@data", payment.Data);
            insertCmd.ExecuteNonQuery();
        }
    }

    public static void AddOffer(Offer offer)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var insertCmd = connection.CreateCommand();
            insertCmd.CommandText = @"
            INSERT INTO Oferty (Nazwa, Opis, Cena)
            VALUES (@nazwa, @opis, @cena)
            ";
            insertCmd.Parameters.AddWithValue("@nazwa", offer.Nazwa);
            insertCmd.Parameters.AddWithValue("@opis", offer.Opis);
            insertCmd.Parameters.AddWithValue("@cena", offer.Cena);
            insertCmd.ExecuteNonQuery();
        }
    }

    public static void DeleteOffer(int id_oferty)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var deleteCmd = connection.CreateCommand();
            deleteCmd.CommandText = @"
            DELETE FROM Oferty
            WHERE Id_oferty = @id_oferty
            ";
            deleteCmd.Parameters.AddWithValue("@id_oferty", id_oferty);
            deleteCmd.ExecuteNonQuery();
        }
    }

    public static List<Offer> GetOffers()
    {
        List<Offer> offers = new List<Offer>();
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = @"
            SELECT Id_oferty, Nazwa, Opis, Cena
            FROM Oferty
            ";
            using (var reader = selectCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id_oferty = reader.GetInt32(0);
                    string nazwa = reader.GetString(1);
                    string opis = reader.GetString(2);
                    int cena = reader.GetInt32(3);
                    Offer offer = new Offer(nazwa, opis, cena, id_oferty);
                    offers.Add(offer);
                }
            }
        }
        return offers;
    }

    public static void AddDeal(Deal deal)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var insertCmd = connection.CreateCommand();
            insertCmd.CommandText = @"
            INSERT INTO Umowy (Id_uzytkownika, Id_oferty, Id_adresu, Data_zawarcia, Data_zakonczenia)
            VALUES (@id_uzytkownika, @id_oferty, @id_adresu, @data_zawarcia, @data_zakonczenia)
            ";
            insertCmd.Parameters.AddWithValue("@id_uzytkownika", deal.Id_uzytkownika);
            insertCmd.Parameters.AddWithValue("@id_oferty", deal.Id_oferty);
            insertCmd.Parameters.AddWithValue("@id_adresu", deal.Id_adresu);
            insertCmd.Parameters.AddWithValue("@data_zawarcia", deal.Data_zawarcia);
            insertCmd.Parameters.AddWithValue("@data_zakonczenia", deal.Data_zakonczenia);
            insertCmd.ExecuteNonQuery();
        }
    }
    public static void AddAddress(Address address)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var insertCmd = connection.CreateCommand();
            insertCmd.CommandText = @"
            INSERT INTO Adresy (Kraj, Wojewodztwo, Kod_pocztowy, Miasto, Ulica, Numer_domu, Numer_mieszkania)
            VALUES (@kraj, @wojewodztwo, @kod_pocztowy, @miasto, @ulica, @numer_domu, @numer_mieszkania)
            ";
            insertCmd.Parameters.AddWithValue("@kraj", address.Kraj);
            insertCmd.Parameters.AddWithValue("@wojewodztwo", address.Wojewodztwo);
            insertCmd.Parameters.AddWithValue("@kod_pocztowy", address.Kod_pocztowy);
            insertCmd.Parameters.AddWithValue("@miasto", address.Miasto);
            insertCmd.Parameters.AddWithValue("@ulica", address.Ulica);
            insertCmd.Parameters.AddWithValue("@numer_domu", address.Numer_domu);
            insertCmd.Parameters.AddWithValue("@numer_mieszkania", address.Numer_mieszkania);
            insertCmd.ExecuteNonQuery();
        }
    }
    


}