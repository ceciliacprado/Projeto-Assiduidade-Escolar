export interface Usuario {
  email: string;
  senha: string;
}

export interface LoginRequest {
  email: string;
  senha: string;
}

export interface LoginResponse {
  token: string;
}

export interface Aluno {
  id?: number;
  nome: string;
  email: string;
  senha: string;
  matricula: string;
  disciplinaId?: number;
  disciplina?: Disciplina;
  criadoEm?: string;
}

export interface Professor {
  id?: number;
  nome: string;
  email: string;
  senha: string;
  especialidade: string;
  criadoEm?: string;
}

export interface Disciplina {
  id?: number;
  nome: string;
  codigo: string;
  cargaHoraria: number;
  professorId?: number;
  professor?: Professor;
  criadoEm?: string;
}

export interface Frequencia {
  id?: number;
  alunoId: number;
  disciplinaId: number;
  data: string;
  presente: boolean;
  aluno?: Aluno;
  disciplina?: Disciplina;
  criadoEm?: string;
}

export interface VincularDisciplinaRequest {
  alunoId: number;
  disciplinaId: number;
}

export interface AuthContextType {
  user: { email: string; role: string } | null;
  login: (token: string) => void;
  logout: () => void;
  isAuthenticated: boolean;
} 