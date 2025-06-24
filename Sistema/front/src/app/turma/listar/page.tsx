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
import { turmaService } from '../../../services/api';
import { Turma } from '../../../types';

export default function TurmasPage() {
  const [turmas, setTurmas] = useState<Turma[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [openDialog, setOpenDialog] = useState(false);
  const [editingTurma, setEditingTurma] = useState<Turma | null>(null);
  const [formData, setFormData] = useState({
    nome: '',
    ano: '',
    serie: ''
  });

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = async () => {
    try {
      const data = await turmaService.listar();
      setTurmas(data);
    } catch (err: unknown) {
      console.error('Erro ao carregar turmas:', err);
      setError('Erro ao carregar turmas');
    } finally {
      setLoading(false);
    }
  };

  const handleOpenDialog = (turma?: Turma) => {
    if (turma) {
      setEditingTurma(turma);
      setFormData({
        nome: turma.nome,
        ano: turma.ano,
        serie: turma.serie
      });
    } else {
      setEditingTurma(null);
      setFormData({
        nome: '',
        ano: '',
        serie: ''
      });
    }
    setOpenDialog(true);
  };

  const handleCloseDialog = () => {
    setOpenDialog(false);
    setEditingTurma(null);
  };

  const handleSubmit = async () => {
    try {
      const turmaData = {
        nome: formData.nome,
        ano: formData.ano,
        serie: formData.serie
      };

      if (editingTurma) {
        await turmaService.atualizar(editingTurma.id!, turmaData);
      } else {
        await turmaService.cadastrar(turmaData);
      }

      fetchData();
      handleCloseDialog();
    } catch (err: unknown) {
      const error = err as { response?: { data?: { mensagem?: string } } };
      setError(error.response?.data?.mensagem || 'Erro ao salvar turma');
    }
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
              Turmas
            </Typography>
            <Button
              variant="contained"
              startIcon={<AddIcon />}
              onClick={() => handleOpenDialog()}
            >
              Nova Turma
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
                  <TableCell>Ano</TableCell>
                  <TableCell>Série</TableCell>
                  <TableCell>Data de Cadastro</TableCell>
                  <TableCell>Ações</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {turmas.map((turma) => (
                  <TableRow key={turma.id}>
                    <TableCell>{turma.nome}</TableCell>
                    <TableCell>{turma.ano}</TableCell>
                    <TableCell>{turma.serie}</TableCell>
                    <TableCell>
                      {turma.criadoEm ? new Date(turma.criadoEm).toLocaleDateString('pt-BR') : '-'}
                    </TableCell>
                    <TableCell>
                      <IconButton
                        size="small"
                        onClick={() => handleOpenDialog(turma)}
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
              {editingTurma ? 'Editar Turma' : 'Nova Turma'}
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
                  label="Ano"
                  value={formData.ano}
                  onChange={(e) => setFormData({ ...formData, ano: e.target.value })}
                  margin="normal"
                  required
                />
                <TextField
                  fullWidth
                  label="Série"
                  value={formData.serie}
                  onChange={(e) => setFormData({ ...formData, serie: e.target.value })}
                  margin="normal"
                  required
                />
              </Box>
            </DialogContent>
            <DialogActions>
              <Button onClick={handleCloseDialog}>Cancelar</Button>
              <Button onClick={handleSubmit} variant="contained">
                {editingTurma ? 'Atualizar' : 'Cadastrar'}
              </Button>
            </DialogActions>
          </Dialog>
        </Box>
      </Layout>
    </ProtectedRoute>
  );
} 