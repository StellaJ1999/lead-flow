"use client";

import { DeleteLeadButton } from "./DeleteLeadButton";
import { FollowUpNotes } from "./FollowUpNotes";
import { LeadStatusForm } from "./LeadStatusForm";
import Link from "next/link";
import { useParams } from "next/navigation";
import { useEffect, useState } from "react";

import { ApiError } from "@/lib/api";
import { getAuthErrorMessage } from "@/features/auth/lib/auth";
import { useSession } from "@/features/auth/components/SessionProvider";

import {
  getLeadById,
  type LeadDetailsResponse,
} from "../lib/leads";

import {
  formatLeadDate,
  statusLabels,
} from "../lib/lead-presentation";

export function LeadDetails() {
  const { id } = useParams<{ id: string }>();
  const { protectedRequest } = useSession();

  const [lead, setLead] = useState<LeadDetailsResponse | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    let active = true;
    const controller = new AbortController();

    protectedRequest(() => getLeadById(id, controller.signal))
      .then(result => {
        if (active) {
          setLead(result);
          setError("");
        }
      })
      .catch(error => {
        if (!active || controller.signal.aborted) {
          return;
        }

        if (error instanceof ApiError && error.status === 401) {
          return;
        }

        setLead(null);

        if (error instanceof ApiError && error.status === 404) {
          setError("Leadet kunde inte hittas.");
        } else if (
          error instanceof ApiError &&
          error.status === 403
        ) {
          setError("Du saknar behörighet att visa leadet.");
        } else {
          setError(
            getAuthErrorMessage(
              error,
              "Leadet kunde inte hämtas. Försök ladda om sidan.",
            ),
          );
        }
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
  }, [id, protectedRequest]);

  return (
    <section className="space-y-6" aria-busy={loading}>
      <Link
        href="/leads"
        className="rounded text-blue-700 underline focus:outline-2 focus:outline-offset-2 focus:outline-blue-700"
      >
        Tillbaka till leadlistan
      </Link>

      {loading ? (
        <p role="status">Hämtar lead…</p>
      ) : error ? (
        <p role="alert">{error}</p>
      ) : lead ? (
        <>
          <h1 className="break-words text-3xl font-semibold">
            {lead.name}
          </h1>

          <dl className="grid gap-5 sm:grid-cols-2">
            <div>
              <dt className="font-semibold">E-post</dt>
              <dd className="break-all">{lead.email}</dd>
            </div>

            <div>
              <dt className="font-semibold">Företag</dt>
              <dd className="break-words">
                {lead.company || "Ej angivet"}
              </dd>
            </div>

            <div>
              <dt className="font-semibold">Status</dt>
              <dd>
                {statusLabels[lead.status] ?? "Okänd status"}
              </dd>
            </div>

            {lead.status === 2 && (
            <div>
                <dt className="font-semibold">Outcome</dt>
                <dd>
                {lead.outcome === 0
                    ? "Customer"
                    : lead.outcome === 1
                    ? "Not interested"
                    : "Not specified"}
                </dd>
            </div>
            )}

            <div>
              <dt className="font-semibold">Skapad</dt>
              <dd>
                <time dateTime={lead.createdAt}>
                  {formatLeadDate(lead.createdAt)}
                </time>
              </dd>
            </div>
          </dl>
          
            <LeadStatusForm
                key={lead.id}
                lead={lead}
                onSaved={result => {
                    setLead(current =>
                    current && current.id === result.id
                        ? {
                            ...current,
                            status: result.status,
                            outcome: result.outcome,
                        }
                        : current,
                    );
                }}
            />

          <div>
            <h2 className="text-xl font-semibold">Meddelande</h2>

            <p className="mt-2 whitespace-pre-wrap break-words">
              {lead.message}
            </p>
          </div>

          <FollowUpNotes notes={lead.followUpNotes} />
          <DeleteLeadButton
                leadId={lead.id}
                leadName={lead.name}
            />
        </>
      ) : null}
    </section>
  );
}