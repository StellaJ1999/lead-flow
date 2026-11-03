"use client";

import Link from "next/link";
import { useEffect, useState } from "react";

import { ApiError } from "@/lib/api";
import { getAuthErrorMessage } from "@/features/auth/lib/auth";
import { useSession } from "@/features/auth/components/SessionProvider";

import { getLeads } from "../lib/leads";
import type { LeadResponse } from "../types/leads";
import {
  formatLeadDate,
  statusLabels,
} from "../lib/lead-presentation";

export function LeadList() {
  const { protectedRequest } = useSession();

  const [leads, setLeads] = useState<LeadResponse[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [attempt, setAttempt] = useState(0);
  const [statusFilter, setStatusFilter] = useState("all");

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

  const visibleLeads =
    statusFilter === "all"
      ? leads
      : leads.filter(lead => lead.status === Number(statusFilter));

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

      <div className="max-w-xs">
        <label
          htmlFor="lead-status-filter"
          className="block font-medium"
        >
          Filtrera efter status
        </label>

        <select
          id="lead-status-filter"
          value={statusFilter}
          onChange={event => setStatusFilter(event.target.value)}
          disabled={loading}
          className="mt-1 block w-full rounded border bg-white p-3 text-gray-950 focus:outline-2 focus:outline-offset-2 focus:outline-blue-700"
        >
          <option value="all">Alla statusar</option>
          <option value="0">New</option>
          <option value="1">Contacted</option>
          <option value="2">Closed</option>
        </select>
      </div>

      {loading ? (
        <p role="status">Hämtar leads…</p>
      ) : error ? (
        <p role="alert">{error}</p>
      ) : leads.length === 0 ? (
        <p>Det finns inga leads ännu.</p>
      ) : (
        <>
          <p role="status">
            Visar {visibleLeads.length} av {leads.length} leads.
          </p>

          {visibleLeads.length === 0 ? (
            <p>Det finns inga leads med vald status.</p>
          ) : (
            <div
              role="region"
              aria-label="Leadlista"
              tabIndex={0}
              className="overflow-x-auto rounded border focus:outline-2 focus:outline-blue-700"
            >
              <table className="w-full text-left">
                <caption className="sr-only">
                  Leads sorterade med de nyaste först.
                  Välj ett namn för att öppna detaljer.
                </caption>

                <thead className="border-b bg-gray-100 text-gray-950">
                  <tr>
                    <th scope="col" className="px-4 py-3">
                      Namn
                    </th>
                    <th scope="col" className="px-4 py-3">
                      Företag
                    </th>
                    <th scope="col" className="px-4 py-3">
                      Status
                    </th>
                    <th scope="col" className="px-4 py-3">
                      Skapad
                    </th>
                  </tr>
                </thead>

                <tbody>
                  {visibleLeads.map(lead => (
                    <tr key={lead.id} className="border-b last:border-0">
                      <th
                        scope="row"
                        className="px-4 py-3 font-medium"
                      >
                        <Link
                          href={`/leads/${encodeURIComponent(lead.id)}`}
                          className="rounded text-blue-700 underline focus:outline-2 focus:outline-offset-2 focus:outline-blue-700"
                        >
                          {lead.name}
                        </Link>
                      </th>

                      <td className="px-4 py-3">
                        {lead.company || "Ej angivet"}
                      </td>

                      <td className="px-4 py-3">
                        {statusLabels[lead.status] ?? "Okänd status"}
                      </td>

                      <td className="whitespace-nowrap px-4 py-3">
                        <time dateTime={lead.createdAt}>
                          {formatLeadDate(lead.createdAt)}
                        </time>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </>
      )}
    </section>
  );
}