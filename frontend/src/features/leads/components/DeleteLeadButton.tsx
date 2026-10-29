"use client";

import { useRef, useState } from "react";

import { ApiError } from "@/lib/api";
import { getAuthErrorMessage } from "@/features/auth/lib/auth";
import { useSession } from "@/features/auth/components/SessionProvider";

import { deleteLead } from "../lib/leads";

type DeleteLeadButtonProps = {
  leadId: string;
  leadName: string;
};

export function DeleteLeadButton({
  leadId,
  leadName,
}: DeleteLeadButtonProps) {
  const { protectedRequest } = useSession();

  const submitting = useRef(false);

  const [pending, setPending] = useState(false);
  const [error, setError] = useState("");

  async function handleDelete() {
    if (submitting.current) {
      return;
    }

    const confirmed = window.confirm(
      `Vill du ta bort "${leadName}"?\n\n` +
        "Leadet och eventuella uppföljningsanteckningar " +
        "tas bort permanent.",
    );

    if (!confirmed) {
      return;
    }

    submitting.current = true;
    setPending(true);
    setError("");

    try {
      await protectedRequest(() => deleteLead(leadId));

      // Ladda listan på nytt utan tidigare bevarat komponentstate.
      window.location.replace("/leads");
    } catch (error) {
      if (error instanceof ApiError && error.status === 401) {
        // SessionProvider skickar användaren till login.
        return;
      }

      if (error instanceof ApiError && error.status === 404) {
        // Leadet har redan tagits bort, exempelvis av en annan användare.
        window.location.replace("/leads");
        return;
      }

      setError(
        error instanceof ApiError && error.status === 403
          ? "Du saknar behörighet att ta bort leadet."
          : getAuthErrorMessage(
              error,
              "Det gick inte att ta bort leadet. Försök igen.",
            ),
      );

      submitting.current = false;
      setPending(false);
    }
  }

  return (
    <div className="space-y-3 border-t pt-6">
      <button
        type="button"
        onClick={handleDelete}
        disabled={pending}
        aria-busy={pending}
        className="rounded bg-red-700 px-5 py-3 font-semibold text-white hover:bg-red-800 focus:outline-2 focus:outline-offset-2 focus:outline-red-700 disabled:opacity-60"
      >
        {pending ? "Tar bort…" : "Ta bort lead"}
      </button>

      {error && (
        <p role="alert" className="text-red-800">
          {error}
        </p>
      )}
    </div>
  );
}