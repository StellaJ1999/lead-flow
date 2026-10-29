import { Suspense } from "react";

import { LeadDetails } from "@/features/leads/components/LeadDetails";

export const instant = false;

export default function LeadDetailsPage() {
  return (
    <Suspense fallback={<p role="status">Öppnar lead…</p>}>
      <LeadDetails />
    </Suspense>
  );
}