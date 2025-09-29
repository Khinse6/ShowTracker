import { useEffect, useMemo } from "react";
import { useNavigate } from "react-router";
import { getCurrentUser, logout as apiLogout } from "~/api/auth";

export function useAuth() {
	const navigate = useNavigate();
	const user = useMemo(() => getCurrentUser(), []);

	useEffect(() => {
		if (!user) {
			navigate("/login", { replace: true });
		}
	}, [user, navigate]);

	const logout = () => {
		apiLogout();
		navigate("/login", { replace: true });
	};

	return { user, logout };
}
