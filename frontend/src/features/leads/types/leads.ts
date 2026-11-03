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

export type ChangeLeadStatusRequest = {
  status: LeadStatus;
  outcome: LeadOutcome | null;
};

export type LeadStatusResponse = {
  id: string;
  status: LeadStatus;
  outcome: LeadOutcome | null;
};

export type CreateFollowUpNoteRequest = {
  text: string;
};

export type LeadRouteParams = {
  id: string;
};
