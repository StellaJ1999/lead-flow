export type FormValues = {
  name: string;
  email: string;
  company: string;
  message: string;
};

export type FieldName = keyof FormValues;

export type FieldErrors = Partial<Record<FieldName, string>>;

export type InterestFormField = {
  name: FieldName;
  label: string;
  type: "text" | "email" | "textarea";
  autoComplete?: string;
};
export type FormStatus = "idle" | "pending" | "success" | "error";
