import type { LeadDetailsResponse, LeadStatusResponse } from "./leads";

export type LeadStatusFormProps = {
  lead: LeadDetailsResponse;
  onSaved: (result: LeadStatusResponse) => void;
};
