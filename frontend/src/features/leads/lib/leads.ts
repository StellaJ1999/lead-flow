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