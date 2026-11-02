import { api } from "@/lib/api";
import type { LeadStatus } from "./lead-presentation";

export type CreateLeadRequest = {
  name: string;
  email: string;
  company: string | null;
  message: string;
};

export type CreateLeadResponse = {
  message: string;
};

export type LeadResponse = {
  id: string;
  name: string;
  email: string;
  company: string | null;
  status: LeadStatus;
  createdAt: string;
};

export function getLeads(
  signal?: AbortSignal,
): Promise<LeadResponse[]> {
  return api<LeadResponse[]>("/api/leads", { signal });
}

export function createLead(
  request: CreateLeadRequest,
): Promise<CreateLeadResponse> {
  return api<CreateLeadResponse>("/api/leads", {
    method: "POST",
    body: request,
  });
}

export type LeadOutcome = 0 | 1;

export type FollowUpNoteResponse = {
  id: string;
  text: string;
  author: string;
  createdAt: string;
};

export type LeadDetailsResponse = LeadResponse & {
  message: string;
  outcome: LeadOutcome | null;
  followUpNotes: FollowUpNoteResponse[];
};

export function getLeadById(
  id: string,
  signal?: AbortSignal,
): Promise<LeadDetailsResponse> {
  return api<LeadDetailsResponse>(
    `/api/leads/${encodeURIComponent(id)}`,
    { signal },
  );
}

export function deleteLead(id: string): Promise<void> {
  return api<void>(
    `/api/leads/${encodeURIComponent(id)}`,
    {
      method: "DELETE",
    },
  );
}

export type ChangeLeadStatusRequest = {
  status: LeadStatus;
  outcome: LeadOutcome | null;
};

export type LeadStatusResponse = {
  id: string;
  status: LeadStatus;
  outcome: LeadOutcome | null;
};

export function changeLeadStatus(
  id: string,
  request: ChangeLeadStatusRequest,
): Promise<LeadStatusResponse> {
  return api<LeadStatusResponse>(
    `/api/leads/${encodeURIComponent(id)}/status`,
    {
      method: "PATCH",
      body: request,
    },
  );
}