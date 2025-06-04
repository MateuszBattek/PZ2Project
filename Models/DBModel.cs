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
    public void AddUser(User user, string password)
    {
        var connectionStringBuilder = new SqliteConnectionStringBuilder();
        connectionStringBuilder.DataSource = connectionString;
        using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
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

    /// <summary>
    /// Usuwa uzytkownika
    /// </summary>
    /// <param name="login"></param>
    public void DeleteUser(string login)
    {
        var connectionStringBuilder = new SqliteConnectionStringBuilder();
        connectionStringBuilder.DataSource = connectionString;
        using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
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
    public User? LogIn(string login, string haslo)
    {
        string hashZBazy = "";
        User? user = null;
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = "SELECT Haslo, Imie, Nazwisko, Nr_tel, Email, Rola FROM Uzytkownicy WHERE Login = @login";
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
                    user = new User(login, imie, nazwisko, nr_tel, email, rola);
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

}