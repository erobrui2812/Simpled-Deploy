import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Star } from 'lucide-react';
import Link from 'next/link';
import { useRouter } from 'next/navigation';

interface FavoriteBoard {
  id: string;
  name: string;
}

interface FavoriteListProps {
  readonly list: FavoriteBoard[] | undefined;
}

export default function FavoriteList({ list }: FavoriteListProps) {
  const router = useRouter();
  if (!list || list.length === 0) {
    return (
      <Card className="h-full">
        <CardHeader className="pb-2">
          <CardTitle className="flex items-center gap-2">
            <Star className="h-5 w-5 text-yellow-300" />
            Tableros favoritos (0)
          </CardTitle>
        </CardHeader>
        <CardContent>
          <p className="text-muted-foreground text-sm">No hay tableros favoritos.</p>
        </CardContent>
      </Card>
    );
  }

  return (
    <Card className="h-full">
      <CardHeader className="pb-2">
        <CardTitle className="flex items-center gap-2">
          <Star className="h-5 w-5 text-yellow-300" />
          Tableros favoritos ({list.length})
        </CardTitle>
      </CardHeader>
      <CardContent>
        <ul className="space-y-3">
          {list.map((element) => (
            <li key={element.id}>
              <Link
                href={`/tableros/${element.id}`}
                className="block cursor-pointer transition hover:opacity-80"
              >
                <Card>
                  <CardContent className="flex items-center justify-between">
                    <span className="font-medium">{element.name}</span>
                  </CardContent>
                </Card>
              </Link>
            </li>
          ))}
        </ul>
      </CardContent>
    </Card>
  );
}
