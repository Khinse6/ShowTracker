export interface User {
	id: string;
	email?: string;
	displayName?: string;
}

export interface AuthResponse {
	accessToken: string;
	refreshToken: string;
	user: User;
}

export interface LoginRequest {
	email: string;
	password: string;
}

export interface RegisterRequest {
	email: string;
	password: string;
	displayName: string;
	acceptedTerms: boolean;
}

export interface RefreshResponse {
	accessToken: string;
	refreshToken: string;
}
