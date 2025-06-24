'use client';

import React, { createContext, useContext, useEffect, useState } from 'react';
import { AuthContextType } from '../types';
import { jwtDecode } from 'jwt-decode';

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth deve ser usado dentro de um AuthProvider');
  }
  return context;
};

interface AuthProviderProps {
  children: React.ReactNode;
}

interface DecodedToken {
  name?: string;
  email?: string;
  role?: string;
  nome?: string;
  exp: number;
}

export const AuthProvider: React.FC<AuthProviderProps> = ({ children }) => {
  const [user, setUser] = useState<{ email: string; role: string; nome: string } | null>(null);
  const [isAuthenticated, setIsAuthenticated] = useState(false);

  useEffect(() => {
    // Verificar se há um token válido no localStorage
    const token = localStorage.getItem('token');
    if (token) {
      try {
        const decoded: DecodedToken = jwtDecode(token);
        const currentTime = Date.now() / 1000;
        
        if (decoded.exp > currentTime) {
          setUser({
            email: decoded.name || decoded.email || '',
            role: decoded.role || 'user',
            nome: decoded.nome || ''
          });
          setIsAuthenticated(true);
        } else {
          // Token expirado
          localStorage.removeItem('token');
          localStorage.removeItem('user');
        }
      } catch (error) {
        console.error('Erro ao decodificar token:', error);
        // Token inválido
        localStorage.removeItem('token');
        localStorage.removeItem('user');
      }
    }
  }, []);

  const login = (token: string) => {
    try {
      const decoded: DecodedToken = jwtDecode(token);
      console.log('Token decodificado:', decoded);
      
      // Tentar diferentes formas de acessar as claims
      const userInfo = {
        email: decoded.name || decoded.email || (decoded as any).email || '',
        role: decoded.role || (decoded as any).role || 'user',
        nome: decoded.nome || (decoded as any).nome || ''
      };
      
      console.log('UserInfo criado:', userInfo);
      
      localStorage.setItem('token', token);
      localStorage.setItem('user', JSON.stringify(userInfo));
      
      setUser(userInfo);
      setIsAuthenticated(true);
    } catch (error) {
      console.error('Erro ao decodificar token:', error);
    }
  };

  const logout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    setUser(null);
    setIsAuthenticated(false);
  };

  const value: AuthContextType = {
    user,
    login,
    logout,
    isAuthenticated
  };

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  );
}; 