import type { Metadata } from "next";
import { Poppins } from "next/font/google";
import "./globals.css";
import { AuthProvider } from '../contexts/AuthContext';
import { ThemeRegistry } from '../components/ThemeRegistry';

const poppins = Poppins({ 
  subsets: ["latin"],
  weight: ["300", "400", "500", "600", "700"]
});

export const metadata: Metadata = {
  title: "Sistema de Assiduidade Escolar",
  description: "Sistema para controle de frequência escolar",
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="pt-BR">
      <body className={poppins.className}>
        <ThemeRegistry>
          <AuthProvider>
            <main className="min-h-screen bg-gray-100">
              {children}
            </main>
          </AuthProvider>
        </ThemeRegistry>
      </body>
    </html>
  );
}
