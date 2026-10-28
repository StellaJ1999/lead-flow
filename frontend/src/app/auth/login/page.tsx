import { LoginForm } from "@/features/auth/components/LoginForm";

export default function LoginPage() {
  return (
    <main className="mx-auto max-w-md px-6 py-16">
      <h1 className="text-3xl font-semibold">
        Logga in på LeadFlow
      </h1>

      <LoginForm />
    </main>
  );
}