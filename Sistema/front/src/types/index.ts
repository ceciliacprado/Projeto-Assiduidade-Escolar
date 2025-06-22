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

export interface Turma {
  id?: number;
  nome: string;
  ano: string;
  serie: string;
  criadoEm?: string;
}

export interface Aluno {
  id?: number;
  nome: string;
  turmaId?: number;
  turma?: Turma;
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
  role: string;
  criadoEm?: string;
}

export interface Disciplina {
  id?: number;
  nome: string;
  codigo: string;
  cargaHoraria: number;
  turmaId?: number;
  turma?: Turma;
  professorId?: number;
  professor?: Professor;
  criadoEm?: string;
}

export enum StatusFrequencia {
  Presente = 1,
  Ausente = 2,
  Atraso = 3
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