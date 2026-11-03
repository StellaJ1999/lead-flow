"use client";

import type { LeadStatusFormProps } from "../types/LeadStatusForm";
import { useState, type FormEvent } from "react";

import { ApiError } from "@/lib/api";
import { useSession } from "@/features/auth/components/SessionProvider";

import { changeLeadStatus } from "../lib/leads";
import type { LeadOutcome } from "../types/leads";

import type { LeadStatus } from "../types/lead-presentation";

export function LeadStatusForm({ lead, onSaved }: LeadStatusFormProps) {
  const { protectedRequest } = useSession();

  const [status, setStatus] = useState<LeadStatus>(lead.status);

  const [outcome, setOutcome] = useState(
    lead.outcome === null ? "" : String(lead.outcome),
  );

  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    setError("");
    setSuccess("");

    if (status === 2 && outcome === "") {
      setError("Select an outcome before closing the lead.");
      return;
    }

    setSaving(true);

    try {
      const result = await protectedRequest(() =>
        changeLeadStatus(lead.id, {
          status,
          outcome:
            status === 2
              ? (Number(outcome) as LeadOutcome)
              : null,
        }),
      );

      onSaved(result);
      setSuccess("Lead status saved.");
    } catch (error) {
      if (error instanceof ApiError && error.status === 401) {
        return;
      }

      setError(
        error instanceof ApiError
          ? error.message
          : "Could not save the status. Please try again.",
      );
    } finally {
      setSaving(false);
    }
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <h2 className="text-xl font-semibold">Update status</h2>

      <fieldset disabled={saving} className="space-y-4">
        <div>
          <label htmlFor="lead-status" className="block font-semibold">
            Status
          </label>

          <select
            id="lead-status"
            value={status}
            onChange={event => {
              setStatus(Number(event.target.value) as LeadStatus);
              setOutcome("");
              setError("");
              setSuccess("");
            }}
            className="mt-1 rounded border p-2"
          >
            {lead.status === 0 && <option value={0}>New</option>}
            <option value={1}>Contacted</option>
            <option value={2}>Closed</option>
          </select>
        </div>

        {status === 2 && (
          <div>
            <label
              htmlFor="lead-outcome"
              className="block font-semibold"
            >
              Outcome
            </label>

            <select
              id="lead-outcome"
              value={outcome}
              required
              onChange={event => {
                setOutcome(event.target.value);
                setError("");
                setSuccess("");
              }}
              className="mt-1 rounded border p-2"
            >
              <option value="">Select an outcome</option>
              <option value="0">Customer</option>
              <option value="1">Not interested</option>
            </select>

            {lead.status === 2 && (
              <p className="mt-2 text-sm">
                Reopen the lead as Contacted before changing its outcome.
              </p>
            )}
          </div>
        )}

        <button
          type="submit"
          className="rounded bg-blue-700 px-4 py-2 text-white disabled:opacity-50"
        >
          {saving ? "Saving…" : "Save status"}
        </button>
      </fieldset>

      {error && <p role="alert">{error}</p>}
      {success && <p role="status">{success}</p>}
    </form>
  );
}