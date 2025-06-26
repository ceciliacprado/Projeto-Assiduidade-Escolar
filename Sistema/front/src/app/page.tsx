'use client';

import React, { useEffect, useState } from 'react';
import {
  Box,
  Card,
  CardContent,
  Typography,
  CircularProgress,
  Alert,
  Select,
  MenuItem,
  FormControl,
  InputLabel,
  TextField,
  Button,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Chip,
  IconButton,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions
} from '@mui/material';
import {
  School as SchoolIcon,
  Book as BookIcon,
  Save as SaveIcon,
  CheckCircle as CheckCircleIcon,
  Cancel as CancelIcon
} from '@mui/icons-material';
import { ProtectedRoute } from '../components/ProtectedRoute';
import { Layout } from '../components/Layout';
import { alunoService, disciplinaService, frequenciaService, turmaService } from '../services/api';
import { Disciplina, Turma } from '../types';

interface StudentAttendance {
  id: number;
  name: string;
  status: 'present' | 'absent' | 'late';
}

export default function DashboardPage() {
  const [turmas, setTurmas] = useState<Turma[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [selectedTurma, setSelectedTurma] = useState<number | ''>('');
  const [selectedDisciplina, setSelectedDisciplina] = useState<number | ''>('');
  const [selectedDate, setSelectedDate] = useState(new Date().toISOString().split('T')[0]);
  const [studentAttendance, setStudentAttendance] = useState<StudentAttendance[]>([]);
  const [showSuccessModal, setShowSuccessModal] = useState(false);
  
  // Estados para dados filtrados
  const [disciplinasFiltradas, setDisciplinasFiltradas] = useState<Disciplina[]>([]);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const turmasData = await turmaService.listar();
        setTurmas(turmasData);
      } catch (err: unknown) {
        console.error('Erro ao carregar dashboard:', err);
        setError('Erro ao carregar dados');
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  // Função para lidar com a mudança de turma
  const handleTurmaChange = async (turmaId: number | '') => {
    setSelectedTurma(turmaId);
    setSelectedDisciplina(''); // Reset disciplina quando turma muda
    setLoading(true);
    
    if (turmaId) {
      try {
        console.log('Carregando dados da turma:', turmaId);
        
        // Buscar alunos da turma usando o endpoint correto
        const alunosDaTurma = await alunoService.listarPorTurma(turmaId);
        console.log('Alunos da turma:', alunosDaTurma);
        
        // Buscar disciplinas da turma
        const disciplinasDaTurma = await disciplinaService.listarPorTurma(turmaId);
        console.log('Disciplinas da turma:', disciplinasDaTurma);
        
        // Definir disciplinas da turma
        setDisciplinasFiltradas(disciplinasDaTurma);
        
        // Definir alunos da turma
        setStudentAttendance(alunosDaTurma.map(aluno => ({
          id: aluno.id || 0,
          name: aluno.nome,
          status: 'present' as const
        })));
        
        setError(''); // Limpar erros anteriores
      } catch (err) {
        console.error('Erro ao carregar dados da turma:', err);
        setError('Erro ao carregar dados da turma selecionada');
        setDisciplinasFiltradas([]);
        setStudentAttendance([]);
      }
    } else {
      setDisciplinasFiltradas([]);
      setStudentAttendance([]);
    }
    
    setLoading(false);
  };

  const handleStatusChange = (studentId: number, newStatus: 'present' | 'absent' | 'late') => {
    setStudentAttendance(prev => 
      prev.map(student => 
        student.id === studentId 
          ? { ...student, status: newStatus }
          : student
      )
    );
  };

  const handleSaveAttendance = async () => {
    if (!selectedDisciplina || !selectedTurma) {
      setError('Selecione uma turma e disciplina');
      return;
    }

    if (studentAttendance.length === 0) {
      setError('Não há alunos para registrar frequência');
      return;
    }

    try {
      const frequencias = studentAttendance.map(student => ({
        alunoId: student.id,
        disciplinaId: selectedDisciplina as number,
        data: selectedDate,
        presente: student.status === 'present'
      }));

      await frequenciaService.registrarLote(frequencias);
      setShowSuccessModal(true);
      setError(''); // Limpar erros anteriores
    } catch (err) {
      console.error('Erro ao salvar frequências:', err);
      setError('Erro ao salvar frequências. Verifique se os dados estão corretos.');
    }
  };

  const getStatusColor = (status: string): 'success' | 'error' | 'warning' | 'default' => {
    switch (status) {
      case 'present':
        return 'success';
      case 'absent':
        return 'error';
      case 'late':
        return 'warning';
      default:
        return 'default';
    }
  };

  const getStatusText = (status: string) => {
    switch (status) {
      case 'present':
        return 'Presente';
      case 'absent':
        return 'Falta';
      default:
        return 'Presente';
    }
  };

  const getAttendanceStats = () => {
    const present = studentAttendance.filter(s => s.status === 'present').length;
    const absent = studentAttendance.filter(s => s.status === 'absent').length;
    const late = studentAttendance.filter(s => s.status === 'late').length;
    return { present, absent, late, total: studentAttendance.length };
  };

  const attendanceStats = getAttendanceStats();

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
          {/* Header */}
          <Box sx={{ mb: 4 }}>
            <Typography variant="h4" component="h1" gutterBottom>
              Controle de Assiduidade
            </Typography>
            <Typography variant="body1" color="text.secondary">
              Gerencie a frequência dos alunos de forma eficiente
            </Typography>
          </Box>

          {error && (
            <Alert severity="error" sx={{ mb: 3 }}>
              {error}
            </Alert>
          )}

          {/* Filtros */}
          <Card sx={{ mb: 4 }}>
            <CardContent>
              <Box sx={{ display: 'flex', flexDirection: { xs: 'column', md: 'row' }, gap: 3 }}>
                <FormControl sx={{ minWidth: { xs: '100%', md: 200 } }}>
                  <InputLabel>Turma</InputLabel>
                  <Select
                    value={selectedTurma}
                    label="Turma"
                    onChange={(e) => handleTurmaChange(e.target.value as number | '')}
                  >
                    <MenuItem value="">
                      <em>Selecione uma turma</em>
                    </MenuItem>
                    {turmas.map((turma) => (
                      <MenuItem key={turma.id} value={turma.id}>
                        {turma.nome}
                      </MenuItem>
                    ))}
                  </Select>
                </FormControl>
                <FormControl sx={{ minWidth: { xs: '100%', md: 200 } }}>
                  <InputLabel>Disciplina</InputLabel>
                  <Select
                    value={selectedDisciplina}
                    label="Disciplina"
                    onChange={(e) => setSelectedDisciplina(e.target.value as number | '')}
                    disabled={!selectedTurma}
                  >
                    <MenuItem value="">
                      <em>Selecione uma disciplina</em>
                    </MenuItem>
                    {disciplinasFiltradas.map((disciplina) => (
                      <MenuItem key={disciplina.id} value={disciplina.id}>
                        {disciplina.nome}
                      </MenuItem>
                    ))}
                  </Select>
                </FormControl>
                <TextField
                  type="date"
                  label="Data"
                  value={selectedDate}
                  onChange={(e) => setSelectedDate(e.target.value)}
                  InputLabelProps={{ shrink: true }}
                  sx={{ minWidth: { xs: '100%', md: 200 } }}
                />
                <Button
                  variant="outlined"
                  onClick={() => {
                    handleTurmaChange('');
                    setSelectedDate(new Date().toISOString().split('T')[0]);
                    setError('');
                  }}
                  sx={{ minWidth: { xs: '100%', md: 120 } }}
                >
                  Limpar
                </Button>
              </Box>
            </CardContent>
          </Card>

          {/* Estatísticas */}
          <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 3, mb: 4 }}>
            <Card sx={{ flex: { xs: '1 1 100%', sm: '1 1 calc(50% - 12px)', md: '1 1 calc(25% - 12px)' } }}>
              <CardContent>
                <Box display="flex" alignItems="center">
                  <Box sx={{ p: 1, borderRadius: '50%', bgcolor: 'success.light', mr: 2 }}>
                    <CheckCircleIcon sx={{ color: 'white' }} />
                  </Box>
                  <Box>
                    <Typography color="textSecondary" variant="body2">
                      Presentes
                    </Typography>
                    <Typography variant="h5" fontWeight="bold">
                      {attendanceStats.present}
                    </Typography>
                  </Box>
                </Box>
              </CardContent>
            </Card>
            <Card sx={{ flex: { xs: '1 1 100%', sm: '1 1 calc(50% - 12px)', md: '1 1 calc(25% - 12px)' } }}>
              <CardContent>
                <Box display="flex" alignItems="center">
                  <Box sx={{ p: 1, borderRadius: '50%', bgcolor: 'error.light', mr: 2 }}>
                    <CancelIcon sx={{ color: 'white' }} />
                  </Box>
                  <Box>
                    <Typography color="textSecondary" variant="body2">
                      Faltas
                    </Typography>
                    <Typography variant="h5" fontWeight="bold">
                      {attendanceStats.absent}
                    </Typography>
                  </Box>
                </Box>
              </CardContent>
            </Card>
            <Card sx={{ flex: { xs: '1 1 100%', sm: '1 1 calc(50% - 12px)', md: '1 1 calc(25% - 12px)' } }}>
              <CardContent>
                <Box display="flex" alignItems="center">
                  <Box sx={{ p: 1, borderRadius: '50%', bgcolor: 'primary.light', mr: 2 }}>
                    <SchoolIcon sx={{ color: 'white' }} />
                  </Box>
                  <Box>
                    <Typography color="textSecondary" variant="body2">
                      Total Alunos
                    </Typography>
                    <Typography variant="h5" fontWeight="bold">
                      {attendanceStats.total}
                    </Typography>
                  </Box>
                </Box>
              </CardContent>
            </Card>
          </Box>

          {/* Lista de Chamada */}
          <Card sx={{ mb: 4 }}>
            <Box sx={{ p: 2, borderBottom: 1, borderColor: 'divider', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
              <Typography variant="h6" fontWeight="bold">
                Lista de Chamada
              </Typography>
              <Button
                variant="contained"
                startIcon={<SaveIcon />}
                onClick={handleSaveAttendance}
                disabled={!selectedTurma || !selectedDisciplina}
              >
                Salvar
              </Button>
            </Box>
            {!selectedTurma ? (
              <Box sx={{ p: 4, textAlign: 'center' }}>
                <SchoolIcon sx={{ fontSize: 64, color: 'text.secondary', mb: 2 }} />
                <Typography variant="h6" color="text.secondary" gutterBottom>
                  Selecione uma turma para começar
                </Typography>
                <Typography variant="body2" color="text.secondary">
                  Escolha uma turma no filtro acima para visualizar a lista de alunos e realizar a chamada.
                </Typography>
              </Box>
            ) : !selectedDisciplina ? (
              <Box sx={{ p: 4, textAlign: 'center' }}>
                <BookIcon sx={{ fontSize: 64, color: 'text.secondary', mb: 2 }} />
                <Typography variant="h6" color="text.secondary" gutterBottom>
                  Selecione uma disciplina
                </Typography>
                <Typography variant="body2" color="text.secondary">
                  Escolha uma disciplina para realizar a chamada dos alunos.
                </Typography>
              </Box>
            ) : (
              <TableContainer>
                <Table>
                  <TableHead>
                    <TableRow>
                      <TableCell>N.</TableCell>
                      <TableCell>Nome do Aluno</TableCell>
                      <TableCell>Status</TableCell>
                      <TableCell>Ações</TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    {studentAttendance.map((student, index) => (
                      <TableRow key={student.id}>
                        <TableCell>{index + 1}</TableCell>
                        <TableCell>{student.name}</TableCell>
                        <TableCell>
                          <Chip
                            label={getStatusText(student.status)}
                            color={getStatusColor(student.status)}
                            size="small"
                          />
                        </TableCell>
                        <TableCell>
                          <IconButton
                            size="small"
                            onClick={() => handleStatusChange(student.id, 'present')}
                            sx={{ bgcolor: 'success.light', color: 'white', mr: 1, '&:hover': { bgcolor: 'success.main' } }}
                          >
                            P
                          </IconButton>
                          <IconButton
                            size="small"
                            onClick={() => handleStatusChange(student.id, 'absent')}
                            sx={{ bgcolor: 'error.light', color: 'white', mr: 1, '&:hover': { bgcolor: 'error.main' } }}
                          >
                            F
                          </IconButton>
                        </TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </TableContainer>
            )}
          </Card>

          {/* Resumo */}
          <Box sx={{ display: 'flex', flexDirection: { xs: 'column', md: 'row' }, gap: 3 }}>
            <Card sx={{ flex: 1 }}>
              <CardContent>
                <Typography variant="h6" gutterBottom>
                  Resumo por Disciplina
                </Typography>
                <Box sx={{ height: 200, display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                  <Typography color="textSecondary">
                    Gráfico de frequência por disciplina
                  </Typography>
                </Box>
              </CardContent>
            </Card>
            <Card sx={{ flex: 1 }}>
              <CardContent>
                <Typography variant="h6" gutterBottom>
                  Últimos Registros
                </Typography>
                <Box sx={{ space: 2 }}>
                  <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                    <CheckCircleIcon sx={{ color: 'success.main', mr: 1 }} />
                    <Box>
                      <Typography variant="body2" fontWeight="medium">
                        15/11/2023 - Ciências
                      </Typography>
                      <Typography variant="body2" color="textSecondary">
                        24 presentes, 3 faltas, 2 atrasos
                      </Typography>
                    </Box>
                  </Box>
                  <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                    <CancelIcon sx={{ color: 'error.main', mr: 1 }} />
                    <Box>
                      <Typography variant="body2" fontWeight="medium">
                        14/11/2023 - Matemática
                      </Typography>
                      <Typography variant="body2" color="textSecondary">
                        19 presentes, 7 faltas, 3 atrasos
                      </Typography>
                      <Typography variant="caption" color="error">
                        Alta taxa de ausência
                      </Typography>
                    </Box>
                  </Box>
                  <Box sx={{ display: 'flex', alignItems: 'center' }}>
                    <CheckCircleIcon sx={{ color: 'success.main', mr: 1 }} />
                    <Box>
                      <Typography variant="body2" fontWeight="medium">
                        13/11/2023 - História
                      </Typography>
                      <Typography variant="body2" color="textSecondary">
                        27 presentes, 1 falta, 1 atraso
                      </Typography>
                    </Box>
                  </Box>
                </Box>
              </CardContent>
            </Card>
          </Box>
        </Box>

        {/* Modal de Sucesso */}
        <Dialog open={showSuccessModal} onClose={() => setShowSuccessModal(false)}>
          <DialogTitle>Chamada salva!</DialogTitle>
          <DialogContent>
            <Typography>
              A lista de presença foi armazenada com sucesso.
            </Typography>
          </DialogContent>
          <DialogActions>
            <Button 
              onClick={() => {
                setShowSuccessModal(false);
                // Resetar campos após salvar
                handleTurmaChange('');
                setSelectedDate(new Date().toISOString().split('T')[0]);
              }} 
              variant="contained"
            >
              OK
            </Button>
          </DialogActions>
        </Dialog>
      </Layout>
    </ProtectedRoute>
  );
}