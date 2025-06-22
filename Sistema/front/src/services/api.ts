// src/services/api.ts
import axios from 'axios';
import { 
  LoginRequest, 
  LoginResponse, 
  Aluno, 
  Professor, 
  Disciplina, 
  Frequencia,
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
      window.location.href = '/usuario/login';
    }
    return Promise.reject(error);
  }
);

// Serviços de autenticação
export const authService = {
  login: async (credentials: LoginRequest): Promise<LoginResponse> => {
    const response = await api.post<LoginResponse>('/usuario/login', credentials);
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
  
  vincularDisciplina: async (request: VincularDisciplinaRequest): Promise<Aluno> => {
    const response = await api.post<Aluno>('/aluno/vincular-disciplina', request);
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
  
  listarPorAluno: async (alunoId: number): Promise<Frequencia[]> => {
    const response = await api.get<Frequencia[]>(`/frequencia/aluno/${alunoId}`);
    return response.data;
  }
};

export default api;