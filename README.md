# System płatności za internet - 

Baza danych:

Uzytkownicy - Id_uzytkownika, Login, Haslo, Imie, Nazwisko, Nr_tel, Email, Rola
Adresy - Id_adresu, Kraj, Wojewodztwo, Kod_pocztowy, Miasto, Ulica, Numer_domu, Numer_mieszkania
Platnosci - Id_platnosci, Id_uzytkownika, Id_oferty, Kwota, Data
Oferty - Id_oferty, Nazwa, Opis, Cena
Umowy - Id_umowy, Id_uzytkownika, Id_oferty, Id_adresu, Data_zawarcia

platnosci za internet, podczas platnosci analiza tabeli platnosci
(rabat jako osobne oferty)

whcodzi user na stronke i widzi stan swoich platnosci i za co zaplacil

jak nie zaplacil to ma duzy czerwone okienko, które mu kaze zaplacic

opcja platnosci na stronie
jak placisz od x lat to masz taniej

admin widzi wszystkie dane

podsumowanie wszystkich platnosci


// lekko zmieniłem baze
// adres jest teraz "połączony" z ofertą, żeby można było zawierać oferty na dane adresy

zakładamy, że użytkownik nie zawiera sam umowy

TODO:

controller/widok dodawania/usuwania użytkownika (na jakiejś liście uzytkowników) 
controller/widok dodawania/usuwania ofert (zeby były pola w formularzu na wpisanie danych i pod spodem wypisane oferty)
zmienić logowanie żeby pasowało do naszej bazy
controller/widok zmiany hasła/emailu/itd użytkownika
controller/widok wpłata pieniędzy

w modelu:
dodawanie, usuwanie uzytkownika - gotowe
zrobić funkcje zwracającą ile musi zapłacić użytkownik
zrobić funkcje zwracającą użytkowników, krórzy nie zapłacili
zrobić funkcję wpłaty       // podajemy jako argument kto i ilość
logowanie - gotowe
dodanie oferty
zrobić klasy dla każdej tabeli z bazy



todo niekompletne, zabralem sie za to za późno ¯\_(ツ)_/¯





