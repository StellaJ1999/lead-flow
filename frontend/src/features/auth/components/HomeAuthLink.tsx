"use client";

import Link from "next/link";
import { useEffect, useState } from "react";

import { getCurrentUser } from "@/features/auth/lib/auth";
import { ApiError } from "@/lib/api";

type AuthState = "checking" | "signedIn" | "signedOut" | "error";

export function HomeAuthLink() {
  const [state, setState] = useState<AuthState>("checking");

  useEffect(() => {
    let active = true;

    async function checkSession() {
      try {
        await getCurrentUser();

        if (active) {
          setState("signedIn");
        }
      } catch (error) {
        if (active) {
          setState(
            error instanceof ApiError && error.status === 401
              ? "signedOut"
              : "error",
          );
        }
      }
    }

    void checkSession();

    return () => {
      active = false;
    };
  }, []);

  if (state === "checking") {
    return <span role="status">Kontrollerar inloggning…</span>;
  }

  if (state === "error") {
    return (
      <span role="status">
        Kunde inte kontrollera inloggningen. Ladda om sidan.
      </span>
    );
  }

  const signedIn = state === "signedIn";

  return (
    <Link
      href={signedIn ? "/leads" : "/auth/login"}
      className="inline-flex rounded bg-blue-700 px-4 py-2 font-medium text-white hover:bg-blue-800 focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-blue-700"
    >
      {signedIn ? "Leadlista" : "Logga in"}
    </Link>
  );
}
