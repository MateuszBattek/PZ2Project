namespace MvcPracownicy.Models;

using Microsoft.Data.Sqlite;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Metoda dodania uzytkownika, tworzy sie hash SHA256 i zapisuje do bazy
/// </summary>
public class DBModel
{
    private readonly string connectionString = "Models/database.db";
    public void AddUser(string login, string haslo, string imie, string nazwisko, string nr_tel, string email, string rola)
    {
        var connectionStringBuilder = new SqliteConnectionStringBuilder();
        connectionStringBuilder.DataSource = connectionString;
        using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
        {
            string password_hash = "";
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(haslo);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                password_hash = Convert.ToHexString(hashBytes);
            }
            connection.Open();
            SqliteCommand insertCmd = connection.CreateCommand();
            insertCmd.CommandText = @"
                INSERT INTO Uzytkownicy (Login, Haslo, Imie, Nazwisko, Nr_tel, Email, Rola)
                VALUES (@login, @haslo, @imie, @nazwisko, @nrTel, @email, @rola)";
            insertCmd.Parameters.AddWithValue("@login", login);
            insertCmd.Parameters.AddWithValue("@haslo", password_hash);
            insertCmd.Parameters.AddWithValue("@imie", imie);
            insertCmd.Parameters.AddWithValue("@nazwisko", nazwisko);
            insertCmd.Parameters.AddWithValue("@nrTel", nr_tel);
            insertCmd.Parameters.AddWithValue("@email", email);
            insertCmd.Parameters.AddWithValue("@rola", rola);
            insertCmd.ExecuteNonQuery();
        }
    }

    /// <summary>
    /// Sprawdza czy użytkownik się zalogował i jego role
    /// </summary>
    /// <param name="login"></param>
    /// <param name="haslo"></param>
    /// <returns>
    /// 0 - nie ma użytkownika w bazie
    /// 1 - złe hasło
    /// 10 - zalogowany zwykły użytkownik
    /// 20 - zalogowany admin
    /// </returns>
    public int LogIn(string login, string haslo)
    {
        string hashZBazy = "";
        string rola = "";
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = "SELECT Haslo, Rola FROM Uzytkownicy WHERE Login = @login";
            selectCmd.Parameters.AddWithValue("@login", login);
            using (var reader = selectCmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    hashZBazy = reader.GetString(0);
                    rola = reader.GetString(1);
                }
                else
                    return 0; // login nie istnieje
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
            return rola.ToLower() == "admin" ? 20 : 10;
        else
            return 1;

    }
}