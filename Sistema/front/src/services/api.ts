// src/services/api.ts
import axios from 'axios';
import { 
  LoginRequest, 
  LoginResponse, 
  Aluno, 
  Professor, 
  Disciplina, 
  Frequencia,
  Turma,
  VincularDisciplinaRequest
} from '../types';

const api = axios.create({
  baseURL: 'http://localhost:5268/api', // Ajuste conforme o ambiente
});

// Interceptor de requisição: adiciona o token automaticamente
api.interceptors.request.use((config) => {
  if (typeof window !== 'undefined') {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers = config.headers || {};
      config.headers.Authorization = `Bearer ${token}`;
    }
  }
  return config;
});

// Interceptor de resposta: trata erro de autenticação
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (typeof window !== 'undefined' && error.response?.status === 401) {  
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      window.location.href = '/professor/login';
    }
    return Promise.reject(error);
  }
);

// Serviços de autenticação
export const authService = {
  login: async (credentials: LoginRequest): Promise<LoginResponse> => {
    const response = await api.post<string>('/professor/login', credentials);
    return { token: response.data };
  }
};

// Serviços de Turma
export const turmaService = {
  listar: async (): Promise<Turma[]> => {
    const response = await api.get<Turma[]>('/turma/listar');
    return response.data;
  },
  
  buscarPorId: async (id: number): Promise<Turma> => {
    const response = await api.get<Turma>(`/turma/${id}`);
    return response.data;
  },
  
  buscarComAlunosEDisciplinas: async (id: number): Promise<Turma> => {
    const response = await api.get<Turma>(`/turma/${id}`);
    return response.data;
  },
  
  cadastrar: async (turma: Omit<Turma, 'id' | 'criadoEm'>): Promise<Turma> => {
    const response = await api.post<Turma>('/turma/cadastrar', turma);
    return response.data;
  },
  
  atualizar: async (id: number, turma: Omit<Turma, 'id' | 'criadoEm'>): Promise<Turma> => {
    const response = await api.put<Turma>(`/turma/${id}`, turma);
    return response.data;
  },
  
  excluir: async (id: number): Promise<void> => {
    await api.delete(`/turma/${id}`);
  }
};

// Serviços de Aluno
export const alunoService = {
  listar: async (): Promise<Aluno[]> => {
    const response = await api.get<Aluno[]>('/aluno/listar');
    return response.data;
  },
  
  buscarPorId: async (id: number): Promise<Aluno> => {
    const response = await api.get<Aluno>(`/aluno/${id}`);
    return response.data;
  },
  
  cadastrar: async (aluno: Omit<Aluno, 'id' | 'criadoEm' | 'turma'>): Promise<Aluno> => {
    const response = await api.post<Aluno>('/aluno/cadastrar', aluno);
    return response.data;
  },
  
  atualizar: async (id: number, aluno: Omit<Aluno, 'id' | 'criadoEm' | 'turma'>): Promise<Aluno> => {
    const response = await api.put<Aluno>(`/aluno/${id}`, aluno);
    return response.data;
  },
  
  excluir: async (id: number): Promise<void> => {
    await api.delete(`/aluno/${id}`);
  },
  
  listarPorTurma: async (turmaId: number): Promise<Aluno[]> => {
    console.log('Chamando alunoService.listarPorTurma com turmaId:', turmaId);
    const response = await api.get<Aluno[]>(`/aluno/turma/${turmaId}`);
    console.log('Resposta do alunoService.listarPorTurma:', response.data);
    return response.data;
  },
  
  listarPorNomeTurma: async (nomeTurma: string): Promise<{ turma: { id: number; nome: string }; alunos: Aluno[] }> => {
    console.log('Chamando alunoService.listarPorNomeTurma com nomeTurma:', nomeTurma);
    const response = await api.get<{ turma: { id: number; nome: string }; alunos: Aluno[] }>(`/aluno/turma-nome/${nomeTurma}`);
    console.log('Resposta do alunoService.listarPorNomeTurma:', response.data);
    return response.data;
  },
  
};

