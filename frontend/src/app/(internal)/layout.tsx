import type { LayoutChildrenProps } from "@/app/types/layout-props";

import { InternalNavigation } from "@/features/auth/components/InternalNavigation";
import { SessionProvider } from "@/features/auth/components/SessionProvider";

export default function InternalLayout({
  children,
}: LayoutChildrenProps) {
  return (
    <SessionProvider>
      <InternalNavigation />

      <main className="mx-auto max-w-6xl px-4 py-8 sm:px-6">
        {children}
      </main>
    </SessionProvider>
  );
}