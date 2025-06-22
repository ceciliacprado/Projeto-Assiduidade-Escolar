'use client';

import { useEffect, useState } from 'react';
import api from '@/services/api';
import { Disciplina } from '../../../types';

interface Aluno {
    id: number;
    nome: string;
    email: string;
    senha: string;
    criadoEm: string; 
    disciplinaId?: number | null;
    disciplina?: Disciplina | null;
}

export default function Alunos() {
  const [alunos, setAlunos] = useState<Aluno[]>([]);

  useEffect(() => {
    api.get<Aluno[]>('/alunos')
      .then((res) => setAlunos(res.data))
      .catch((err) => console.error(err));
  }, []);

  return (
    <div>
      <h1>Alunos</h1>
      <ul>
        {alunos.map((aluno) => (
          <li key={aluno.id}>{aluno.nome}</li>
        ))}
      </ul>
    </div>
  );
}