// Serviços de Professor
export const professorService = {
  listar: async (): Promise<Professor[]> => {
    const response = await api.get<Professor[]>('/professor/listar');
    return response.data;
  },
  
  cadastrar: async (professor: Omit<Professor, 'id' | 'criadoEm' | 'role'>): Promise<Professor> => {
    const response = await api.post<Professor>('/professor/cadastrar', professor);
    return response.data;
  }
};

// Serviços de Disciplina
export const disciplinaService = {
  listar: async (): Promise<Disciplina[]> => {
    const response = await api.get<Disciplina[]>('/disciplina/listar');
    return response.data;
  },
  
  buscarPorId: async (id: number): Promise<Disciplina> => {
    const response = await api.get<Disciplina>(`/disciplina/${id}`);
    return response.data;
  },
  
  cadastrar: async (disciplina: Omit<Disciplina, 'id' | 'criadoEm' | 'turma'>): Promise<{ disciplina: Disciplina; alunosVinculados: number; mensagem: string }> => {
    const response = await api.post<{ disciplina: Disciplina; alunosVinculados: number; mensagem: string }>('/disciplina/cadastrar', disciplina);
    return response.data;
  },
  
  atualizar: async (id: number, disciplina: Omit<Disciplina, 'id' | 'criadoEm' | 'turma'>): Promise<Disciplina> => {
    const response = await api.put<Disciplina>(`/disciplina/${id}`, disciplina);
    return response.data;
  },
  
  excluir: async (id: number): Promise<void> => {
    await api.delete(`/disciplina/${id}`);
  },
  
  listarPorTurma: async (turmaId: number): Promise<Disciplina[]> => {
    const response = await api.get<Disciplina[]>(`/disciplina/turma/${turmaId}`);
    return response.data;
  },
  
  vincularAlunosExistente: async (disciplinaId: number): Promise<{ mensagem: string; disciplina: string; totalVinculados: number; totalAlunos: number }> => {
    const response = await api.post<{ mensagem: string; disciplina: string; totalVinculados: number; totalAlunos: number }>(`/disciplina/vincular-alunos-existente/${disciplinaId}`);
    return response.data;
  }
};

// Serviços de Frequência
export const frequenciaService = {
  listar: async (): Promise<Frequencia[]> => {
    const response = await api.get<Frequencia[]>('/frequencia/listar');
    return response.data;
  },
  
  buscarPorId: async (id: number): Promise<Frequencia> => {
    const response = await api.get<Frequencia>(`/frequencia/${id}`);
    return response.data;
  },
  
  registrar: async (frequencia: Omit<Frequencia, 'id' | 'criadoEm' | 'aluno' | 'disciplina'>): Promise<Frequencia> => {
    const response = await api.post<Frequencia>('/frequencia/registrar', frequencia);
    return response.data;
  },
  
  atualizar: async (id: number, frequencia: Omit<Frequencia, 'id' | 'criadoEm' | 'aluno' | 'disciplina'>): Promise<Frequencia> => {
    const response = await api.put<Frequencia>(`/frequencia/${id}`, frequencia);
    return response.data;
  },
  
  excluir: async (id: number): Promise<void> => {
    await api.delete(`/frequencia/${id}`);
  },
  
  listarPorAluno: async (alunoId: number): Promise<Frequencia[]> => {
    const response = await api.get<Frequencia[]>(`/frequencia/aluno/${alunoId}`);
    return response.data;
  },
  
  listarPorDisciplina: async (disciplinaId: number): Promise<Frequencia[]> => {
    const response = await api.get<Frequencia[]>(`/frequencia/disciplina/${disciplinaId}`);
    return response.data;
  },
  
  listarPorData: async (data: string): Promise<Frequencia[]> => {
    const response = await api.get<Frequencia[]>(`/frequencia/data/${data}`);
    return response.data;
  },
  
  registrarLote: async (frequencias: Omit<Frequencia, 'id' | 'criadoEm' | 'aluno' | 'disciplina'>[]): Promise<any> => {
    const response = await api.post<any>('/frequencia/registrar-lote', frequencias);
    return response.data;
  }
};

export default api;