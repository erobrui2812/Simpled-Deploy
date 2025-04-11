'use client'

import { useEffect, useState } from 'react'
import { useRouter } from 'next/navigation'
import { useAuth } from '@/contexts/AuthContext'

export default function Layout({
  children,
}: {
  children: React.ReactNode
}) {
  const { isAuthenticated } = useAuth()
  const router = useRouter()
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    if (!isAuthenticated) {
      const timeout = setTimeout(() => {
        router.push('/')
      }, 1500)

      return () => clearTimeout(timeout)
    }

    setLoading(false)
  }, [isAuthenticated, router])

  if (loading) {
    return (
      <div className="flex items-center flex-col justify-center h-screen">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-foreground" />
        <p className="mt-4 text-foreground text-sm">Redirigiendo a inicio...</p>

      </div>
    )
  }

  return <>{children}</>
}
