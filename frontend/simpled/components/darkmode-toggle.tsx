"use client";
import { useState, useEffect } from "react";

export function DarkModeToggle() {
  const [darkMode, setDarkMode] = useState(() => {
    const storedPreference = localStorage.getItem("darkMode");
    return storedPreference === "true";
  });

  /* ## Arreglar error de localStorage en consola de front pero genera flash porque empieza en blanco y luego pone el tema predeterminado ## 
    
    const [darkMode, setDarkMode] = useState(() => {
    if (typeof window !== "undefined") {
      const storedPreference = localStorage.getItem("darkMode");
      return storedPreference !== "false"; 
    }
    return true;
  }); */

  const [isTransitioning, setIsTransitioning] = useState(false);

  const toggleDarkMode = () => {
    setIsTransitioning(true);
    setTimeout(() => {
      setDarkMode(!darkMode);
      localStorage.setItem("darkMode", (!darkMode).toString());
      setIsTransitioning(false);
    }, 500);
  };

  useEffect(() => {
    if (darkMode) {
      document.documentElement.classList.add("dark");
    } else {
      document.documentElement.classList.remove("dark");
    }
  }, [darkMode]);

  return (
    <>
      {isTransitioning && (
        <div
          className={`fixed inset-0 flex items-center justify-center z-50 transition-opacity duration-500 ${darkMode ? "bg-black bg-opacity-50" : "bg-white bg-opacity-50"}`}
        >
          <div className="spinner"></div>
        </div>
      )}

      <button
        onClick={toggleDarkMode}
        className="p-2 rounded-full border bg-background text-foreground hover:bg-foreground hover:text-background"
      >
        {darkMode ?
          (
            <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" className="lucide lucide-sun">
              <circle cx="12" cy="12" r="4" /><path d="M12 2v2" /><path d="M12 20v2" /><path d="m4.93 4.93 1.41 1.41" /><path d="m17.66 17.66 1.41 1.41" /><path d="M2 12h2" /><path d="M20 12h2" /><path d="m6.34 17.66-1.41 1.41" /><path d="m19.07 4.93-1.41 1.41" />
            </svg>
          ) : (
            <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" className="lucide lucide-moon">
              <path d="M12 3a6 6 0 0 0 9 9 9 9 0 1 1-9-9Z" />
            </svg>
          )
        }
      </button>
    </>
  );
}