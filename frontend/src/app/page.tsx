import { InterestForm } from "@/features/leads/components/InterestForm";

export default function HomePage() {
  return (
    <main className="mx-auto max-w-2xl px-4 py-10 sm:px-6 sm:py-16">
      <h1 className="text-3xl font-semibold">
        Intresseanmälan
      </h1>

      <p className="mt-3">
        Vill du veta mer? Lämna dina uppgifter och ett meddelande,
        så kontaktar vi dig.
      </p>

      <div className="mt-8">
        <InterestForm />
      </div>
    </main>
  );
}