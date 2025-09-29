// src/routes/Home.tsx

import { useAuth } from "~/hooks/useAuth";

export default function Home() {
  const { user, logout: handleLogout } = useAuth();

  if (!user) {
    return null;
  }

  return (
    <div className="max-w-md mx-auto mt-20 p-6 border rounded shadow">
      <h1 className="text-2xl font-bold mb-4">Welcome, {user.displayName}</h1>
      <p className="mb-4">You are logged in with email: {user.email}</p>
      <button
        onClick={handleLogout}
        className="bg-red-500 text-white px-4 py-2 rounded hover:bg-red-600"
      >
        Logout
      </button>
    </div>
  );
}
