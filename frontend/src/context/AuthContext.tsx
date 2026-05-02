import React, { createContext, useContext, useState } from 'react';
import { Role } from '../types';

interface User {
  username: string;
  role: Role;
}

interface AuthContextType {
  user: User | null;
  login: (username: string, password: string) => boolean;
  logout: () => void;
  hasRole: (...roles: Role[]) => boolean;
}

const AuthContext = createContext<AuthContextType | null>(null);

const ROLE_MAP: Record<string, Role> = {
  admin: 'Admin',
  manager: 'Manager',
  financecontroller: 'FinanceController',
  budgetuser: 'BudgetUser',
};

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [user, setUser] = useState<User | null>(() => {
    const stored = localStorage.getItem('auth_user');
    return stored ? JSON.parse(stored) : null;
  });

  const login = (username: string, _password: string): boolean => {
    const role = ROLE_MAP[username.toLowerCase()];
    if (!role) return false;
    const u = { username, role };
    setUser(u);
    localStorage.setItem('auth_user', JSON.stringify(u));
    localStorage.setItem('auth_token', `fake-jwt-token-${username}`);
    return true;
  };

  const logout = () => {
    setUser(null);
    localStorage.removeItem('auth_user');
    localStorage.removeItem('auth_token');
  };

  const hasRole = (...roles: Role[]) => {
    return user ? roles.includes(user.role) : false;
  };

  return (
    <AuthContext.Provider value={{ user, login, logout, hasRole }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used within AuthProvider');
  return ctx;
};
