import type { ReactNode } from "react";

import { InternalNavigation } from "@/features/auth/components/InternalNavigation";
import { SessionProvider } from "@/features/auth/components/SessionProvider";

export default function InternalLayout({
  children,
}: {
  children: ReactNode;
}) {
  return (
    <SessionProvider>
      <InternalNavigation />

      <main className="mx-auto max-w-6xl px-4 py-8 sm:px-6">
        {children}
      </main>
    </SessionProvider>
  );
}