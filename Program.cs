using System.Security.Cryptography;
using Microsoft.Data.Sqlite;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Dodanie obsługo sesji
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(1200);
    options.Cookie.HttpOnly = true;//plik cookie jest niedostępny przez skrypt po stronie klienta
    options.Cookie.IsEssential = true;//pliki cookie sesji będą zapisywane dzięki czemu sesje będzie mogła być śledzona podczas nawigacji lub przeładowania strony
});
//KONIEC

bool first_run = File.Exists("baza.db");            // Create tables and admin if first run
if (!first_run)
{
    var connectionStringBuilder = new SqliteConnectionStringBuilder();
    connectionStringBuilder.DataSource = "./baza.db";
    using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
    {
        connection.Open();

        SqliteCommand createTableCmd = connection.CreateCommand();
        createTableCmd.CommandText =
            "CREATE TABLE \"Uzytkownicy\" ("
            + "\"Id_uzytkownika\"	INTEGER PRIMARY KEY,"
            + "\"Login\"	TEXT PRIMARY KEY,"
            + "\"Haslo\"	TEXT NOT NULL,"
            + "\"Nr_tel\"	TEXT,"
            + "\"Email\"	TEXT NOT NULL,"
            + "\"Rola\"	    TEXT NOT NULL);";
        createTableCmd.ExecuteNonQuery();

        createTableCmd.CommandText =
            "CREATE TABLE \"Dane_kontaktowe\" ("
            + "\"Id_kontaktowe\"	INTEGER PRIMARY KEY,"
            + "\"Id_uzytkownika\"	INTEGER FOREIGN KEY,"
            + "\"Login\"	TEXT PRIMARY KEY,"
            + "\"Haslo\"	TEXT NOT NULL,"
            + "\"Nr_tel\"	TEXT,"
            + "\"Email\"	TEXT NOT NULL,"
            + "\"Rola\"	    TEXT NOT NULL);";
        createTableCmd.ExecuteNonQuery();

        createTableCmd.CommandText =
            "CREATE TABLE \"Adresy\" ("
            + "\"Id_adresu\"	INTEGER PRIMARY KEY,"
            + "\"Id_uzytkownika\"	    INTEGER FOREIGN KEY,"
            + "\"Kraj\"	    TEXT NOT NULL,"
            + "\"Wojewodztwo\"	    TEXT NOT NULL,"
            + "\"Kod_pocztowy\"	    TEXT NOT NULL,"
            + "\"Miasto\"	TEXT NOT NULL,"
            + "\"Ulica\"	TEXT NOT NULL,"
            + "\"Numer_domu\"	TEXT NOT NULL,"
            + "\"Numer_mieszkania\"	TEXT);";
        createTableCmd.ExecuteNonQuery();

        createTableCmd.CommandText =
            "CREATE TABLE \"Platnosci\" ("
            + "\"Id_platnosci\"	INTEGER PRIMARY KEY,"
            + "\"Id_uzytkownika\"	INTEGER FOREIGN KEY,"
            + "\"Id_oferty\"	INTEGER FOREIGN KEY,"
            + "\"Data\" TEXT NOT NULL);";
        createTableCmd.ExecuteNonQuery();

        createTableCmd.CommandText =
            "CREATE TABLE \"Oferty\" ("
            + "\"Id_oferty\"	INTEGER PRIMARY KEY,"
            + "\"Nazwa\"	TEXT NOT NULL,"
            + "\"Opis\"	TEXT NOT NULL,"
            + "\"Cena\"	REAL NOT NULL);";
        createTableCmd.ExecuteNonQuery();

        createTableCmd.CommandText =
            "CREATE TABLE \"Rabaty\" ("
            + "\"Id_rabatu\"	INTEGER PRIMARY KEY,"
            + "\"Lata\"	TEXT NOT NULL,"
            + "\"rabat\"	REAL NOT NULL);";
        createTableCmd.ExecuteNonQuery();

        string admin_hash = "";
        using (MD5 md5 = MD5.Create())
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes("admin");
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            admin_hash = Convert.ToHexString(hashBytes);
        }

        SqliteCommand insertCmd = connection.CreateCommand();
        insertCmd.CommandText =
        "INSERT INTO Uzytkownicy"
        + "(Login, Haslo)"
        + "VALUES (\"admin\", +\"" + admin_hash + "\");";
        insertCmd.ExecuteNonQuery();

        insertCmd = connection.CreateCommand();
        insertCmd.CommandText =
        "INSERT INTO Dane"
        + "(Pesel, Imie, Nazwisko)"
        + "VALUES (\"12345678901\", \"Jan\", \"Kowalski\"),"
        + "(\"22345678901\", \"Jan\", \"Kowalski\");";
        insertCmd.ExecuteNonQuery();
    }
}


var app = builder.Build();

// app.Use(async (ctx, next) =>
// {
//     await next();
//     if(ctx.Response.StatusCode == 404 && !ctx.Response.HasStarted)
//     {
//         ctx.Response.Redirect("/IO/Logowanie");
//     }
// });

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

//Dodanie obsługo sesji
app.UseSession();
//KONIEC

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



app.Run();
