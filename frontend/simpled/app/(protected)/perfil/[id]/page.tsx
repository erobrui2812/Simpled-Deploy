'use client';

import AchievementCounter from '@/components/AchievementCounter';
import InvitationModal from '@/components/InvitationModal';
import ProfileHeader from '@/components/ProfileHeader';
import TeamsList from '@/components/TeamsList';
import { useAuth } from '@/contexts/AuthContext';
import { notFound } from 'next/navigation';
import { use, useEffect, useState } from 'react';

type User = {
  id: string | null;
  name: string;
  email: string;
  imageUrl: string;
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
  key: string;
  name: string;
  role: string;
}

export default function ProfilePage({ params }: { readonly params: Promise<{ id: string }> }) {
  const { id } = use(params);
  const { fetchUserProfile, auth } = useAuth();
  const [user, setUser] = useState<User | null>(null);
  const [showInvites, setShowInvites] = useState(false);

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

        {isOwner && (
          <div className="mt-4 flex justify-end">
            <button
              onClick={() => setShowInvites(true)}
              className="rounded bg-blue-600 px-4 py-2 text-white transition hover:bg-blue-700"
            >
              Ver invitaciones
            </button>
          </div>
        )}

        <div className="mt-8 grid grid-cols-1 gap-8 md:grid-cols-2">
          <AchievementCounter achievements={user.achievementsCompleted} userId={user.id} />
          <TeamsList teams={user.teams} />
        </div>
      </div>

      {showInvites && <InvitationModal onClose={() => setShowInvites(false)} />}
    </div>
  );
}
