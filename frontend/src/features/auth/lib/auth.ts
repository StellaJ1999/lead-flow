import {
  api,
  clearCsrfToken,
  refreshCsrfToken,
} from "../lib/api";

export type CurrentUser = {
  id: string;
  email: string;
};

export function getCurrentUser(): Promise<CurrentUser> {
  return api<CurrentUser>("/api/auth/me");
}

export async function login(
  email: string,
  password: string,
): Promise<void> {
  // Login ska använda en token för den nuvarande identiteten.
  await refreshCsrfToken();

  await api("/api/auth/login", {
    method: "POST",
    body: { email, password },
  });

  // Efter login har identiteten ändrats.
  await refreshCsrfToken();
}

export async function logout(): Promise<void> {
  await api("/api/auth/logout", {
    method: "POST",
  });

  clearCsrfToken();
}