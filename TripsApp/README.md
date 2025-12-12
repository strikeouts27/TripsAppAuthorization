# Trips Log

Trips Log is an ASP.NET Core MVC app that demonstrates passing data across requests with view models, ViewBag, ViewData, and TempData while persisting trips in SQLite via Entity Framework Core.

## Features

- Home page lists all saved trips and shows a temporary success message after adding a trip.
- Banner/subhead uses `ViewBag.SubHeader` to show context during the add flow.
- Three-step Add Trip process using view models and TempData to carry values between requests.
  - Step 1: Destination, start date, end date (all required).
  - Step 2: Accommodations (required) plus optional phone and email.
  - Step 3: Optional activities; Save writes all TempData values to the database.
- Cancel on any step clears TempData and returns to Home.
- SQLite database is created and seeded automatically on first run.

## Prerequisites

- .NET 8 SDK

## Run locally

```bash
cd .
dotnet run
```

The app listens on the configured port (see Properties/launchSettings.json). A `trips.db` file is created in the project root if it does not exist.

## Project notes

- Database configured via the `TripsDb` connection string in appsettings.
- EF Core context: `TripsContext` with a single `Trips` table.
- Data models live in `Models/`; add-flow view models are `TripStep1ViewModel`, `TripStep2ViewModel`, and `TripStep3ViewModel`.
