import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Trophy } from 'lucide-react';
import Link from 'next/link';

interface AchievementCounterProps {
  readonly achievements: number;
  readonly userId: string;
}

export default function AchievementCounter({ achievements, userId }: AchievementCounterProps) {
  return (
    <Link href={`/perfil/${userId}/logros`} className="block">
      <Card className="h-full transition-shadow hover:shadow-lg">
        <CardHeader className="pb-2">
          <CardTitle className="flex items-center gap-2">
            <Trophy className="h-5 w-5 text-yellow-500" />
            Logros
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="flex flex-col items-center justify-center py-4">
            <div className="text-4xl font-bold">{achievements} / 21</div>
            <p className="mt-2 text-sm text-gray-500">Haz clic para ver todos tus logros</p>
          </div>
        </CardContent>
      </Card>
    </Link>
  );
}
