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
  Chip,
  FormControl,
  InputLabel,
  Select,
  MenuItem
} from '@mui/material';
import {
  Add as AddIcon,
  Edit as EditIcon,
  FilterList as FilterIcon
} from '@mui/icons-material';
import { ProtectedRoute } from '../../../components/ProtectedRoute';
import { Layout } from '../../../components/Layout';
import { alunoService, turmaService } from '../../../services/api';
import { Aluno, Turma } from '../../../types';

export default function AlunosPage() {
  const [alunos, setAlunos] = useState<Aluno[]>([]);
  const [turmas, setTurmas] = useState<Turma[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [openDialog, setOpenDialog] = useState(false);
  const [editingAluno, setEditingAluno] = useState<Aluno | null>(null);
  const [selectedTurmaId, setSelectedTurmaId] = useState<string>('');
  const [formData, setFormData] = useState({
    nome: '',
    turmaId: ''
  });

  useEffect(() => {
    fetchData();
  }, []);

  useEffect(() => {
    if (selectedTurmaId) {
      fetchAlunosPorTurma(parseInt(selectedTurmaId));
    } else {
      fetchAlunos();
    }
  }, [selectedTurmaId]);

  const fetchData = async () => {
    try {
      const turmasData = await turmaService.listar();
      setTurmas(turmasData);
    } catch (err: unknown) {
      console.error('Erro ao carregar turmas:', err);
      setError('Erro ao carregar turmas');
    } finally {
      setLoading(false);
    }
  };

  const fetchAlunos = async () => {
    try {
      const alunosData = await alunoService.listar();
      setAlunos(alunosData);
    } catch (err: unknown) {
      console.error('Erro ao carregar alunos:', err);
      setError('Erro ao carregar alunos');
    }
  };

  const fetchAlunosPorTurma = async (turmaId: number) => {
    try {
      console.log('Carregando alunos da turma:', turmaId);
      const alunosData = await alunoService.listarPorTurma(turmaId);
      console.log('Alunos retornados:', alunosData);
      setAlunos(alunosData);
    } catch (err: unknown) {
      console.error('Erro detalhado ao carregar alunos da turma:', err);
      console.error('Tipo do erro:', typeof err);
      console.error('Mensagem do erro:', (err as Error).message);
      if ((err as { response?: unknown }).response) {
        console.error('Response do erro:', (err as { response?: unknown }).response);
      }
      setError('Erro ao carregar alunos da turma');
    }
  };

  const handleOpenDialog = (aluno?: Aluno) => {
    if (aluno) {
      setEditingAluno(aluno);
      setFormData({
        nome: aluno.nome,
        turmaId: aluno.turmaId.toString()
      });
    } else {
      setEditingAluno(null);
      setFormData({
        nome: '',
        turmaId: selectedTurmaId || ''
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
      const alunoData = {
        nome: formData.nome,
        turmaId: parseInt(formData.turmaId)
      };

      if (editingAluno) {
        await alunoService.atualizar(editingAluno.id!, alunoData);
      } else {
        await alunoService.cadastrar(alunoData);
      }

      // Recarregar dados baseado no filtro atual
      if (selectedTurmaId) {
        fetchAlunosPorTurma(parseInt(selectedTurmaId));
      } else {
        fetchAlunos();
      }
      
      handleCloseDialog();
    } catch (err: unknown) {
      const error = err as { response?: { data?: { mensagem?: string } } };
      setError(error.response?.data?.mensagem || 'Erro ao salvar aluno');
    }
  };

  const getTurmaNome = (turmaId: number) => {
    const turma = turmas.find(t => t.id === turmaId);
    return turma?.nome || 'Turma não encontrada';
  };

  const getDisciplinasCount = () => {
    return 0; // Alunos não têm mais disciplinas diretamente
  };

  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  const handleTurmaChange = (event: any) => {
    setSelectedTurmaId(event.target.value);
  };

  const clearFilter = () => {
    setSelectedTurmaId('');
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

          {/* Filtro por Turma */}
          <Paper sx={{ p: 2, mb: 3 }}>
            <Box sx={{ display: 'flex', gap: 2, alignItems: 'center', flexWrap: 'wrap' }}>
              <FormControl sx={{ minWidth: 200 }}>
                <InputLabel>Filtrar por Turma</InputLabel>
                <Select
                  value={selectedTurmaId}
                  label="Filtrar por Turma"
                  onChange={handleTurmaChange}
                  startAdornment={<FilterIcon sx={{ mr: 1 }} />}
                >
                  <MenuItem value="">
                    <em>Todas as Turmas</em>
                  </MenuItem>
                  {turmas.map((turma) => (
                    <MenuItem key={turma.id} value={turma.id}>
                      {turma.nome} - {turma.serie}
                    </MenuItem>
                  ))}
                </Select>
              </FormControl>
              <Button
                variant="outlined"
                onClick={clearFilter}
                disabled={!selectedTurmaId}
              >
                Limpar Filtro
              </Button>
            </Box>
          </Paper>

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
                  <TableCell>Turma</TableCell>
                  <TableCell>Disciplinas</TableCell>
                  <TableCell>Ações</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {alunos.length === 0 ? (
                  <TableRow>
                    <TableCell colSpan={4} align="center">
                      {selectedTurmaId 
                        ? 'Nenhum aluno encontrado nesta turma' 
                        : 'Nenhum aluno cadastrado'
                      }
                    </TableCell>
                  </TableRow>
                ) : (
                  alunos.map((aluno) => (
                    <TableRow key={aluno.id}>
                      <TableCell>{aluno.nome}</TableCell>
                      <TableCell>
                        <Chip
                          label={getTurmaNome(aluno.turmaId)}
                          color="primary"
                          size="small"
                        />
                      </TableCell>
                      <TableCell>
                        <Chip
                          label={`${getDisciplinasCount()} disciplinas`}
                          color="secondary"
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
                  ))
                )}
              </TableBody>
            </Table>
          </TableContainer>

          <Dialog open={openDialog} onClose={handleCloseDialog} maxWidth="sm" fullWidth>
            <DialogTitle>
              {editingAluno ? 'Editar Aluno' : 'Novo Aluno'}
            </DialogTitle>
            <DialogContent>
              <Box sx={{ pt: 1 }}>
                <TextField
                  fullWidth
                  label="Nome"
                  value={formData.nome}
                  onChange={(e) => setFormData({ ...formData, nome: e.target.value })}
                  margin="normal"
                  required
                />
                <TextField
                  select
                  fullWidth
                  label="Turma"
                  value={formData.turmaId}
                  onChange={(e) => setFormData({ ...formData, turmaId: e.target.value })}
                  margin="normal"
                  required
                >
                  {turmas.map((turma) => (
                    <option key={turma.id} value={turma.id}>
                      {turma.nome} - {turma.serie}
                    </option>
                  ))}
                </TextField>
              </Box>
            </DialogContent>
            <DialogActions>
              <Button onClick={handleCloseDialog}>Cancelar</Button>
              <Button onClick={handleSubmit} variant="contained">
                {editingAluno ? 'Atualizar' : 'Cadastrar'}
              </Button>
            </DialogActions>
          </Dialog>
        </Box>
      </Layout>
    </ProtectedRoute>
  );
}