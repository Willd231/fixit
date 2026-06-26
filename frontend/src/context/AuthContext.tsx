import React, { createContext, useContext, useState, ReactNode } from 'react';
import { User } from '../types';

interface AuthContextValue {
  user?: User | null;
  signin: (email: string, password: string, remember?: boolean) => Promise<void>;
  signout: () => void;
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [user, setUser] = useState<User | null>({ id: 4, name: 'Admin User', email: 'admin@example.com', role: 'Administrator' });

  const signin = async (email: string) => {
    // Mock sign-in
    setUser({ id: 4, name: 'Admin User', email, role: 'Administrator' });
  };

  const signout = () => setUser(null);

  return <AuthContext.Provider value={{ user, signin, signout }}>{children}</AuthContext.Provider>;
};

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used within AuthProvider');
  return ctx;
}
