import Link from "next/link";
import type React from "react";

export default function IconLink({
    href,
    children,
    icon,
}: {
    href: string;
    children: React.ReactNode;
    icon?: React.ReactNode;
}) {
    return (
        <Link
            href={href}
            className="flex items-center gap-2 px-3 py-2 text-sm font-medium rounded-md hover:bg-accent"
        >
            {icon}
            <span>{children}</span>
        </Link>
    );
}
