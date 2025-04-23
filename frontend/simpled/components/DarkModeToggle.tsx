'use client';
import { useEffect, useState } from 'react';

export function DarkModeToggle() {
  const [darkMode, setDarkMode] = useState<boolean | null>(null);
  const [isTransitioning, setIsTransitioning] = useState(false);

  useEffect(() => {
    const stored = localStorage.getItem('darkMode');
    const prefersDark =
      stored === null
        ? window.matchMedia('(prefers-color-scheme: dark)').matches
        : stored === 'true';

    setDarkMode(prefersDark);
  }, []);

  useEffect(() => {
    if (darkMode !== null) {
      document.documentElement.classList.toggle('dark', darkMode);
      localStorage.setItem('darkMode', darkMode.toString());
    }
  }, [darkMode]);

  const toggleDarkMode = () => {
    setIsTransitioning(true);
    setTimeout(() => {
      setDarkMode((prev) => !prev);
      setIsTransitioning(false);
    }, 300);
  };

  if (darkMode === null) return null;

  return (
    <>
      {isTransitioning && (
        <div
          className={`fixed inset-0 z-50 flex items-center justify-center transition-opacity duration-500 ${
            darkMode ? 'bg-opacity-50 bg-black' : 'bg-opacity-50 bg-white'
          }`}
        >
          <div className="border-foreground h-12 w-12 animate-spin rounded-full border-b-2" />
        </div>
      )}

      <button
        onClick={toggleDarkMode}
        className="bg-background text-foreground hover:bg-foreground hover:text-background rounded-full border p-2"
      >
        {darkMode ? (
          <svg
            xmlns="http://www.w3.org/2000/svg"
            width="20"
            height="20"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            strokeWidth="2"
            strokeLinecap="round"
            strokeLinejoin="round"
            className="lucide lucide-sun"
          >
            <circle cx="12" cy="12" r="4" />
            <path d="M12 2v2" />
            <path d="M12 20v2" />
            <path d="m4.93 4.93 1.41 1.41" />
            <path d="m17.66 17.66 1.41 1.41" />
            <path d="M2 12h2" />
            <path d="M20 12h2" />
            <path d="m6.34 17.66-1.41 1.41" />
            <path d="m19.07 4.93-1.41 1.41" />
          </svg>
        ) : (
          <svg
            xmlns="http://www.w3.org/2000/svg"
            width="20"
            height="20"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            strokeWidth="2"
            strokeLinecap="round"
            strokeLinejoin="round"
            className="lucide lucide-moon"
          >
            <path d="M12 3a6 6 0 0 0 9 9 9 9 0 1 1-9-9Z" />
          </svg>
        )}
      </button>
    </>
  );
}
