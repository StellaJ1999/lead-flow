"use client";

import Link from "next/link";
import { useRef, useState } from "react";

import { ApiError } from "@/lib/api";
import { getAuthErrorMessage, logout } from "../lib/auth";
import { useSession } from "./SessionProvider";

export function InternalNavigation() {
  const { user, endSession } = useSession();

  const submitting = useRef(false);

  const [pending, setPending] = useState(false);
  const [error, setError] = useState("");

  async function handleLogout() {
    if (submitting.current) {
      return;
    }

    submitting.current = true;
    setPending(true);
    setError("");

    try {
      await logout();

      // Rensa frontendens session först efter bekräftad logout.
      endSession();
    } catch (error) {
      if (error instanceof ApiError && error.status === 401) {
        // Backend bekräftar att sessionen redan är ogiltig.
        endSession();
        return;
      }

      setError(
        getAuthErrorMessage(
          error,
          "Det gick inte att logga ut. Försök igen.",
        ),
      );
    } finally {
      submitting.current = false;
      setPending(false);
    }
  }

  return (
    <header className="border-b">
      <div className="mx-auto max-w-6xl space-y-3 px-4 py-4 sm:px-6">
        <nav
          aria-label="Intern navigation"
          className="flex flex-wrap items-center gap-4"
        >
          <Link
            href="/leads"
            className="rounded font-semibold underline focus:outline-2 focus:outline-offset-2 focus:outline-blue-700"
          >
            Leadlista
          </Link>

          <p className="min-w-0 break-all text-sm sm:ml-auto">
            {user.email}
          </p>

          <button
            type="button"
            onClick={handleLogout}
            disabled={pending}
            aria-busy={pending}
            className="rounded border px-4 py-2 focus:outline-2 focus:outline-offset-2 focus:outline-blue-700 disabled:opacity-60"
          >
            {pending ? "Loggar ut…" : "Logga ut"}
          </button>
        </nav>

        {error && (
          <p role="alert" className="text-red-800">
            {error}
          </p>
        )}
      </div>
    </header>
  );
}