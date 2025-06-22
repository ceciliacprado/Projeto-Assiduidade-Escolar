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
  Paper,
  Chip,
  IconButton,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions
} from '@mui/material';
import {
  School as SchoolIcon,
  Person as PersonIcon,
  Book as BookIcon,
  Assessment as AssessmentIcon,
  Save as SaveIcon,
  CheckCircle as CheckCircleIcon,
  Cancel as CancelIcon,
  Schedule as ScheduleIcon,
  Notifications as NotificationsIcon,
  Email as EmailIcon,
  AccountCircle as AccountCircleIcon
} from '@mui/icons-material';
import { ProtectedRoute } from '../components/ProtectedRoute';
import { Layout } from '../components/Layout';
import { alunoService, professorService, disciplinaService, frequenciaService, turmaService } from '../services/api';
import { Aluno, Disciplina, Frequencia, Turma } from '../types';

interface DashboardStats {
  totalAlunos: number;
  totalProfessores: number;
  totalDisciplinas: number;
  totalFrequencias: number;
}

interface StudentAttendance {
  id: number;
  name: string;
  status: 'present' | 'absent' | 'late';
}

export default function DashboardPage() {
  const [stats, setStats] = useState<DashboardStats | null>(null);
  const [alunos, setAlunos] = useState<Aluno[]>([]);
  const [disciplinas, setDisciplinas] = useState<Disciplina[]>([]);
  const [turmas, setTurmas] = useState<Turma[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [selectedTurma, setSelectedTurma] = useState<number | ''>('');
  const [selectedDisciplina, setSelectedDisciplina] = useState<number | ''>('');
  const [selectedDate, setSelectedDate] = useState(new Date().toISOString().split('T')[0]);
  const [studentAttendance, setStudentAttendance] = useState<StudentAttendance[]>([]);
  const [showSuccessModal, setShowSuccessModal] = useState(false);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const [alunosData, professores, disciplinasData, frequencias, turmasData] = await Promise.all([
          alunoService.listar(),
          professorService.listar(),
          disciplinaService.listar(),
          frequenciaService.listar(),
          turmaService.listar()
        ]);

        setAlunos(alunosData);
        setDisciplinas(disciplinasData);
        setTurmas(turmasData);

        // Inicializar lista de presença
        const attendanceList = alunosData.map(aluno => ({
          id: aluno.id || 0,
          name: aluno.nome,
          status: 'present' as const
        }));
        setStudentAttendance(attendanceList);

        setStats({
          totalAlunos: alunosData.length,
          totalProfessores: professores.length,
          totalDisciplinas: disciplinasData.length,
          totalFrequencias: frequencias.length
        });
      } catch (err: unknown) {
        console.error('Erro ao carregar dashboard:', err);
        setError('Erro ao carregar dados');
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  // Filtrar disciplinas quando uma turma é selecionada
  useEffect(() => {
    if (selectedTurma) {
      const disciplinasDaTurma = disciplinas.filter(d => d.turmaId === selectedTurma);
      setDisciplinas(disciplinasDaTurma);
    }
  }, [selectedTurma]);

  // Filtrar alunos quando uma turma é selecionada
  useEffect(() => {
    if (selectedTurma) {
      const alunosDaTurma = alunos.filter(a => a.turmaId === selectedTurma);
      const attendanceList = alunosDaTurma.map(aluno => ({
        id: aluno.id || 0,
        name: aluno.nome,
        status: 'present' as const
      }));
      setStudentAttendance(attendanceList);
    }
  }, [selectedTurma, alunos]);

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

    try {
      const frequencias = studentAttendance.map(student => ({
        alunoId: student.id,
        disciplinaId: selectedDisciplina as number,
        data: selectedDate,
        presente: student.status === 'present'
      }));

      await frequenciaService.registrarLote(frequencias);
      setShowSuccessModal(true);
    } catch (err) {
      setError('Erro ao salvar frequências');
    }
  };

  const getStatusColor = (status: string) => {
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
                    onChange={(e) => setSelectedTurma(e.target.value as number | '')}
                  >
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
                    {disciplinas.map((disciplina) => (
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
                          color={getStatusColor(student.status) as any}
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
            <Button onClick={() => setShowSuccessModal(false)} variant="contained">
              OK
            </Button>
          </DialogActions>
        </Dialog>
      </Layout>
    </ProtectedRoute>
  );
}