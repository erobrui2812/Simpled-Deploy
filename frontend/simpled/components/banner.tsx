import { ReactNode } from 'react';

interface BannerProps {
  children: ReactNode;
  className: string;
}

export default function Banner({ children, className }: BannerProps) {
  return (
    <section
      className={`${className} bg-gradient-to-r from-blue-500 to-indigo-600 px-4 py-16 text-center`}
    >
      {children}
    </section>
  );
}
