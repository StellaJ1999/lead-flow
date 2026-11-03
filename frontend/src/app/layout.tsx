import type { Metadata } from "next";
import type { LayoutChildrenProps } from "@/app/types/layout-props";
import "./globals.css";

export const metadata: Metadata = {
  title: "LeadFlow",
  description: "Från intresseanmälan till uppföljt lead.",
};

export default function RootLayout({ children }: LayoutChildrenProps) {
  return (
    <html lang="sv">
      <body>{children}</body>
    </html>
  );
}
