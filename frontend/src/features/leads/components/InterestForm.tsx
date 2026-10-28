"use client";

import {
  useEffect,
  useId,
  useRef,
  useState,
  type FormEvent,
} from "react";

import { ApiError } from "@/lib/api";
import { createLead } from "../lib/leads";

import {
  fieldOrder,
  initialValues,
  limits,
  mapBackendErrors,
  validateInterestForm,
  type FieldErrors,
  type FieldName,
  type FormValues,
} from "../lib/interest-form";

type FormStatus = "idle" | "pending" | "success" | "error";

const fields: {
  name: FieldName;
  label: string;
  type: "text" | "email" | "textarea";
  autoComplete?: string;
}[] = [
  {
    name: "name",
    label: "Namn",
    type: "text",
    autoComplete: "name",
  },
  {
    name: "email",
    label: "E-post",
    type: "email",
    autoComplete: "email",
  },
  {
    name: "company",
    label: "Företag (valfritt)",
    type: "text",
    autoComplete: "organization",
  },
  {
    name: "message",
    label: "Meddelande",
    type: "textarea",
  },
];

export function InterestForm() {
  const id = useId();

  const formRef = useRef<HTMLFormElement>(null);
  const successRef = useRef<HTMLDivElement>(null);
  const submitting = useRef(false);

  const [values, setValues] = useState<FormValues>({
    ...initialValues,
  });

  const [errors, setErrors] = useState<FieldErrors>({});
  const [formError, setFormError] = useState("");
  const [status, setStatus] = useState<FormStatus>("idle");

  const pending = status === "pending";

  useEffect(() => {
    if (status === "success") {
      successRef.current?.focus();
    }
  }, [status]);

  function focusFirstError(nextErrors: FieldErrors) {
    const firstField = fieldOrder.find(
      field => Boolean(nextErrors[field]),
    );

    if (!firstField) {
      return;
    }

    const element = formRef.current?.elements.namedItem(firstField);

    if (
      element instanceof HTMLInputElement ||
      element instanceof HTMLTextAreaElement
    ) {
      element.focus();
    }
  }

  function updateValue(field: FieldName, value: string) {
    setValues(previous => ({
      ...previous,
      [field]: value,
    }));

    setErrors(previous => ({
      ...previous,
      [field]: undefined,
    }));

    setFormError("");

    if (status === "error") {
      setStatus("idle");
    }
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    // Stoppar även två submit-händelser innan React hunnit rendera om.
    if (submitting.current || status === "success") {
      return;
    }

    const nextErrors = validateInterestForm(values);

    // Använd även browserns formatkontroll för type="email".
    const emailInput = event.currentTarget.elements.namedItem("email");

    if (
      emailInput instanceof HTMLInputElement &&
      emailInput.validity.typeMismatch &&
      !nextErrors.email
    ) {
      nextErrors.email = "Ange en giltig e-postadress.";
    }

    setErrors(nextErrors);
    setFormError("");

    if (Object.keys(nextErrors).length > 0) {
      setStatus("error");
      setFormError("Kontrollera de markerade fälten.");
      focusFirstError(nextErrors);
      return;
    }

    submitting.current = true;
    setStatus("pending");

    try {
      await createLead({
        name: values.name.trim(),
        email: values.email.trim(),
        company: values.company.trim() || null,
        message: values.message.trim(),
      });

      setStatus("success");
    } catch (error) {
      const backendErrors =
        error instanceof ApiError &&
        (error.status === 400 || error.status === 422)
          ? mapBackendErrors(error.validationErrors)
          : {};

      setErrors(backendErrors);
      setStatus("error");

      if (Object.keys(backendErrors).length > 0) {
        setFormError("Kontrollera de markerade fälten.");
        focusFirstError(backendErrors);
      } else {
        setFormError("Det gick inte att skicka. Försök igen.");
      }
    } finally {
      submitting.current = false;
    }
  }

  if (status === "success") {
    return (
      <div
        ref={successRef}
        role="status"
        tabIndex={-1}
        className="rounded-lg border border-green-700 bg-green-50 p-6 text-green-950 focus:outline-2 focus:outline-offset-4 focus:outline-green-700"
      >
        <h2 className="text-xl font-semibold">Tack!</h2>

        <p className="mt-2">
          Din intresseanmälan har skickats.
        </p>
      </div>
    );
  }

  return (
    <form
      ref={formRef}
      onSubmit={handleSubmit}
      noValidate
      aria-label="Intresseanmälan"
      aria-busy={pending}
      className="space-y-6"
    >
      <p>Alla fält utom företag är obligatoriska.</p>

      {formError && (
        <p
          role="alert"
          className="rounded border border-red-700 bg-red-50 p-3 text-red-800"
        >
          {formError}
        </p>
      )}

      <fieldset
        disabled={pending}
        className="min-w-0 space-y-5"
      >
        <legend className="sr-only">Dina uppgifter</legend>

        {fields.map(field => {
          const inputId = `${id}-${field.name}`;
          const errorId = `${inputId}-error`;
          const error = errors[field.name];

          const inputClassName =
            "mt-1 block w-full min-w-0 rounded-md border " +
            "border-gray-500 bg-white px-3 py-2 text-base " +
            "text-gray-950 focus:outline-2 focus:outline-offset-2 " +
            "focus:outline-blue-700 disabled:opacity-60";

          const commonProps = {
            id: inputId,
            name: field.name,
            value: values[field.name],
            required: field.name !== "company",
            maxLength: limits[field.name],
            "aria-invalid": Boolean(error),
            "aria-describedby": error ? errorId : undefined,
            className: inputClassName,
          };

          return (
            <div key={field.name}>
              <label
                htmlFor={inputId}
                className="block font-medium"
              >
                {field.label}
              </label>

              {field.type === "textarea" ? (
                <textarea
                  {...commonProps}
                  rows={6}
                  onChange={event =>
                    updateValue(field.name, event.target.value)
                  }
                />
              ) : (
                <input
                  {...commonProps}
                  type={field.type}
                  autoComplete={field.autoComplete}
                  onChange={event =>
                    updateValue(field.name, event.target.value)
                  }
                />
              )}

              {error && (
                <p
                  id={errorId}
                  className="mt-1 break-words text-sm text-red-800"
                >
                  {error}
                </p>
              )}
            </div>
          );
        })}
      </fieldset>

      <button
        type="submit"
        disabled={pending}
        className="w-full rounded-md bg-blue-700 px-5 py-3 font-semibold text-white hover:bg-blue-800 focus:outline-2 focus:outline-offset-2 focus:outline-blue-700 disabled:cursor-wait disabled:opacity-60 sm:w-auto"
      >
        {pending ? "Skickar…" : "Skicka intresseanmälan"}
      </button>
    </form>
  );
}