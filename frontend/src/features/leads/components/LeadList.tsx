"use client";

import { useEffect, useState } from "react";

import { ApiError } from "@/lib/api";
import { getAuthErrorMessage } from "@/features/auth/lib/auth";
import { useSession } from "@/features/auth/components/SessionProvider";

import { getLeads, type LeadResponse } from "../lib/leads";

export function LeadList() {
  const { protectedRequest } = useSession();

  const [leads, setLeads] = useState<LeadResponse[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [attempt, setAttempt] = useState(0);

  useEffect(() => {
    let active = true;
    const controller = new AbortController();

    protectedRequest(() => getLeads(controller.signal))
      .then(result => {
        if (active) {
          setLeads(result);
        }
      })
      .catch(error => {
        if (!active || controller.signal.aborted) {
          return;
        }

        if (error instanceof ApiError && error.status === 401) {
          // SessionProvider hanterar navigeringen till login.
          return;
        }

        setError(
          error instanceof ApiError && error.status === 403
            ? "Du saknar behörighet att visa leads."
            : getAuthErrorMessage(
                error,
                "Leadlistan kunde inte hämtas. Försök igen.",
              ),
        );
      })
      .finally(() => {
        if (active) {
          setLoading(false);
        }
      });

    return () => {
      active = false;
      controller.abort();
    };
  }, [attempt, protectedRequest]);

  function reload() {
    setLoading(true);
    setError("");
    setLeads([]);
    setAttempt(previous => previous + 1);
  }

  return (
    <section className="space-y-5" aria-busy={loading}>
      <div className="flex flex-wrap items-center gap-4">
        <h1 className="text-3xl font-semibold">Leadlista</h1>

        <button
          type="button"
          onClick={reload}
          disabled={loading}
          className="rounded border px-4 py-2 focus:outline-2 focus:outline-offset-2 focus:outline-blue-700 disabled:opacity-60"
        >
          Uppdatera
        </button>
      </div>

      {loading ? (
        <p role="status">Hämtar leads…</p>
      ) : error ? (
        <p role="alert">{error}</p>
      ) : leads.length === 0 ? (
        <p>Det finns inga leads ännu.</p>
      ) : (
        <ul className="grid gap-4 md:grid-cols-2">
          {leads.map(lead => (
            <li
              key={lead.id}
              className="min-w-0 rounded border p-4"
            >
              <h2 className="font-semibold">{lead.name}</h2>
              <p className="break-all">{lead.email}</p>

              {lead.company && (
                <p className="mt-1 break-words">{lead.company}</p>
              )}
            </li>
          ))}
        </ul>
      )}
    </section>
  );
}