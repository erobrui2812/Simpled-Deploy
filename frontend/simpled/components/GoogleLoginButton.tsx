'use client';

import { useAuth } from '@/contexts/AuthContext';
import { GoogleLogin } from '@react-oauth/google';
import { toast } from 'react-toastify';

type GoogleLoginButtonProps = Readonly<{
  onSuccess?: () => void;
  text?: 'signin_with' | 'signup_with' | 'continue_with' | 'signin';
  shape?: 'rectangular' | 'pill' | 'circle' | 'square';
  theme?: 'outline' | 'filled_blue' | 'filled_black';
  size?: 'large' | 'medium' | 'small';
  locale?: string;
  className?: string;
}>;

export default function GoogleLoginButton({
  onSuccess,
  text = 'continue_with',
  shape = 'pill',
  theme = 'filled_blue',
  size = 'large',
  locale = 'es',
  className,
}: GoogleLoginButtonProps) {
  const { loginWithGoogle } = useAuth();

  const handleGoogleLogin = async (credentialResponse: any) => {
    const idToken = credentialResponse.credential;
    if (idToken) {
      try {
        await loginWithGoogle(idToken);
        if (onSuccess) onSuccess();
      } catch (error) {
        console.error('Error al iniciar sesión con Google:', error);
        toast.error('Error al iniciar sesión con Google.');
      }
    }
  };

  return (
    <div className={className}>
      <GoogleLogin
        onSuccess={handleGoogleLogin}
        onError={() => {
          toast.error('Error al iniciar sesión con Google.');
        }}
        theme={theme}
        shape={shape}
        size={size}
        text={text}
        locale={locale}
      />
    </div>
  );
}
