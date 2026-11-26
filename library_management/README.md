# Library Management

## Database setup

The app now uses PostgreSQL through Npgsql. Install PostgreSQL and create an empty database named `library_management`. Copy `.env.example` to `.env` in the project folder and put your PostgreSQL username and password in the connection string:

```text
LIBRARY_DB_CONNECTION=Host=localhost;Port=5432;Database=library_management;Username=YOUR_USERNAME;Password=YOUR_PASSWORD
```

The app loads `.env` when it starts. The real `.env` file is ignored by Git; commit `.env.example` only. If Visual Studio is already open, restart it after creating `.env` so the file is copied to the output folder.

Restore NuGet packages in Visual Studio, or run `nuget restore library_management.sln` from the repository root before building. The package cache is excluded from Git.

The app creates the tables and indexes when it starts. It does not create the PostgreSQL database itself. The available tables are `users`, `authors`, `categories`, `books`, `members`, and `loans`.

If you use a different database name or account, update the connection string. Never commit a real password into source control.

## Features

- User registration with salted PBKDF2 password hashes and login.
- Create, update, delete, list, and search authors and categories.
- Create, update, delete, list, and search books, with author, category, description, publish date, and copy count.
- Register borrowers, check books out with due dates, view loan history, and return books.
- Checkout checks the number of copies and prevents borrowing when none are available.

Existing records in the old MongoDB database are not copied automatically. Export and import those records separately before switching if you need to keep them.
