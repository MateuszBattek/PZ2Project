namespace MvcPracownicy.Models;

using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Diagnostics.HealthChecks;
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
            insertCmd.CommandText =
    "INSERT INTO Uzytkownicy (Login, Haslo, Imie, Nazwisko, Nr_tel, Email, Rola) " +
    "VALUES (@Login, @Haslo, @Imie, @Nazwisko, @Nr_tel, @Email, @Rola);";

            insertCmd.Parameters.AddWithValue("@Login", user.Login);
            insertCmd.Parameters.AddWithValue("@Haslo", password_hash);
            insertCmd.Parameters.AddWithValue("@Imie", user.Imie);
            insertCmd.Parameters.AddWithValue("@Nazwisko", user.Nazwisko);
            insertCmd.Parameters.AddWithValue("@Nr_tel", user.Nr_tel);
            insertCmd.Parameters.AddWithValue("@Email", user.Email);
            insertCmd.Parameters.AddWithValue("@Rola", user.Rola);

            insertCmd.ExecuteNonQuery();
        }
    }

    public static void ChangeUserData(User user, int id_uzytkownika)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            SqliteCommand insertCmd = connection.CreateCommand();
            insertCmd.CommandText =
    "UPDATE Uzytkownicy SET Imie=@Imie, Nazwisko=@Nazwisko, Nr_tel=@Nr_tel, Email=@Email " +
    "WHERE (Id_uzytkownika=@Id_uzytkownika);";

            insertCmd.Parameters.AddWithValue("@Imie", user.Imie);
            insertCmd.Parameters.AddWithValue("@Nazwisko", user.Nazwisko);
            insertCmd.Parameters.AddWithValue("@Nr_tel", user.Nr_tel);
            insertCmd.Parameters.AddWithValue("@Email", user.Email);
            insertCmd.Parameters.AddWithValue("@Id_uzytkownika", id_uzytkownika);

            Console.WriteLine(user.Imie + " " + user.Nazwisko + " " + user.Nr_tel + " " + user.Email + " " + id_uzytkownika);

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
    public static Dictionary<int, int> GetUserDebt(int Id_uzytkownika)
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
                (strftime('%m', 'now') - strftime('%m', U.Data_zawarcia))) * O.Cena AS Suma
            FROM Umowy U
            JOIN Oferty O on U.Id_oferty = O.Id_oferty
            JOIN Uzytkownicy U2 on U.Id_uzytkownika = U2.Id_uzytkownika
            WHERE U2.Id_uzytkownika = @user_id and U.Data_zakonczenia IS NULL
            GROUP BY U.Id_umowy
            ";
            selectCmd.Parameters.AddWithValue("@user_id", Id_uzytkownika);

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
            WHERE U2.Id_uzytkownika = @user_id and U.Data_zakonczenia IS NULL
            GROUP BY U.Id_umowy
            ";

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



    /// <summary>
    /// Liczy długów uzytkowników
    /// </summary>
    /// <returns>
    /// Metoda zwraca Dictionary<int, int> z id_uzytkownika jako klucz i kwotą jako wartość
    /// </returns>
    public static Dictionary<int, int> GetUsersDebt()
    {
        Dictionary<int, int> user_to_debt = new Dictionary<int, int>();
        List<User> users = GetUsers();
        foreach (User user in users)
        {
            Dictionary<int, int> deal_to_debt = GetUserDebt(user.Id_uzytkownika ?? 0);
            int user_debt = 0;
            foreach (int value in deal_to_debt.Values)
                user_debt += value;
            if (user_debt > 0)
                user_to_debt.Add(user.Id_uzytkownika!.Value, user_debt);
        }
        return user_to_debt;
    }

    public static List<User> GetUsersWithDebt()
    {
        List<User> users = GetUsers();
        List<User> users_with_debt = new List<User>();
        foreach (User user in users)
        {
            Dictionary<int, int> deal_to_debt = GetUserDebt(user.Id_uzytkownika ?? 0);
            int user_debt = 0;
            foreach (int value in deal_to_debt.Values)
                user_debt += value;
            if (user_debt > 0)
            {
                user.Debt = user_debt;
                users_with_debt.Add(user);
            }
        }
        return users_with_debt;
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

    public static List<Deal> GetUserDeals(int Id_uzytkownika)
    {
        List<Deal> deals = new List<Deal>();
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = @"
            SELECT Id_umowy, Id_oferty, Id_adresu, Data_zawarcia, Data_zakonczenia
            FROM Umowy
            WHERE Id_uzytkownika = @user_id
        ";
            selectCmd.Parameters.AddWithValue("@user_id", Id_uzytkownika);
            using (var reader = selectCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id_umowy = reader.GetInt32(0);
                    // int id_uzytkownika = reader.GetInt32(1); // To jest ID użytkownika, które już masz
                    int id_oferty = reader.GetInt32(1); // Kolumna 1 to Id_oferty
                    int id_adresu = reader.GetInt32(2); // Kolumna 2 to Id_adresu

                    // Poprawne parsowanie dat
                    DateTime data_zawarcia = reader.GetDateTime(3); // Jeśli to data, lepiej GetDateTime

                    DateTime? data_zakonczenia = null; // Użyj DateTime? (Nullable DateTime)

                    // Sprawdź, czy wartość w kolumnie Data_zakonczenia jest NULL w bazie
                    if (!reader.IsDBNull(4)) // Sprawdź, czy kolumna o indeksie 4 (Data_zakonczenia) NIE jest NULL
                    {
                        data_zakonczenia = reader.GetDateTime(4); // Jeśli nie jest NULL, pobierz jako DateTime
                    }

                    // Utwórz obiekt Deal. Pamiętaj, że konstruktor Deal musi przyjmować DateTime?
                    // Jeśli konstruktor Deal wymaga DateTime, zdecyduj, co ma być domyślną wartością dla NULL (np. DateTime.MinValue)
                    Deal deal = new Deal(
                        Id_uzytkownika,
                        id_oferty,
                        id_adresu,
                        data_zawarcia,
                        id_umowy,
                        data_zakonczenia // Tutaj przekazujesz nullable DateTime?
                    );
                    deals.Add(deal);
                }
            }
        }
        Console.WriteLine(deals);
        return deals;
    }

    public static void ChangeUserData(int user_id, User user)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var updateCmd = connection.CreateCommand();
            updateCmd.CommandText = @"
            UPDATE Uzytkownicy
            SET Nr_tel = @nr_tel, Email = @email
            WHERE Id_uzytkownika = @user_id
            ";
            updateCmd.Parameters.AddWithValue("@nr_tel", user.Nr_tel);
            updateCmd.Parameters.AddWithValue("@email", user.Email);
            updateCmd.Parameters.AddWithValue("@user_id", user_id);
            updateCmd.ExecuteNonQuery();
        }
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
            insertCmd.Parameters.AddWithValue("@data", DateTime.Now.ToString("yyyy-MM-dd"));

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

    public static List<Address> GetAddresses()
    {
        List<Address> addresses = new List<Address>();
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = @"
            SELECT Id_adresu, Kraj, Wojewodztwo, Kod_pocztowy, Miasto, Ulica, Numer_domu, Numer_mieszkania
            FROM Adresy
            ";
            using (var reader = selectCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id_adresu = reader.GetInt32(0);
                    string kraj = reader.GetString(1);
                    string wojewodztwo = reader.GetString(2);
                    string kod_pocztowy = reader.GetString(3);
                    string miasto = reader.GetString(4);
                    string ulica = reader.GetString(5);
                    string numer_domu = reader.GetString(6);
                    string numer_mieszkania = reader.IsDBNull(7) ? "-" : reader.GetString(7);
                    Address address = new Address(kraj, wojewodztwo, kod_pocztowy, miasto, ulica, numer_domu, numer_mieszkania, id_adresu);
                    addresses.Add(address);
                }
            }
        }
        return addresses;
    }

    public static List<Deal> GetDeals()
    {
        List<Deal> deals = new List<Deal>();
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = @"
            SELECT Id_umowy, Id_uzytkownika, Id_oferty, Id_adresu, Data_zawarcia, Data_zakonczenia
            FROM Umowy
            ";
            using (var reader = selectCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id_umowy = reader.GetInt32(0);
                    int id_uzytkownika = reader.GetInt32(1);
                    int id_oferty = reader.GetInt32(2);
                    int id_adresu = reader.GetInt32(3);
                    DateTime data_zawarcia = DateTime.Parse(reader.GetString(4));
                    DateTime? data_zakonczenia = reader.IsDBNull(5) ? null : reader.GetDateTime(5);
                    Deal deal = new Deal(id_uzytkownika, id_oferty, id_adresu, data_zawarcia, id_umowy, data_zakonczenia);
                    deals.Add(deal);
                }
            }
        }
        return deals;
    }

    public static List<Payment> GetPayments()
    {
        List<Payment> payments = new List<Payment>();
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = @"
            SELECT Id_platnosci, Id_uzytkownika, Id_umowy, Kwota, Data
            FROM Platnosci
            ";
            using (var reader = selectCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id_platnosci = reader.GetInt32(0);
                    int id_uzytkownika = reader.GetInt32(1);
                    int id_umowy = reader.GetInt32(2);
                    int kwota = reader.GetInt32(3);
                    DateTime data = DateTime.Parse(reader.GetString(4));
                    Payment payment = new Payment(id_uzytkownika, id_umowy, kwota, data, id_platnosci);
                    payments.Add(payment);
                }
            }
        }
        return payments;
    }



    public static void AddDeal(Deal deal)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var insertCmd = connection.CreateCommand();
            insertCmd.CommandText = @"
            INSERT INTO Umowy (Id_uzytkownika, Id_oferty, Id_adresu, Data_zawarcia)
            VALUES (@id_uzytkownika, @id_oferty, @id_adresu, @data_zawarcia)
            ";
            insertCmd.Parameters.AddWithValue("@id_uzytkownika", deal.Id_uzytkownika);
            insertCmd.Parameters.AddWithValue("@id_oferty", deal.Id_oferty);
            insertCmd.Parameters.AddWithValue("@id_adresu", deal.Id_adresu);
            insertCmd.Parameters.AddWithValue("@data_zawarcia", deal.Data_zawarcia.ToString("yyyy-MM-dd"));
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

    public static void EndDeal(int deal_id)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var updateCmd = connection.CreateCommand();
            updateCmd.CommandText = @"
            UPDATE Umowy
            SET Data_zakonczenia = @data_zakonczenia
            WHERE Id_umowy = @id_umowy
            ";
            updateCmd.Parameters.AddWithValue("@data_zakonczenia", DateTime.Now.ToString("yyyy-MM-dd"));
            updateCmd.Parameters.AddWithValue("@id_umowy", deal_id);
            updateCmd.ExecuteNonQuery();
        }
    }



}