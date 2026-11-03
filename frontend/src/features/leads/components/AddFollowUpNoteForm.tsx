"use client";

import type { AddFollowUpNoteFormProps } from "../types/AddFollowUpNoteForm";
import { useRef, useState, type FormEvent } from "react";

import { ApiError } from "@/lib/api";
import { useSession } from "@/features/auth/components/SessionProvider";

import { createFollowUpNote } from "../lib/leads";

export function AddFollowUpNoteForm({
  leadId,
  onSaved,
}: AddFollowUpNoteFormProps) {
  const { protectedRequest } = useSession();

  const submitting = useRef(false);

  const [text, setText] = useState("");
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  async function handleSubmit(
    event: FormEvent<HTMLFormElement>,
  ) {
    event.preventDefault();

    if (submitting.current)
      return;

    setError("");
    setSuccess("");

    if (!text.trim()) {
      setError("Note text is required.");
      return;
    }

    if (text.length > 4000) {
      setError("Note must not exceed 4000 characters.");
      return;
    }

    submitting.current = true;
    setSaving(true);

    try {
      await protectedRequest(() =>
        createFollowUpNote(leadId, { text }),
      );

      // POST har nu bekräftat att anteckningen är sparad.
      setText("");
      setSuccess("Note saved.");

      try {
        await onSaved();
      } catch (error) {
        if (error instanceof ApiError && error.status === 401)
          return;

        setSuccess(
          "Note saved, but the history could not be refreshed. Reload the page.",
        );
      }
    } catch (error) {
      if (error instanceof ApiError && error.status === 401)
        return;

      // Texten rensas inte när sparandet misslyckas.
      setError(
        error instanceof ApiError
          ? error.message
          : "Could not save the note. Please try again.",
      );
    } finally {
      submitting.current = false;
      setSaving(false);
    }
  }

  return (
    <form
      onSubmit={handleSubmit}
      className="space-y-3"
      aria-busy={saving}
    >
      <h2 className="text-xl font-semibold">
        Add follow-up note
      </h2>

      <label
        htmlFor="follow-up-note"
        className="block font-semibold"
      >
        Note
      </label>

      <textarea
        id="follow-up-note"
        value={text}
        onChange={event => {
          setText(event.target.value);
          setError("");
          setSuccess("");
        }}
        required
        maxLength={4000}
        rows={5}
        disabled={saving}
        aria-describedby="note-character-count"
        className="w-full rounded border p-3"
      />

      <p id="note-character-count" className="text-sm">
        {text.length}/4000 characters
      </p>

      <button
        type="submit"
        disabled={saving || !text.trim()}
        className="rounded bg-blue-700 px-4 py-2 text-white disabled:opacity-50"
      >
        {saving ? "Saving…" : "Save note"}
      </button>

      {error && <p role="alert">{error}</p>}
      {success && <p role="status">{success}</p>}
    </form>
  );
}