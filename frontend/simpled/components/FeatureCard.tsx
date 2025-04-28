import { Card, CardContent } from '@/components/ui/card';
import Image from 'next/image';
import type React from 'react';

export interface FeatureCardProps {
  readonly icon: React.ReactNode;
  readonly title: string;
  readonly description: string;
  readonly imageUrl: string;
}

export default function FeatureCard({ icon, title, description, imageUrl }: FeatureCardProps) {
  return (
    <Card className="overflow-hidden border-none shadow-md transition-all hover:shadow-lg">
      <div className="relative aspect-video">
        <Image src={imageUrl} alt={title} fill className="object-cover" />
        <div className="absolute inset-0 flex items-end bg-gradient-to-t from-black/60 to-transparent p-6">
          <div className="text-white">{icon}</div>
        </div>
      </div>
      <CardContent className="p-6">
        <h3 className="mb-2 text-xl font-semibold">{title}</h3>
        <p className="text-muted-foreground">{description}</p>
      </CardContent>
    </Card>
  );
}
