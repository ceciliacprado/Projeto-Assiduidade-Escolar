"use client";

import React, { useEffect, useState } from 'react';
import {
  Box,
  Typography,
  Button,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  IconButton,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  Alert,
  CircularProgress,
  Chip
} from '@mui/material';
import {
  Add as AddIcon,
  Edit as EditIcon
} from '@mui/icons-material';
import { ProtectedRoute } from '../../../components/ProtectedRoute';
import { Layout } from '../../../components/Layout';
import { alunoService, disciplinaService } from '../../../services/api';
import { Aluno, Disciplina } from '../../../types';

export default function AlunosPage() {
  const [alunos, setAlunos] = useState<Aluno[]>([]);
  const [disciplinas, setDisciplinas] = useState<Disciplina[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [openDialog, setOpenDialog] = useState(false);
  const [editingAluno, setEditingAluno] = useState<Aluno | null>(null);
  const [formData, setFormData] = useState({
    nome: '',
    email: '',
    senha: '',
    matricula: '',
    disciplinaId: ''
  });

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = async () => {
    try {
      const [alunosData, disciplinasData] = await Promise.all([
        alunoService.listar(),
        disciplinaService.listar()
      ]);
      setAlunos(alunosData);
      setDisciplinas(disciplinasData);
    } catch (err: unknown) {
      console.error('Erro ao carregar dados:', err);
      setError('Erro ao carregar dados');
    } finally {
      setLoading(false);
    }
  };

  const handleOpenDialog = (aluno?: Aluno) => {
    if (aluno) {
      setEditingAluno(aluno);
      setFormData({
        nome: aluno.nome,
        email: aluno.email,
        senha: aluno.senha,
        matricula: aluno.matricula,
        disciplinaId: aluno.disciplinaId?.toString() || ''
      });
    } else {
      setEditingAluno(null);
      setFormData({
        nome: '',
        email: '',
        senha: '',
        matricula: '',
        disciplinaId: ''
      });
    }
    setOpenDialog(true);
  };

  const handleCloseDialog = () => {
    setOpenDialog(false);
    setEditingAluno(null);
  };

  const handleSubmit = async () => {
    try {
      const alunoData: Aluno = {
        ...formData,
        disciplinaId: formData.disciplinaId ? parseInt(formData.disciplinaId) : undefined
      };

      if (editingAluno) {
        // Atualizar aluno existente
        await alunoService.vincularDisciplina({
          alunoId: editingAluno.id!,
          disciplinaId: parseInt(formData.disciplinaId)
        });
      } else {
        // Criar novo aluno
        await alunoService.cadastrar(alunoData);
      }

      fetchData();
      handleCloseDialog();
    } catch (err: unknown) {
      const error = err as { response?: { data?: { mensagem?: string } } };
      setError(error.response?.data?.mensagem || 'Erro ao salvar aluno');
    }
  };

  const getDisciplinaNome = (disciplinaId?: number) => {
    if (!disciplinaId) return 'Não vinculado';
    const disciplina = disciplinas.find(d => d.id === disciplinaId);
    return disciplina?.nome || 'Disciplina não encontrada';
  };

  if (loading) {
    return (
      <ProtectedRoute>
        <Layout>
          <Box display="flex" justifyContent="center" alignItems="center" minHeight="60vh">
            <CircularProgress />
          </Box>
        </Layout>
      </ProtectedRoute>
    );
  }

  return (
    <ProtectedRoute>
      <Layout>
        <Box>
          <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
            <Typography variant="h4" component="h1">
              Alunos
            </Typography>
            <Button
              variant="contained"
              startIcon={<AddIcon />}
              onClick={() => handleOpenDialog()}
            >
              Novo Aluno
            </Button>
          </Box>

          {error && (
            <Alert severity="error" sx={{ mb: 3 }}>
              {error}
            </Alert>
          )}

          <TableContainer component={Paper}>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell>Nome</TableCell>
                  <TableCell>Email</TableCell>
                  <TableCell>Matrícula</TableCell>
                  <TableCell>Disciplina</TableCell>
                  <TableCell>Ações</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {alunos.map((aluno) => (
                  <TableRow key={aluno.id}>
                    <TableCell>{aluno.nome}</TableCell>
                    <TableCell>{aluno.email}</TableCell>
                    <TableCell>{aluno.matricula}</TableCell>
                    <TableCell>
                      <Chip
                        label={getDisciplinaNome(aluno.disciplinaId)}
                        color={aluno.disciplinaId ? 'success' : 'default'}
                        size="small"
                      />
                    </TableCell>
                    <TableCell>
                      <IconButton
                        size="small"
                        onClick={() => handleOpenDialog(aluno)}
                        title="Editar"
                      >
                        <EditIcon />
                      </IconButton>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </TableContainer>

          <Dialog open={openDialog} onClose={handleCloseDialog} maxWidth="sm" fullWidth>
            <DialogTitle>
              {editingAluno ? 'Vincular Disciplina' : 'Novo Aluno'}
            </DialogTitle>
            <DialogContent>
              <Box sx={{ pt: 1 }}>
                {editingAluno ? (
                  <TextField
                    select
                    fullWidth
                    label="Disciplina"
                    value={formData.disciplinaId}
                    onChange={(e) => setFormData({ ...formData, disciplinaId: e.target.value })}
                    margin="normal"
                  >
                    <option value="">Selecione uma disciplina</option>
                    {disciplinas.map((disciplina) => (
                      <option key={disciplina.id} value={disciplina.id}>
                        {disciplina.nome}
                      </option>
                    ))}
                  </TextField>
                ) : (
                  <>
                    <TextField
                      fullWidth
                      label="Nome"
                      value={formData.nome}
                      onChange={(e) => setFormData({ ...formData, nome: e.target.value })}
                      margin="normal"
                      required
                    />
                    <TextField
                      fullWidth
                      label="Email"
                      type="email"
                      value={formData.email}
                      onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                      margin="normal"
                      required
                    />
                    <TextField
                      fullWidth
                      label="Senha"
                      type="password"
                      value={formData.senha}
                      onChange={(e) => setFormData({ ...formData, senha: e.target.value })}
                      margin="normal"
                      required
                    />
                    <TextField
                      fullWidth
                      label="Matrícula"
                      value={formData.matricula}
                      onChange={(e) => setFormData({ ...formData, matricula: e.target.value })}
                      margin="normal"
                      required
                    />
                    <TextField
                      select
                      fullWidth
                      label="Disciplina"
                      value={formData.disciplinaId}
                      onChange={(e) => setFormData({ ...formData, disciplinaId: e.target.value })}
                      margin="normal"
                    >
                      <option value="">Selecione uma disciplina</option>
                      {disciplinas.map((disciplina) => (
                        <option key={disciplina.id} value={disciplina.id}>
                          {disciplina.nome}
                        </option>
                      ))}
                    </TextField>
                  </>
                )}
              </Box>
            </DialogContent>
            <DialogActions>
              <Button onClick={handleCloseDialog}>Cancelar</Button>
              <Button onClick={handleSubmit} variant="contained">
                {editingAluno ? 'Vincular' : 'Cadastrar'}
              </Button>
            </DialogActions>
          </Dialog>
        </Box>
      </Layout>
    </ProtectedRoute>
  );
}