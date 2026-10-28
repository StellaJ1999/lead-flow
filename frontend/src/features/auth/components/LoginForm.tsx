"use client";

import { useEffect, useState, type FormEvent } from "react";
import { ApiError } from "../../../lib/api";
import {
  getCurrentUser,
  login,
  logout,
  type CurrentUser,
} from "../lib/auth";

export function LoginForm() {
  const [user, setUser] = useState<CurrentUser | null>(null);
  const [busy, setBusy] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    let active = true;

    getCurrentUser()
      .then(currentUser => {
        if (active) {
          setUser(currentUser);
        }
      })
      .catch(error => {
        // 401 är förväntat när besökaren inte är inloggad.
        const isAnonymous =
          error instanceof ApiError && error.status === 401;

        if (active && !isAnonymous) {
          setError("Kunde inte kontrollera inloggningen.");
        }
      })
      .finally(() => {
        if (active) {
          setBusy(false);
        }
      });

    return () => {
      active = false;
    };
  }, []);

  async function handleLogin(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    const form = event.currentTarget;
    const data = new FormData(form);

    const email = String(data.get("email") ?? "");
    const password = String(data.get("password") ?? "");

    setBusy(true);
    setError("");

    try {
      await login(email, password);
      form.reset();

      const currentUser = await getCurrentUser();
      setUser(currentUser);
    } catch (error) {
      setError(
        error instanceof Error
          ? error.message
          : "Inloggningen misslyckades.",
      );

      // Login kan ha lyckats även om nästa tokenhämtning misslyckades.
      try {
        setUser(await getCurrentUser());
      } catch {
        setUser(null);
      }
    } finally {
      setBusy(false);
    }
  }

  async function handleLogout() {
    setBusy(true);
    setError("");

    try {
      await logout();
      setUser(null);
    } catch (error) {
      setError(
        error instanceof Error
          ? error.message
          : "Utloggningen misslyckades.",
      );
    } finally {
      setBusy(false);
    }
  }

  return (
    <section className="mt-6 space-y-4">
      {error && <p role="alert">{error}</p>}

      {user ? (
        <>
          <p>Inloggad som {user.email}</p>

          <button
            type="button"
            onClick={handleLogout}
            disabled={busy}
            className="rounded border px-4 py-2"
          >
            Logga ut
          </button>
        </>
      ) : (
        <form
          onSubmit={handleLogin}
          aria-busy={busy}
          className="space-y-4"
        >
          <label className="block">
            E-post
            <input
              name="email"
              type="email"
              autoComplete="username"
              required
              disabled={busy}
              className="mt-1 block w-full rounded border p-2"
            />
          </label>

          <label className="block">
            Lösenord
            <input
              name="password"
              type="password"
              autoComplete="current-password"
              required
              disabled={busy}
              className="mt-1 block w-full rounded border p-2"
            />
          </label>

          <button
            type="submit"
            disabled={busy}
            className="rounded border px-4 py-2"
          >
            {busy ? "Vänta…" : "Logga in"}
          </button>
        </form>
      )}
    </section>
  );
}