export type LeadStatus = 0 | 1 | 2;

export const statusLabels: Record<LeadStatus, string> = {
  0: "New",
  1: "Contacted",
  2: "Closed",
};

const dateFormatter = new Intl.DateTimeFormat("sv-SE", {
  dateStyle: "short",
  timeStyle: "short",
  timeZone: "Europe/Stockholm",
});

export function formatLeadDate(value: string): string {
  const date = new Date(value);

  if (Number.isNaN(date.getTime())) {
    return "Okänt datum";
  }

  return dateFormatter.format(date);
}