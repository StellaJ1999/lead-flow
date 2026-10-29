import {
  api,
  ApiError,
  ApiNetworkError,
  clearCsrfToken,
  refreshCsrfToken,
} from "@/lib/api";

export type CurrentUser = {
  id: string;
  email: string;
};

export class InvalidCredentialsError extends Error {
  constructor() {
    super("Felaktig e-postadress eller lösenord.");
    this.name = "InvalidCredentialsError";
  }
}

export function getCurrentUser(): Promise<CurrentUser> {
  return api<CurrentUser>("/api/auth/me");
}

export async function login(
  email: string,
  password: string,
): Promise<CurrentUser> {
  await refreshCsrfToken();

  try {
    await api("/api/auth/login", {
      method: "POST",
      body: {
        email: email.trim(),
        password,
      },
    });
  } catch (error) {
    if (error instanceof ApiError && error.status === 401) {
      throw new InvalidCredentialsError();
    }

    throw error;
  }

  // Användaridentiteten har ändrats efter lyckad login.
  await refreshCsrfToken();

  return getCurrentUser();
}

export async function logout(): Promise<void> {
  // Hämta en aktuell token innan sessionen avslutas.
  await refreshCsrfToken();

  await api("/api/auth/logout", {
    method: "POST",
  });

  clearCsrfToken();
}

export function getAuthErrorMessage(
  error: unknown,
  fallback = "Inloggningen kunde inte genomföras. Försök igen senare.",
): string {
  if (error instanceof InvalidCredentialsError) {
    return error.message;
  }

  if (
    error instanceof ApiNetworkError ||
    (error instanceof ApiError &&
      [502, 503, 504].includes(error.status))
  ) {
    return "Det gick inte att ansluta. Försök igen.";
  }

  return fallback;
}