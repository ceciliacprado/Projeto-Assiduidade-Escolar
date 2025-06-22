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
  VincularDisciplinaRequest,
  StatusFrequencia
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
    // Usando o endpoint de login do professor
    const response = await api.post<string>('/professor/login', credentials);
    // A API retorna apenas o token como string, então precisamos encapsular
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
  
  cadastrar: async (turma: Turma): Promise<Turma> => {
    const response = await api.post<Turma>('/turma/cadastrar', turma);
    return response.data;
  },
  
  atualizar: async (id: number, turma: Turma): Promise<Turma> => {
    const response = await api.put<Turma>(`/turma/${id}`, turma);
    return response.data;
  },
  
  excluir: async (id: number): Promise<void> => {
    await api.delete(`/turma/${id}`);
  },
  
  listarAlunos: async (turmaId: number): Promise<Aluno[]> => {
    const response = await api.get<Aluno[]>(`/turma/${turmaId}/alunos`);
    return response.data;
  },
  
  listarDisciplinas: async (turmaId: number): Promise<Disciplina[]> => {
    const response = await api.get<Disciplina[]>(`/turma/${turmaId}/disciplinas`);
    return response.data;
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
  
  cadastrar: async (aluno: Aluno): Promise<Aluno> => {
    const response = await api.post<Aluno>('/aluno/cadastrar', aluno);
    return response.data;
  },
  
  atualizar: async (id: number, aluno: Aluno): Promise<Aluno> => {
    const response = await api.put<Aluno>(`/aluno/${id}`, aluno);
    return response.data;
  },
  
  excluir: async (id: number): Promise<void> => {
    await api.delete(`/aluno/${id}`);
  },
  
  vincularDisciplina: async (request: VincularDisciplinaRequest): Promise<Aluno> => {
    const response = await api.post<Aluno>('/aluno/vincular-disciplina', request);
    return response.data;
  },
  
  listarPorTurma: async (turmaId: number): Promise<Aluno[]> => {
    const response = await api.get<Aluno[]>(`/aluno/turma/${turmaId}`);
    return response.data;
  }
};

// Serviços de Professor
export const professorService = {
  listar: async (): Promise<Professor[]> => {
    const response = await api.get<Professor[]>('/professor/listar');
    return response.data;
  },
  
  buscarPorId: async (id: number): Promise<Professor> => {
    const response = await api.get<Professor>(`/professor/${id}`);
    return response.data;
  },
  
  cadastrar: async (professor: Professor): Promise<Professor> => {
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
  
  cadastrar: async (disciplina: Disciplina): Promise<Disciplina> => {
    const response = await api.post<Disciplina>('/disciplina/cadastrar', disciplina);
    return response.data;
  },
  
  atualizar: async (id: number, disciplina: Disciplina): Promise<Disciplina> => {
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
  
  listarPorProfessor: async (professorId: number): Promise<Disciplina[]> => {
    const response = await api.get<Disciplina[]>(`/disciplina/professor/${professorId}`);
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
  
  cadastrar: async (frequencia: Frequencia): Promise<Frequencia> => {
    const response = await api.post<Frequencia>('/frequencia/cadastrar', frequencia);
    return response.data;
  },
  
  atualizar: async (id: number, frequencia: Frequencia): Promise<Frequencia> => {
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
  
  registrarLote: async (frequencias: Frequencia[]): Promise<void> => {
    await api.post('/frequencia/registrar-lote', frequencias);
  }
};

export default api;