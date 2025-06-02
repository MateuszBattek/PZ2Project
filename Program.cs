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

bool first_run = File.Exists("Models/baza.db");            // Create tables and admin if first run
if (!first_run)
{
    var connectionStringBuilder = new SqliteConnectionStringBuilder();
    connectionStringBuilder.DataSource = "./Models/baza.db";
    using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
    {
        connection.Open();

        SqliteCommand createTableCmd = connection.CreateCommand();
        createTableCmd.CommandText =
            "CREATE TABLE \"Uzytkownicy\" ("
            + "\"Id_uzytkownika\"	INTEGER PRIMARY KEY AUTOINCREMENT,"
            + "\"Login\"	TEXT NOT NULL,"
            + "\"Haslo\"	TEXT NOT NULL,"
            + "\"Imie\"	TEXT NOT NULL,"
            + "\"Nazwisko\"	TEXT NOT NULL,"
            + "\"Nr_tel\"	TEXT,"
            + "\"Email\"	TEXT NOT NULL,"
            + "\"Rola\"	    TEXT NOT NULL);";
        createTableCmd.ExecuteNonQuery();

        createTableCmd.CommandText =
            "CREATE TABLE \"Adresy\" ("
            + "\"Id_adresu\"	INTEGER PRIMARY KEY AUTOINCREMENT,"
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
            + "\"Id_platnosci\"	INTEGER PRIMARY KEY AUTOINCREMENT,"
            + "\"Id_uzytkownika\"	INTEGER FOREIGN KEY,"
            + "\"Id_oferty\"	INTEGER FOREIGN KEY,"
            + "\"Kwota\"	INTEGER FOREIGN KEY,"
            + "\"Data\" TEXT NOT NULL);";
        createTableCmd.ExecuteNonQuery();

        createTableCmd.CommandText =
            "CREATE TABLE \"Oferty\" ("
            + "\"Id_oferty\"	INTEGER PRIMARY KEY AUTOINCREMENT,"
            + "\"Nazwa\"	TEXT NOT NULL,"
            + "\"Opis\"	TEXT NOT NULL,"
            + "\"Cena\"	INTEGER NOT NULL);";
        createTableCmd.ExecuteNonQuery();

        createTableCmd.CommandText =
            "CREATE TABLE \"Umowy\" ("
            + "\"Id_umowy\"	INTEGER PRIMARY KEY AUTOINCREMENT,"
            + "\"Id_uzytkownika\"	INTEGER FOREIGN KEY,"
            + "\"Id_oferty\"	INTEGER FOREIGN KEY,"
            + "\"Id_adresu\"	INTEGER FOREIGN KEY,"
            + "\"Data_zawarcia\"	TEXT NOT NULL);";
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
        + "(Login, Haslo, Imie, Nazwisko, Email, Rola)"
        + "VALUES (\"admin\", +\"" + admin_hash + "\", \"Jan\", \"Kowalski\", \"admin@change.this\", \"Admin\");";
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
