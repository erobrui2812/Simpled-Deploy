'use client';
import { use } from 'react';
import { useState, useEffect } from 'react';
import { useAuth } from '@/contexts/AuthContext';
import { notFound } from 'next/navigation';
import ProfileHeader from '@/components/ProfileHeader';
import TeamsList from '@/components/TeamsList';
import AchievementCounter from '@/components/AchievementCounter';

type User = {
  id: string | null;
  name: string;
  email: string;
  photo: string;
  isOnline: boolean;
  achievementsCompleted: number;
  achievements: Achievement[];
  teams: Team[];
};

interface Achievement {
  id: string;
  title: string;
  description?: string;
}

interface Team {
  id: string;
  name: string;
  role: string;
}

export default function PerfilPage({ params }: { params: Promise<{ id: string }> }) {
  const { id } = use(params);
  const { fetchUserProfile, auth } = useAuth();
  const [user, setUser] = useState<User | null>(null);

  useEffect(() => {
    const loadUser = async () => {
      const data = await fetchUserProfile(id);
      setUser(data);
    };

    loadUser();
  }, [id, fetchUserProfile]);

  if (!user) {
    return <div className="py-12 text-center">Loading user profile...</div>;
  }

  if (!user.id) {
    notFound();
  }

  const isOwner = auth.id === user.id;

  return (
    <div className="container mx-auto min-h-screen px-4 py-8">
      <div className="mx-auto max-w-4xl">
        <ProfileHeader user={user as User & { id: string }} isOwner={isOwner} />
        <div className="mt-8 grid grid-cols-1 gap-8 md:grid-cols-2">
          <AchievementCounter achievements={user.achievementsCompleted} userId={user.id} />
          <TeamsList teams={user.teams} />
        </div>
      </div>
    </div>
  );
}
