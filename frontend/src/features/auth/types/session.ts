import type { ReactNode } from "react";
import type { CurrentUser } from "./auth";

export type SessionState =
  | { status: "checking" | "redirecting" }
  | { status: "error"; message: string }
  | {
      status: "authenticated";
      user: CurrentUser;
      path: string;
    };

export type SessionContextValue = {
  user: CurrentUser;
  endSession: () => void;
  protectedRequest: <T>(
    operation: () => Promise<T>,
  ) => Promise<T>;
};

export type SessionProviderProps = {
  children: ReactNode;
};
