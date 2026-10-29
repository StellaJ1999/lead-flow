"use client";

import { useEffect, useRef, useState, type FormEvent } from "react";
import { useRouter } from "next/navigation";

import { ApiError } from "@/lib/api";

import {
  getAuthErrorMessage,
  getCurrentUser,
  login,
} from "../lib/auth";

export function LoginForm() {
  const router = useRouter();
  const submitting = useRef(false);

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  const [checking, setChecking] = useState(true);
  const [busy, setBusy] = useState(false);
  const [redirecting, setRedirecting] = useState(false);

  useEffect(() => {
    let active = true;

    getCurrentUser()
      .then(() => {
        if (active) {
          setRedirecting(true);
          router.replace("/leads");
        }
      })
      .catch(error => {
        if (!active) {
          return;
        }
        
        // Återställ eventuell navigering från föregående inloggning.
        setRedirecting(false);
        
        const isAnonymous =
          error instanceof ApiError && error.status === 401;

        if (!isAnonymous) {
          setError(
            getAuthErrorMessage(
              error,
              "Kunde inte kontrollera sessionen. Försök logga in igen.",
            ),
          );
        }
      })
      .finally(() => {
        if (active) {
          setChecking(false);
        }
      });

    return () => {
      active = false;
    };
  }, [router]);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    if (submitting.current || redirecting) {
      return;
    }

    if (!email.trim() || password.length === 0) {
      setError("Ange e-postadress och lösenord.");
      return;
    }

    submitting.current = true;
    setBusy(true);
    setError("");

    try {
      await login(email, password);

      setPassword("");
      setRedirecting(true);

      router.replace("/leads");
    } catch (error) {
      setError(getAuthErrorMessage(error));

      // E-postadressen behålls.
      setPassword("");
    } finally {
      submitting.current = false;
      setBusy(false);
    }
  }

  if (checking || redirecting) {
    return (
      <p role="status" className="mt-6">
        {redirecting ? "Öppnar leadlistan…" : "Kontrollerar session…"}
      </p>
    );
  }

  return (
    <form
      onSubmit={handleSubmit}
      aria-busy={busy}
      aria-describedby={error ? "login-error" : undefined}
      className="mt-6 space-y-5"
    >
      {error && (
        <p
          id="login-error"
          role="alert"
          className="rounded border border-red-700 bg-red-50 p-3 text-red-800"
        >
          {error}
        </p>
      )}

      <div>
        <label htmlFor="login-email" className="block font-medium">
          E-post
        </label>

        <input
          id="login-email"
          name="email"
          type="email"
          autoComplete="username"
          required
          value={email}
          onChange={event => setEmail(event.target.value)}
          disabled={busy}
          className="mt-1 block w-full rounded border p-3 focus:outline-2 focus:outline-offset-2 focus:outline-blue-700"
        />
      </div>

      <div>
        <label htmlFor="login-password" className="block font-medium">
          Lösenord
        </label>

        <input
          id="login-password"
          name="password"
          type="password"
          autoComplete="current-password"
          required
          value={password}
          onChange={event => setPassword(event.target.value)}
          disabled={busy}
          className="mt-1 block w-full rounded border p-3 focus:outline-2 focus:outline-offset-2 focus:outline-blue-700"
        />
      </div>

      <button
        type="submit"
        disabled={busy}
        className="w-full rounded bg-blue-700 px-5 py-3 font-semibold text-white focus:outline-2 focus:outline-offset-2 focus:outline-blue-700 disabled:opacity-60"
      >
        {busy ? "Loggar in…" : "Logga in"}
      </button>
    </form>
  );
}