import Sidebar from "@/components/Sidebar";

export default function Layout({ children }: { children: React.ReactNode }) { 
    return (
        <div className="flex min-h-screen">
            <Sidebar />

            <main className="flex-1 overflow-y-auto p-4">
                {children}
            </main>
        </div>
    );
}
