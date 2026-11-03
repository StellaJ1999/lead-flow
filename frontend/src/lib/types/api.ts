export type ApiOptions = Omit<RequestInit, "body" | "credentials"> & {
  body?: unknown;
};

export type ApiProblem = {
  message?: string;
  title?: string;
  errors?: Record<string, string[]>;
};

export type CsrfTokenResponse = {
  token: string;
};
