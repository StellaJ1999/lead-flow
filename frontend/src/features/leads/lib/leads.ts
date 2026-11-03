import type {
  CreateLeadRequest,
  CreateLeadResponse,
  LeadResponse,
  FollowUpNoteResponse,
  LeadDetailsResponse,
  ChangeLeadStatusRequest,
  LeadStatusResponse,
  CreateFollowUpNoteRequest,
} from "../types/leads";

import { api } from "@/lib/api";

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

export function createFollowUpNote(
  leadId: string,
  request: CreateFollowUpNoteRequest,
): Promise<FollowUpNoteResponse> {
  return api<FollowUpNoteResponse>(
    `/api/leads/${encodeURIComponent(leadId)}/notes`,
    {
      method: "POST",
      body: request,
    },
  );
}