"use client";

import { useEffect, useState } from "react";
import { cn } from "@/lib/utils";
import { Button } from "@/components/ui/button";
import { Accordion, AccordionContent, AccordionItem, AccordionTrigger } from "@/components/ui/accordion";
import { Home, Settings, Menu, ChevronRight, ChevronDown, RefreshCw, Grid2x2 } from "lucide-react";
import { Select, SelectContent, SelectGroup, SelectItem, SelectLabel, SelectTrigger, SelectValue } from "@/components/ui/select"

import { useBoards } from "@/contexts/BoardsContext";

import { useParams, useRouter } from "next/navigation";
import { useAuth } from "@/contexts/AuthContext";

const API = "https://localhost:7177";

const Sidebar = () => {
  const { auth } = useAuth();
  const [isCollapsed, setIsCollapsed] = useState(false);
  const { boards, fetchBoards } = useBoards();
  const params = useParams();
  const router = useRouter();
  const selectedBoardId = params.id as string;
  const [columns, setColumns] = useState<any[]>([]);
  const headers: HeadersInit = {};
  if (auth.token) headers["Authorization"] = `Bearer ${auth.token}`;

  useEffect(() => {
    fetchData();
  }, [])

  const fetchData = async () => {
    try {
      const [columnRes] = await Promise.all([
        fetch(`${API}/api/Columns`, { headers }),
      ]);

      const columnData = (await columnRes.json());

      console.log("Columnas:", columnData);

      setColumns(columnData);
    } catch (err) {
      console.error("Error al cargar el tablero:", err);
    }
  };

  return (
    <aside
      className={cn(
        "p-4 transition-all duration-300 flex flex-col border-r",
        isCollapsed ? "w-16" : "w-64"
      )}
      style={{ maxHeight: "90vh", overflowY: "auto" }}
    >
      <Button
        variant="ghost"
        className="mb-4 flex items-center gap-2"
        onClick={() => setIsCollapsed(!isCollapsed)}
      >
        <Menu />
        {!isCollapsed && <span>Menú</span>}
      </Button>

      <nav className="space-y-2">
        {!isCollapsed &&
          <div className="flex flex-row gap-1">
            <Select defaultValue={selectedBoardId} onValueChange={(value) => router.push(`/tableros/${value}`)}>
              <SelectTrigger className="w-[180px]">
                <SelectValue placeholder="" />
              </SelectTrigger>
              <SelectContent>
                <SelectGroup>
                  <SelectLabel>Tableros disponibles</SelectLabel>

                  {boards.map((board) => (
                    <SelectItem
                      key={board.id}
                      value={board.id}
                    >
                      <Grid2x2 />
                      <div className="pl-1">
                        {board.name}
                      </div>
                    </SelectItem>
                  ))}

                </SelectGroup>
              </SelectContent>
            </Select>

            <Button
              variant="outline"
              className="mb-4 flex items-center gap-2"
              onClick={() => fetchBoards()}
            >
              <RefreshCw />
            </Button>
          </div>
        }
        <NavItem icon={<Home />} isCollapsed={isCollapsed}>Home</NavItem>

        <Accordion type="single" collapsible className="w-full">
          <AccordionItem value="settings">
            <AccordionTrigger className="flex items-center gap-2 w-full">
              <Settings />
              {!isCollapsed && <span>Configuración</span>}
              {!isCollapsed && <ChevronDown className="text-muted-foreground pointer-events-none size-5 shrink-0 translate-y-0.5 transition-transform duration-200" />
              }
            </AccordionTrigger>
            <AccordionContent className="pl-6">
              <NavItem icon={<ChevronRight />} isCollapsed={isCollapsed}>Perfil</NavItem>
              <NavItem icon={<ChevronRight />} isCollapsed={isCollapsed}>Cerrar sesion</NavItem>
            </AccordionContent>
          </AccordionItem>
        </Accordion>
      </nav>
    </aside >
  );
};

const NavItem = ({ icon, children, isCollapsed }: { icon: React.ReactNode; children: React.ReactNode; isCollapsed: boolean }) => (
  <div className="flex items-center gap-2 p-2 hover:bg-accent rounded-md cursor-pointer">
    {icon}
    {!isCollapsed && <span>{children}</span>}
  </div>
);

export default Sidebar;