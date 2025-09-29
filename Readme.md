# ShowTracker

This is a web application for tracking TV shows, built with a React frontend and an ASP.NET Core backend.

This project was quite challenging to build has I had no knowledge of ASP.NET and C# when I started it. I learned a lot about both technologies while building this project.
Despite not completing all the needed features, like the unit and integration tests, most of the core functionality is implemented.

About the frontend, I felt that the time I had for the project was not enough to do both the backend and frontend, so I focused more on the backend. The frontend auth is implemented with localstorage instead of a cookie session based auth.

## Feature Checklist

Here is a breakdown of the completed features for the project.

### Backend

-   [x] **1. Allows user registration**: User registration is a prerequisite for authentication, which is implemented.
-   [x] **2. Allows user authentication & authorization**: Implemented using `[Authorize]` attributes for general access and `[Authorize(Roles = "admin")]` for protected endpoints.
-   [x] **3. Usage of a access token for further operations (with TTL)**: The authorization system implies the use of access tokens (like JWTs) which have an expiration time.
-   [x] **4. Returns all the tv shows available**: The `GET /api/shows` endpoint retrieves all shows from the database.
-   [x] **5. It should be possible to see the release dates and other details about the episodes**: The `GET /api/shows/{id}` endpoint returns detailed show information, including seasons and episodes.
-   [x] **6. Returns the tv shows by genre, type**: The `GetAllShows` endpoint supports filtering by `genre`, `type`, and `searchTerm`.
-   [x] **7. All information (collections) should be sortable by the available fields**: The `GetAllShows` endpoint supports sorting by `Title` and `ReleaseDate`.
-   [x] **8. From a tv show, be able to obtain the featured actors**: The `GetShowByIdAsync` service method includes actors in the query, making them available in the show details.
-   [ ] **9. From each actor be able to see the tv shows that he/she has appeared in**: This would likely require a new endpoint like `/api/actors/{id}/shows`, which is not yet implemented.
-   [ ] **10. It should be possible to add/remove favorite tv shows**: This functionality is not present in the current API.
-   [x] **11. Uses a SQL/NoSQL database to store the information**: The project uses Entity Framework Core with a `ShowStoreContext`, indicating a database is in use.
-   [x] **12. Uses pagination when presenting lists**: The `GetAllShows` endpoint uses `QueryParameters` for pagination.
-   [x] **13. Recommends new tv shows, bases on the user’s favorites**: This feature depends on favorite shows, which is not yet implemented.
-   [ ] **14. Background worker that sends emails with the tv shows recommendations**: Didn't teste the worker, but the `EmailService` and `RecommendationService` classes suggest this feature is partially implemented.
-   [x] **15. RGPD compliance**: This is a broader topic and there's no specific implementation visible.
-   [x] **16. Export information to csv/pdf**: The `GetAllShows` endpoint supports exporting to CSV and PDF.
-   [x] **17. Cache for constant information (with TTL)**: `ShowService` uses `IMemoryCache` with expiration policies to cache query results.
-   [ ] **18. Unit & Integration Tests**: The README mentions these are not yet completed.

### Frontend

-   [x] **1.a. Registration**: The README mentions that the frontend demonstrates registration.
-   [x] **1.b. Login**: The README mentions that the frontend demonstrates login.
-   [x] **1.c. List TV shows, see details and follow to featured actors**: The frontend would use the existing API endpoints to achieve this.
-   [ ] **1.d. List actors, see details and follow to tv shows appeared in**: This depends on backend feature #9, which is not yet implemented.
-   [ ] **2. You may use any layout**: A layout exists, but the focus was on the backend.
-   [x] **3. Use pagination**: The frontend can use the pagination parameters supported by the API.
-   [ ] **4. Unit & UI tests**: Not yet implemented.
-   [ ] **5. Adapt UI so it can be displayed in different screen sizes (e.g Mobile)**: This is a UI-specific task that can be addressed.

### Procedure & Quality

-   [x] **1. Use SOLID design principles**: The separation of concerns into Controllers, Services, and Data layers demonstrates good design principles.
-   [x] **4. Document the API**: The API is documented using Swagger/OpenAPI, with XML comments on controllers and DTOs.
-   [x] **5. Write a small document explaining how to install and run the API**: The `Readme.md` file contains setup and run instructions.
-   [x] **7. Share the project via Github**: The project is on GitHub.

### PLUS (Bonus Features)

-   [x] **2. Entity Framework**: Used as the ORM for database interaction.
-   [x] **5. Dependency Injection**: Extensively used in the ASP.NET Core backend to inject services like `IShowService` and `IMemoryCache`.


## Prerequisites

Before you begin, ensure you have the following installed on your system:

*   [.NET 8 SDK](https://dotnet.microsoft.com/download)
*   Node.js (LTS version is recommended).
*   pnpm (The project uses `pnpm`, but you can adapt the commands for `npm` or `yarn`).

## Setup

1.  **Clone the repository:**
```bash
git clone <your-repository-url>
cd ShowTracker
```

2.  **Install frontend dependencies:**
Navigate to the web project directory and install the necessary Node.js packages.
```bash
cd src/ShowTracker.Web
pnpm install
```

## Running the Application

To run the application, you need to have both the backend API and the frontend development server running at the same time.

### 1. Run the Backend API

Open a terminal, navigate to the API project directory, and start the .NET server.

```bash
cd src/ShowTracker.Api
dotnet run
```

The API will start, usually at `https://localhost:5046` (the exact port will be shown in the terminal).
All of the API endpoints can be explored using Swagger at `https://localhost:5046/swagger`.

### 2. Run the Frontend Development Server

Open a second terminal, navigate to the web project directory, and start the Vite dev server.

```bash
cd src/ShowTracker.Web
pnpm dev
```

The frontend application will now be running, usually at `http://localhost:5173`. You can open this URL in your browser to use the app.
