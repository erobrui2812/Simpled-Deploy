import { ReactNode } from "react";

interface BannerProps {
    children: ReactNode;
}

export default function Banner({children} : BannerProps) {
    return (
        <section className="bg-gradient-to-r from-blue-500 to-indigo-600 py-16 px-4 text-center">
            {children}
        </section>
    );
}