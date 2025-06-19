"use client";

import api from "@/services/api";
import { Usuario } from "@/types/usuario";
import { useRouter } from "next/navigation";
import React, { useState } from "react";

function Login() {
  const [email, setEmail] = useState("");
  const [senha, setSenha] = useState("");
  const router = useRouter();

  async function efetuarLogin(e: React.FormEvent) {
    e.preventDefault();
    const usuario: Usuario = {
      email,
      senha,
    };
    try {
      const resposta = await api.post("usuario/login", usuario);
      localStorage.setItem("token", resposta.data as string);
      router.push("/aluno/listar");
    } catch (erro) {
      console.error(erro);
      alert("Login ou senha incorretos!");
    }
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-100 p-4">
      <div className="w-full max-w-md bg-white rounded-lg shadow-md p-8">
        <div className="text-center mb-8">
          <img 
            src="https://img.icons8.com/color/48/000000/school.png" 
            alt="Logo" 
            className="mx-auto w-16 h-16"
          />
          <h1 className="text-2xl font-bold text-gray-800 mt-4">Escola Elite</h1>
          <p className="text-gray-600">Sistema de Controle de Assiduidade</p>
        </div>
        
        <form onSubmit={efetuarLogin} className="space-y-6">
          <div>
            <label htmlFor="email" className="block text-sm font-medium text-gray-700">
              E-mail
            </label>
            <input 
              type="email" 
              id="email" 
              name="email" 
              required 
              value={email}
              onChange={(e: React.ChangeEvent<HTMLInputElement>) => setEmail(e.target.value)}
              className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500"
            />
          </div>
          
          <div>
            <label htmlFor="password" className="block text-sm font-medium text-gray-700">
              Senha
            </label>
            <input 
              type="password" 
              id="password" 
              name="password" 
              required 
              value={senha}
              onChange={(e: React.ChangeEvent<HTMLInputElement>) => setSenha(e.target.value)}
              className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500"
            />
          </div>
          
          <div>
            <button 
              type="submit" 
              className="w-full flex justify-center py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
            >
              Entrar
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

export default Login;