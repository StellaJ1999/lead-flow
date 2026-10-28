import { api } from "@/lib/api";

export type CreateLeadRequest = {
  name: string;
  email: string;
  company?: string;
  message: string;
};

export type LeadResponse = {
  id: string;
  name: string;
  email: string;
  company: string | null;
  status: number;
  createdAt: string;
};

export function getLeads(): Promise<LeadResponse[]> {
  return api<LeadResponse[]>("/api/leads");
}

export function createLead(
  request: CreateLeadRequest,
): Promise<{ id: string }> {
  return api<{ id: string }>("/api/leads", {
    method: "POST",
    body: request,
  });
}