# ShowTracker

This is a web application for tracking TV shows, built with a React frontend and an ASP.NET Core backend.

## Prerequisites

Before you begin, ensure you have the following installed on your system:

*   [.NET SDK](https://dotnet.microsoft.com/download) (Check the version in the `.csproj` files, but a recent version like .NET 8 is recommended).
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

The API will start, typically on a URL like `https://localhost:7...` or `http://localhost:5...`. Check the terminal output for the exact address.

### 2. Run the Frontend Development Server

Open a second terminal, navigate to the web project directory, and start the Vite dev server.

```bash
cd src/ShowTracker.Web
pnpm dev
```

The frontend application will now be running, usually at `http://localhost:5173`. You can open this URL in your browser to use the app.

## Available Frontend Scripts

Inside the `src/ShowTracker.Web` directory, you can use the following scripts:

*   `pnpm dev`: Starts the development server with Hot Module Replacement (HMR).
*   `pnpm build`: Compiles and bundles the application for production.
*   `pnpm start`: Serves the production-built application.
*   `pnpm typecheck`: Runs the TypeScript compiler and React Router's type generator to check for type errors.
