'use client';

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
  CircularProgress
} from '@mui/material';
import {
  Add as AddIcon,
  Edit as EditIcon
} from '@mui/icons-material';
import { ProtectedRoute } from '../../../components/ProtectedRoute';
import { Layout } from '../../../components/Layout';
import { disciplinaService, professorService } from '../../../services/api';
import { Disciplina, Professor } from '../../../types';

export default function DisciplinasPage() {
  const [disciplinas, setDisciplinas] = useState<Disciplina[]>([]);
  const [professores, setProfessores] = useState<Professor[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [openDialog, setOpenDialog] = useState(false);
  const [editingDisciplina, setEditingDisciplina] = useState<Disciplina | null>(null);
  const [formData, setFormData] = useState({
    nome: '',
    codigo: '',
    cargaHoraria: '',
    professorId: ''
  });

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = async () => {
    try {
      const [disciplinasData, professoresData] = await Promise.all([
        disciplinaService.listar(),
        professorService.listar()
      ]);
      setDisciplinas(disciplinasData);
      setProfessores(professoresData);
    } catch (err: unknown) {
      console.error('Erro ao carregar dados:', err);
      setError('Erro ao carregar dados');
    } finally {
      setLoading(false);
    }
  };

  const handleOpenDialog = (disciplina?: Disciplina) => {
    if (disciplina) {
      setEditingDisciplina(disciplina);
      setFormData({
        nome: disciplina.nome,
        codigo: disciplina.codigo,
        cargaHoraria: disciplina.cargaHoraria.toString(),
        professorId: disciplina.professorId?.toString() || ''
      });
    } else {
      setEditingDisciplina(null);
      setFormData({
        nome: '',
        codigo: '',
        cargaHoraria: '',
        professorId: ''
      });
    }
    setOpenDialog(true);
  };

  const handleCloseDialog = () => {
    setOpenDialog(false);
    setEditingDisciplina(null);
  };

  const handleSubmit = async () => {
    try {
      const disciplinaData: Disciplina = {
        ...formData,
        cargaHoraria: parseInt(formData.cargaHoraria),
        professorId: formData.professorId ? parseInt(formData.professorId) : undefined
      };

      if (editingDisciplina) {
        // Atualizar disciplina existente
        // Implementar quando a API tiver endpoint de atualização
        setError('Funcionalidade de edição não implementada na API');
      } else {
        // Criar nova disciplina
        await disciplinaService.cadastrar(disciplinaData);
      }

      fetchData();
      handleCloseDialog();
    } catch (err: unknown) {
      const error = err as { response?: { data?: { mensagem?: string } } };
      setError(error.response?.data?.mensagem || 'Erro ao salvar disciplina');
    }
  };

  const getProfessorNome = (professorId?: number) => {
    if (!professorId) return 'Não atribuído';
    const professor = professores.find(p => p.id === professorId);
    return professor?.nome || 'Professor não encontrado';
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
              Disciplinas
            </Typography>
            <Button
              variant="contained"
              startIcon={<AddIcon />}
              onClick={() => handleOpenDialog()}
            >
              Nova Disciplina
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
                  <TableCell>Código</TableCell>
                  <TableCell>Carga Horária</TableCell>
                  <TableCell>Professor</TableCell>
                  <TableCell>Data de Cadastro</TableCell>
                  <TableCell>Ações</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {disciplinas.map((disciplina) => (
                  <TableRow key={disciplina.id}>
                    <TableCell>{disciplina.nome}</TableCell>
                    <TableCell>{disciplina.codigo}</TableCell>
                    <TableCell>{disciplina.cargaHoraria}h</TableCell>
                    <TableCell>{getProfessorNome(disciplina.professorId)}</TableCell>
                    <TableCell>
                      {disciplina.criadoEm ? new Date(disciplina.criadoEm).toLocaleDateString('pt-BR') : '-'}
                    </TableCell>
                    <TableCell>
                      <IconButton
                        size="small"
                        onClick={() => handleOpenDialog(disciplina)}
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
              {editingDisciplina ? 'Editar Disciplina' : 'Nova Disciplina'}
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
                  fullWidth
                  label="Código"
                  value={formData.codigo}
                  onChange={(e) => setFormData({ ...formData, codigo: e.target.value })}
                  margin="normal"
                  required
                />
                <TextField
                  fullWidth
                  label="Carga Horária (horas)"
                  type="number"
                  value={formData.cargaHoraria}
                  onChange={(e) => setFormData({ ...formData, cargaHoraria: e.target.value })}
                  margin="normal"
                  required
                />
                <TextField
                  select
                  fullWidth
                  label="Professor"
                  value={formData.professorId}
                  onChange={(e) => setFormData({ ...formData, professorId: e.target.value })}
                  margin="normal"
                >
                  <option value="">Selecione um professor</option>
                  {professores.map((professor) => (
                    <option key={professor.id} value={professor.id}>
                      {professor.nome} - {professor.especialidade}
                    </option>
                  ))}
                </TextField>
              </Box>
            </DialogContent>
            <DialogActions>
              <Button onClick={handleCloseDialog}>Cancelar</Button>
              <Button onClick={handleSubmit} variant="contained">
                {editingDisciplina ? 'Atualizar' : 'Cadastrar'}
              </Button>
            </DialogActions>
          </Dialog>
        </Box>
      </Layout>
    </ProtectedRoute>
  );
} 