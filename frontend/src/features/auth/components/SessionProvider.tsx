"use client";

import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useRef,
  useState,
  type ReactNode,
} from "react";

import { usePathname, useRouter } from "next/navigation";

import { ApiError, clearCsrfToken } from "@/lib/api";

import {
  getAuthErrorMessage,
  getCurrentUser,
  type CurrentUser,
} from "../lib/auth";

type SessionState =
  | { status: "checking" | "redirecting" }
  | { status: "error"; message: string }
  | {
      status: "authenticated";
      user: CurrentUser;
      path: string;
    };

type SessionContextValue = {
  user: CurrentUser;
  endSession: () => void;
  protectedRequest: <T>(
    operation: () => Promise<T>,
  ) => Promise<T>;
};

const SessionContext = createContext<SessionContextValue | null>(null);

export function useSession(): SessionContextValue {
  const session = useContext(SessionContext);

  if (!session) {
    throw new Error("useSession måste användas inuti SessionProvider.");
  }

  return session;
}

export function SessionProvider({
  children,
}: {
  children: ReactNode;
}) {
  const router = useRouter();
  const pathname = usePathname();
  const requestId = useRef(0);

  const [state, setState] = useState<SessionState>({
    status: "checking",
  });

  const endSession = useCallback(() => {
    requestId.current++;
    clearCsrfToken();

    setState({ status: "redirecting" });
    router.replace("/auth/login");
  }, [router]);

  const checkSession = useCallback(async () => {
    const currentRequest = ++requestId.current;

    setState({ status: "checking" });

    try {
      const user = await getCurrentUser();

      if (currentRequest !== requestId.current) {
        return;
      }

      setState({
        status: "authenticated",
        user,
        path: pathname,
      });
    } catch (error) {
      if (currentRequest !== requestId.current) {
        return;
      }

      if (error instanceof ApiError && error.status === 401) {
        endSession();
        return;
      }

      setState({
        status: "error",
        message:
          error instanceof ApiError && error.status === 403
            ? "Du saknar behörighet till de interna sidorna."
            : getAuthErrorMessage(
                error,
                "Sessionen kunde inte kontrolleras. Försök igen.",
              ),
      });
    }
  }, [pathname, endSession]);

  const protectedRequest = useCallback(
    async function request<T>(
      operation: () => Promise<T>,
    ): Promise<T> {
      try {
        return await operation();
      } catch (error) {
        if (error instanceof ApiError && error.status === 401) {
          endSession();
        }

        throw error;
      }
    },
    [endSession],
  );

  useEffect(() => {
    function check() {
      void checkSession();
    }

    function hide() {
      requestId.current++;
      setState({ status: "checking" });
    }

    function handlePageShow(event: PageTransitionEvent) {
      if (event.persisted) {
        check();
      }
    }

    check();

    window.addEventListener("popstate", check);
    window.addEventListener("pageshow", handlePageShow);
    window.addEventListener("pagehide", hide);

    return () => {
      requestId.current++;

      window.removeEventListener("popstate", check);
      window.removeEventListener("pageshow", handlePageShow);
      window.removeEventListener("pagehide", hide);
    };
  }, [checkSession]);

  if (state.status === "error") {
    return (
      <main className="mx-auto max-w-xl space-y-4 px-4 py-12">
        <p role="alert">{state.message}</p>

        <button
          type="button"
          onClick={() => void checkSession()}
          className="rounded border px-4 py-2 focus:outline-2 focus:outline-offset-2 focus:outline-blue-700"
        >
          Försök igen
        </button>
      </main>
    );
  }

  if (
    state.status !== "authenticated" ||
    state.path !== pathname
  ) {
    return (
      <main className="px-4 py-12">
        <p role="status">
          {state.status === "redirecting"
            ? "Öppnar inloggningen…"
            : "Kontrollerar session…"}
        </p>
      </main>
    );
  }

  return (
    <SessionContext.Provider
      value={{
        user: state.user,
        endSession,
        protectedRequest,
      }}
    >
      {children}
    </SessionContext.Provider>
  );
}