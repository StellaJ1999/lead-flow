import type { FormValues, FieldName, FieldErrors } from "../types/interest-form";

export const initialValues: FormValues = {
  name: "",
  email: "",
  company: "",
  message: "",
};

export const fieldOrder: FieldName[] = [
  "name",
  "email",
  "company",
  "message",
];

export const limits: Record<FieldName, number> = {
  name: 200,
  email: 320,
  company: 200,
  message: 4000,
};

export function validateInterestForm(
  values: FormValues,
): FieldErrors {
  const errors: FieldErrors = {};

  const name = values.name.trim();
  const email = values.email.trim();
  const company = values.company.trim();
  const message = values.message.trim();

  if (!name) {
    errors.name = "Ange ditt namn.";
  } else if (name.length > limits.name) {
    errors.name = "Namnet får innehålla högst 200 tecken.";
  }

  if (!email) {
    errors.email = "Ange din e-postadress.";
  } else if (email.length > limits.email) {
    errors.email = "E-postadressen får innehålla högst 320 tecken.";
  } else if (!/^[^\s@]+@[^\s@]+$/.test(email)) {
    errors.email = "Ange en giltig e-postadress.";
  }

  if (company.length > limits.company) {
    errors.company = "Företagsnamnet får innehålla högst 200 tecken.";
  }

  if (!message) {
    errors.message = "Skriv ett meddelande.";
  } else if (message.length > limits.message) {
    errors.message = "Meddelandet får innehålla högst 4 000 tecken.";
  }

  return errors;
}

export function mapBackendErrors(
  errors: Record<string, string[]> = {},
): FieldErrors {
  const result: FieldErrors = {};

  for (const [key, messages] of Object.entries(errors)) {
    const normalizedKey = key.toLowerCase();

    const field = fieldOrder.find(
      candidate => candidate === normalizedKey,
    );

    if (field && messages.length > 0) {
      result[field] = messages.join(" ");
    }
  }

  return result;
}