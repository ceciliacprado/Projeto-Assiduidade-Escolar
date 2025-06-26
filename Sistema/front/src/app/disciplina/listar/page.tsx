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
  CircularProgress,
  FormControl,
  InputLabel,
  Select,
  MenuItem
} from '@mui/material';
import {
  Add as AddIcon,
  Edit as EditIcon
} from '@mui/icons-material';
import { ProtectedRoute } from '../../../components/ProtectedRoute';
import { Layout } from '../../../components/Layout';
import { disciplinaService, turmaService } from '../../../services/api';
import { Disciplina, Turma } from '../../../types';

export default function DisciplinasPage() {
  const [disciplinas, setDisciplinas] = useState<Disciplina[]>([]);
  const [turmas, setTurmas] = useState<Turma[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [openDialog, setOpenDialog] = useState(false);
  const [editingDisciplina, setEditingDisciplina] = useState<Disciplina | null>(null);
  const [formData, setFormData] = useState({
    nome: '',
    codigo: '',
    cargaHoraria: '',
    turmaId: ''
  });

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = async () => {
    try {
      const [disciplinasData, turmasData] = await Promise.all([
        disciplinaService.listar(),
        turmaService.listar()
      ]);
      setDisciplinas(disciplinasData);
      setTurmas(turmasData);
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
        turmaId: disciplina.turmaId.toString()
      });
    } else {
      setEditingDisciplina(null);
      setFormData({
        nome: '',
        codigo: '',
        cargaHoraria: '',
        turmaId: ''
      });
    }
    setError(''); // Limpar erros ao abrir
    setOpenDialog(true);
  };

  const handleCloseDialog = () => {
    setOpenDialog(false);
    setEditingDisciplina(null);
    setFormData({
      nome: '',
      codigo: '',
      cargaHoraria: '',
      turmaId: ''
    });
    setError(''); // Limpar erros ao fechar
    setSuccess(''); // Limpar sucesso ao fechar
  };

  const handleSubmit = async () => {
    try {
      // Validação dos campos
      if (!formData.nome.trim()) {
        setError('Nome da disciplina é obrigatório');
        return;
      }
      
      if (!formData.codigo.trim()) {
        setError('Código da disciplina é obrigatório');
        return;
      }
      
      if (!formData.cargaHoraria || parseInt(formData.cargaHoraria) <= 0) {
        setError('Carga horária deve ser maior que zero');
        return;
      }
      
      if (!formData.turmaId) {
        setError('Selecione uma turma');
        return;
      }

      const disciplinaData = {
        nome: formData.nome.trim(),
        codigo: formData.codigo.trim(),
        cargaHoraria: parseInt(formData.cargaHoraria),
        turmaId: parseInt(formData.turmaId)
      };

      if (editingDisciplina) {
        await disciplinaService.atualizar(editingDisciplina.id!, disciplinaData);
        setSuccess('Disciplina atualizada com sucesso!');
      } else {
        const resultado = await disciplinaService.cadastrar(disciplinaData);
        setSuccess(resultado.mensagem);
      }

      fetchData();
      setError(''); // Limpar erros
      handleCloseDialog();
    } catch (err: unknown) {
      const error = err as { response?: { data?: { mensagem?: string } } };
      setError(error.response?.data?.mensagem || 'Erro ao salvar disciplina');
    }
  };

  const getTurmaNome = (turmaId: number) => {
    const turma = turmas.find(t => t.id === turmaId);
    return turma?.nome || 'Turma não encontrada';
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

          {success && (
            <Alert severity="success" sx={{ mb: 3 }}>
              {success}
            </Alert>
          )}

          <TableContainer component={Paper}>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell>Nome</TableCell>
                  <TableCell>Código</TableCell>
                  <TableCell>Carga Horária</TableCell>
                  <TableCell>Turma</TableCell>
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
                    <TableCell>{getTurmaNome(disciplina.turmaId)}</TableCell>
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
                {error && (
                  <Alert severity="error" sx={{ mb: 2 }}>
                    {error}
                  </Alert>
                )}
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
                <FormControl fullWidth margin="normal" required>
                  <InputLabel>Turma</InputLabel>
                  <Select
                    value={formData.turmaId}
                    label="Turma"
                    onChange={(e) => setFormData({ ...formData, turmaId: e.target.value })}
                  >
                    {turmas.map((turma) => (
                      <MenuItem key={turma.id} value={turma.id}>
                        {turma.nome} - {turma.serie}
                      </MenuItem>
                    ))}
                  </Select>
                </FormControl>
              </Box>
            </DialogContent>
            <DialogActions>
              <Button onClick={handleCloseDialog}>Cancelar</Button>
              <Button 
                onClick={handleSubmit} 
                variant="contained"
                disabled={!formData.nome.trim() || !formData.codigo.trim() || !formData.cargaHoraria || !formData.turmaId}
              >
                {editingDisciplina ? 'Atualizar' : 'Cadastrar'}
              </Button>
            </DialogActions>
          </Dialog>
        </Box>
      </Layout>
    </ProtectedRoute>
  );
} 