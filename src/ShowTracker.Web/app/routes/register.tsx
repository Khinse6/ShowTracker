// src/routes/register.tsx
import { useState } from "react";
import { Link, useNavigate } from "react-router";
import { register, saveAuthResponse } from "~/api/auth";

export default function Register() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [displayName, setDisplayName] = useState("");
  const [acceptedTerms, setAcceptedTerms] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  const navigate = useNavigate();

	const handleSubmit = async (e: React.FormEvent) => {
		e.preventDefault();
		setError(null);
		if (password !== confirmPassword) {
			setError("Passwords do not match.");
			return;
		}

    const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/;
    if (!passwordRegex.test(password)) {
      setError("Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one number, and one special character.");
      return;
    }

		if (!acceptedTerms) {
			setError("You must accept the terms and conditions to register.");
			return;
    }

    setLoading(true);

        try {
          const authResponse = await register({ email, password, displayName, acceptedTerms });
          if (authResponse.user && authResponse.user.id) {
            saveAuthResponse(authResponse);
            navigate("/");
          } else {
            console.error("Registration failed: No user ID returned");
            setError("Invalid credentials, please try again.");
            const errorMessage = "An unknown registration error occurred.";
            console.error("Registration failed:", errorMessage);
            setError(errorMessage);
          }
        } catch (err: any) {
          console.error(err);
          setError("Invalid credentials, please try again.");
          if (err.response && err.response.data) {
            const errorData = err.response.data;
            // Handle ASP.NET Core Identity validation errors
            if (errorData.errors) {
              setError(Object.values(errorData.errors).flat().join(" "));
            } else {
              setError(errorData.message || errorData.detail || errorData.title || "An unknown registration error occurred.");
            }
          } else {
            setError("An unknown registration error occurred. Please try again.");
          }
        } finally {
          setLoading(false);
        }
      };

  return (
    <div className="max-w-md mx-auto mt-20 p-6 border rounded shadow">
      <h1 className="text-2xl font-bold mb-4">Register</h1>
      {error && <p className="text-red-500 mb-4">{error}</p>}
      <form onSubmit={handleSubmit} className="space-y-4">
        <div>
          <label htmlFor="displayName" className="block mb-1 font-medium">
            Display Name
          </label>
          <input
            id="displayName"
            type="text"
            value={displayName}
            onChange={(e) => setDisplayName(e.target.value)}
            required
            className="w-full border px-3 py-2 rounded"
          />
        </div>
        <div>
          <label htmlFor="email" className="block mb-1 font-medium">
            Email
          </label>
          <input
            id="email"
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            autoComplete="email"
            required
            className="w-full border px-3 py-2 rounded"
          />
        </div>
        <div>
          <label htmlFor="password" className="block mb-1 font-medium">
            Password
          </label>
          <input
            id="password"
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            autoComplete="new-password"
            required
            className="w-full border px-3 py-2 rounded"
          />
        </div>
        <div>
          <label htmlFor="confirmPassword" className="block mb-1 font-medium">
            Confirm Password
          </label>
          <input
            id="confirmPassword"
            type="password"
            value={confirmPassword}
            onChange={(e) => setConfirmPassword(e.target.value)}
            autoComplete="new-password"
            required
            className="w-full border px-3 py-2 rounded"
          />
        </div>
        <div className="flex items-center">
          <input id="acceptedTerms" type="checkbox" checked={acceptedTerms} onChange={(e) => setAcceptedTerms(e.target.checked)} required className="h-4 w-4 text-blue-600 border-gray-300 rounded focus:ring-blue-500" />
          <label htmlFor="acceptedTerms" className="ml-2 block text-sm text-gray-900">
            I accept the terms and conditions
          </label>
        </div>
        <button type="submit" disabled={loading} className="w-full bg-blue-600 text-white py-2 px-4 rounded hover:bg-blue-700">
          {loading ? "Registering..." : "Register"}
        </button>
      </form>
      <div className="mt-4 text-center">
        <p>
          Already have an account?{" "}
          <Link to="/login" className="text-blue-600 hover:underline">
            Login
          </Link>
        </p>
      </div>
    </div>
  );
}