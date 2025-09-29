import axios from "axios";

const api = axios.create({
	baseURL: "http://localhost:5046/api",
	headers: { "Content-Type": "application/json" }
});

const isBrowser = typeof window !== "undefined";

// Interceptor to attach the Authorization header if token exists
api.interceptors.request.use(
	function (config) {
		// Only access localStorage in the browser
		const token = isBrowser ? localStorage.getItem("accessToken") : null;
		if (token) {
			config.headers.Authorization = `Bearer ${token}`;
		}
		return config;
	},
	function (error) {
		return Promise.reject(error);
	}
);

export default api;
