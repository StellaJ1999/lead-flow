type ApiOptions = Omit<RequestInit, "body" | "credentials"> & {
  body?: unknown;
};

type ApiProblem = {
  message?: string;
  title?: string;
  errors?: Record<string, string[]>;
};

export class ApiError extends Error {
  constructor(
    public readonly status: number,
    message: string,
  ) {
    super(message);
    this.name = "ApiError";
  }
}

let csrfToken: string | undefined;

export function clearCsrfToken(): void {
  csrfToken = undefined;
}

export async function refreshCsrfToken(): Promise<string> {
  clearCsrfToken();

  const result = await api<{ token: string }>("/api/auth/csrf");

  if (!result.token) {
    throw new Error("Servern returnerade ingen CSRF-token.");
  }

  csrfToken = result.token;

  return csrfToken;
}

async function getCsrfToken(): Promise<string> {
  return csrfToken ?? refreshCsrfToken();
}

export async function api<T = void>(
  path: string,
  options: ApiOptions = {},
): Promise<T> {
  if (!path.startsWith("/api/") || /[\\\r\n]/.test(path)) {
    throw new Error("Använd en relativ API-adress som börjar med /api/.");
  }

  const { body, ...requestOptions } = options;

  const method = (requestOptions.method ?? "GET").toUpperCase();
  const headers = new Headers(requestOptions.headers);

  headers.set("Accept", "application/json");

  if (body !== undefined) {
    headers.set("Content-Type", "application/json");
  }

  const changesData = !["GET", "HEAD", "OPTIONS"].includes(method);

  if (changesData) {
    headers.set("X-CSRF-TOKEN", await getCsrfToken());
  }

  const response = await fetch(path, {
    ...requestOptions,
    method,
    headers,
    credentials: "same-origin",
    cache: "no-store",
    body: body === undefined ? undefined : JSON.stringify(body),
  });

  if (response.status === 204) {
    return undefined as T;
  }

  const text = await response.text();
  let data: unknown;

  if (text && response.headers.get("content-type")?.includes("json")) {
    try {
      data = JSON.parse(text);
    } catch {
      throw new ApiError(
        response.status,
        "Servern returnerade ogiltig JSON.",
      );
    }
  }

  if (!response.ok) {
    if (response.status === 400 || response.status === 401) {
      clearCsrfToken();
    }

    const problem = data as ApiProblem | undefined;

    const validationMessage = problem?.errors
      ? Object.values(problem.errors).flat().join(" ")
      : undefined;

    throw new ApiError(
      response.status,
      problem?.message ||
        validationMessage ||
        problem?.title ||
        `Anropet misslyckades (${response.status}).`,
    );
  }

  if (text && data === undefined) {
    throw new ApiError(
      response.status,
      "Servern returnerade ett oväntat svar.",
    );
  }

  return data as T;
}