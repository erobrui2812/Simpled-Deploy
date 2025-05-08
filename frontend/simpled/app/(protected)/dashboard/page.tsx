'use client';

import { DashboardHeader } from '@/components/dashboard/dashboard-header';
import { DashboardStats } from '@/components/dashboard/dashboard-stats';
import { ProjectOverview } from '@/components/dashboard/project-overview';
import type { Activity } from '@/components/dashboard/recent-activity';
import { RecentActivity } from '@/components/dashboard/recent-activity';
import { TaskDistribution } from '@/components/dashboard/task-distribution';
import { TaskProgressChart } from '@/components/dashboard/task-progress-chart';
import { useAuth } from '@/contexts/AuthContext';
import { useBoards } from '@/contexts/BoardsContext';
import { useEffect, useState } from 'react';

export default function DashboardPage() {
  const { auth, userData } = useAuth();
  const { boards, fetchBoards } = useBoards();
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [stats, setStats] = useState<{
    totalTasks: number;
    completedTasks: number;
    inProgressTasks: number;
    delayedTasks: number;
    upcomingDeadlines: number;
  }>({
    totalTasks: 0,
    completedTasks: 0,
    inProgressTasks: 0,
    delayedTasks: 0,
    upcomingDeadlines: 0,
  });
  const [recentActivity, setRecentActivity] = useState<Activity[]>([]);

  useEffect(() => {
    const loadDashboardData = async () => {
      console.log('🟡 Iniciando carga del dashboard');
      setIsLoading(true);
      try {
        console.log('🔍 Token actual:', auth.token);

        if (auth.token) {
          console.log('✅ Token presente, obteniendo boards...');
          await fetchBoards();
          console.log('📦 Boards cargados correctamente');

          setStats({
            totalTasks: 48,
            completedTasks: 21,
            inProgressTasks: 15,
            delayedTasks: 4,
            upcomingDeadlines: 8,
          });

          setRecentActivity([
            {
              id: '1',
              type: 'task_created',
              user: { name: 'Ana García', imageUrl: '/placeholder.svg' },
              task: { title: 'Diseñar nueva interfaz' },
              board: { name: 'Proyecto Web' },
              timestamp: new Date(Date.now() - 1000 * 60 * 30).toISOString(),
            },
            {
              id: '2',
              type: 'task_completed',
              user: { name: 'Carlos Rodríguez', imageUrl: '/placeholder.svg' },
              task: { title: 'Implementar autenticación' },
              board: { name: 'Proyecto Web' },
              timestamp: new Date(Date.now() - 1000 * 60 * 120).toISOString(),
            },
            {
              id: '3',
              type: 'comment_added',
              user: { name: 'Laura Martínez', imageUrl: '/placeholder.svg' },
              task: { title: 'Optimizar rendimiento' },
              board: { name: 'Proyecto Móvil' },
              comment: { text: 'He identificado varios puntos de mejora en el rendimiento.' },
              timestamp: new Date(Date.now() - 1000 * 60 * 180).toISOString(),
            },
            {
              id: '4',
              type: 'task_status_changed',
              user: { name: 'Miguel Sánchez', imageUrl: '/placeholder.svg' },
              task: { title: 'Crear documentación' },
              board: { name: 'Proyecto Web' },
              oldStatus: 'pending',
              newStatus: 'in-progress',
              timestamp: new Date(Date.now() - 1000 * 60 * 240).toISOString(),
            },
            {
              id: '5',
              type: 'task_assigned',
              user: { name: 'Elena López', imageUrl: '/placeholder.svg' },
              task: { title: 'Diseñar logo' },
              board: { name: 'Proyecto Branding' },
              assignee: { name: 'Carlos Rodríguez' },
              timestamp: new Date(Date.now() - 1000 * 60 * 300).toISOString(),
            },
          ]);
          console.log('📊 Datos de dashboard simulados cargados');
        } else {
          console.warn('⛔ No hay token disponible. ¿Usuario no autenticado?');
        }
      } catch (error) {
        console.error('❌ Error cargando datos del dashboard:', error);
      } finally {
        console.log('🏁 Finalizando carga');
        setIsLoading(false);
      }
    };

    loadDashboardData();
  }, [auth.token]); // ✅ Eliminado fetchBoards para evitar bucle infinito

  if (isLoading) {
    return (
      <div className="flex h-screen items-center justify-center">
        <div className="border-foreground h-12 w-12 animate-spin rounded-full border-b-2" />
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <DashboardHeader user={userData} />

      <div className="mt-8">
        <DashboardStats stats={stats} />
      </div>

      <div className="mt-8 grid grid-cols-1 gap-8 lg:grid-cols-2">
        <TaskProgressChart />
        <TaskDistribution />
      </div>

      <div className="mt-8 grid grid-cols-1 gap-8 lg:grid-cols-3">
        <div className="lg:col-span-2">
          <ProjectOverview boards={boards} />
        </div>
        <div>
          <RecentActivity activities={recentActivity} />
        </div>
      </div>
    </div>
  );
}
