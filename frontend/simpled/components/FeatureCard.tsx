import { Card, CardContent } from "@/components/ui/card";
import Image from "next/image";
import type React from "react";

export interface FeatureCardProps {
    icon: React.ReactNode;
    title: string;
    description: string;
    imageUrl: string;
}

export default function FeatureCard({ icon, title, description, imageUrl }: FeatureCardProps) {
    return (
        <Card className="overflow-hidden border-none shadow-md transition-all hover:shadow-lg">
            <div className="aspect-video relative">
                <Image
                    src={imageUrl}
                    alt={title}
                    fill
                    className="object-cover"
                />
                <div className="absolute inset-0 bg-gradient-to-t from-black/60 to-transparent flex items-end p-6">
                    <div className="text-white">{icon}</div>
                </div>
            </div>
            <CardContent className="p-6">
                <h3 className="text-xl font-semibold mb-2">{title}</h3>
                <p className="text-muted-foreground">{description}</p>
            </CardContent>
        </Card>
    );
}