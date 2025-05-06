import { RegisterForm } from '@/components/RegisterForm';

export default function Register() {
  return (
    <div className="flex h-screen w-full items-center justify-center">
      <RegisterForm className="w-full max-w-md p-4" />
    </div>
  );
}
