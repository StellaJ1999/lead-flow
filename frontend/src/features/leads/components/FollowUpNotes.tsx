import type { FollowUpNotesProps } from "../types/FollowUpNotes";
import { formatLeadDate } from "../lib/lead-presentation";

export function FollowUpNotes({ notes }: FollowUpNotesProps) {
  return (
    <section aria-labelledby="follow-up-notes-heading">
      <h2
        id="follow-up-notes-heading"
        className="text-xl font-semibold"
      >
        Follow-up notes
      </h2>

      {notes.length === 0 ? (
        <p className="mt-2">No follow-up notes yet.</p>
      ) : (
        <ol className="mt-4 space-y-4">
          {notes.map(note => (
            <li
              key={note.id}
              className="rounded border p-4"
            >
              <p className="whitespace-pre-wrap break-words">
                {note.text}
              </p>

              <div className="mt-3 text-sm text-gray-600">
                <p className="break-words">
                  Author: {note.author}
                </p>

                <time dateTime={note.createdAt}>
                  {formatLeadDate(note.createdAt)}
                </time>
              </div>
            </li>
          ))}
        </ol>
      )}
    </section>
  );
}