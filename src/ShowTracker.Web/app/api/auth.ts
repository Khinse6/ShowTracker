import api from "./client";
import type {
	AuthResponse,
	LoginRequest,
	RefreshResponse,
	RegisterRequest,
	User
} from "../types/auth";

const isBrowser = typeof window !== "undefined";

// Login: stores tokens and user info in localStorage
export async function login(data: LoginRequest): Promise<AuthResponse> {
	const response = await api.post<AuthResponse>("/auth/login", data);
	return response.data;
}

// This function should only be called on the client
export function saveAuthResponse(authResponse: AuthResponse): void {
	const { accessToken, refreshToken, user } = authResponse;
	if (isBrowser) {
		localStorage.setItem("accessToken", accessToken);
		localStorage.setItem("refreshToken", refreshToken);
		localStorage.setItem("user", JSON.stringify(user));
	}
}

// Refresh: updates access token in localStorage
export async function refresh(): Promise<string> {
	const refreshToken = isBrowser ? localStorage.getItem("refreshToken") : null;
	const response = await api.post<RefreshResponse>("/auth/refresh", {
		refreshToken
	});
	const { accessToken, refreshToken: newRefreshToken } = response.data;
	if (isBrowser) {
		localStorage.setItem("accessToken", accessToken);
		localStorage.setItem("refreshToken", newRefreshToken);
	}
	return accessToken;
}

// Register: registers user and stores tokens/user info
export async function register(data: RegisterRequest): Promise<AuthResponse> {
	const response = await api.post<AuthResponse>("/auth/register", data);
	const { accessToken, refreshToken, user } = response.data;
	if (isBrowser) {
		localStorage.setItem("accessToken", accessToken);
		localStorage.setItem("refreshToken", refreshToken);
		localStorage.setItem("user", JSON.stringify(user));
	}
	return response.data;
}

// Logout: removes tokens and user info from localStorage
export function logout(): void {
	if (isBrowser) {
		localStorage.removeItem("accessToken");
		localStorage.removeItem("refreshToken");
		localStorage.removeItem("user");
	}
}

// Returns the current user from localStorage, or null if not found
export function getCurrentUser(): User | null {
	if (!isBrowser) {
		return null;
	}
	const userJson = localStorage.getItem("user");
	if (!userJson) return null;
	try {
		return JSON.parse(userJson) as User;
	} catch {
		return null;
	}
}
