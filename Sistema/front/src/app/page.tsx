'use client';

import React, { useEffect, useState } from 'react';
import {
  Box,
  Card,
  CardContent,
  Typography,
  CircularProgress,
  Alert
} from '@mui/material';
import Grid from '@mui/material/Grid';
import {
  School as SchoolIcon,
  Person as PersonIcon,
  Book as BookIcon,
  Assessment as AssessmentIcon
} from '@mui/icons-material';
import { ProtectedRoute } from '../components/ProtectedRoute';
import { Layout } from '../components/Layout';
import { alunoService, professorService, disciplinaService, frequenciaService } from '../services/api';

interface DashboardStats {
  totalAlunos: number;
  totalProfessores: number;
  totalDisciplinas: number;
  totalFrequencias: number;
}

export default function DashboardPage() {
  const [stats, setStats] = useState<DashboardStats | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    const fetchStats = async () => {
      try {
        const [alunos, professores, disciplinas, frequencias] = await Promise.all([
          alunoService.listar(),
          professorService.listar(),
          disciplinaService.listar(),
          frequenciaService.listar()
        ]);

        setStats({
          totalAlunos: alunos.length,
          totalProfessores: professores.length,
          totalDisciplinas: disciplinas.length,
          totalFrequencias: frequencias.length
        });
      } catch (err: unknown) {
        console.error('Erro ao carregar dashboard:', err);
        setError('Erro ao carregar estatísticas');
      } finally {
        setLoading(false);
      }
    };

    fetchStats();
  }, []);

  const StatCard = ({ title, value, icon, color }: {
    title: string;
    value: number;
    icon: React.ReactNode;
    color: string;
  }) => (
    <Card sx={{ height: '100%' }}>
      <CardContent>
        <Box display="flex" alignItems="center" justifyContent="space-between">
          <Box>
            <Typography color="textSecondary" gutterBottom variant="h6">
              {title}
            </Typography>
            <Typography variant="h4" component="div">
              {value}
            </Typography>
          </Box>
          <Box
            sx={{
              backgroundColor: color,
              borderRadius: '50%',
              p: 1,
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center'
            }}
          >
            {icon}
          </Box>
        </Box>
      </CardContent>
    </Card>
  );

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
          <Typography variant="h4" component="h1" gutterBottom>
            Dashboard
          </Typography>
          <Typography variant="body1" color="text.secondary" sx={{ mb: 4 }}>
            Bem-vindo ao Sistema de Assiduidade Escolar
          </Typography>

          {error && (
            <Alert severity="error" sx={{ mb: 3 }}>
              {error}
            </Alert>
          )}

          {stats && (
            <Grid container spacing={3}>
              {/* eslint-disable-next-line @typescript-eslint/no-explicit-any */}
              <Grid item xs={12} sm={6} md={3} {...({} as any)}>
                <StatCard
                  title="Total de Alunos"
                  value={stats.totalAlunos}
                  icon={<SchoolIcon sx={{ color: 'white' }} />}
                  color="#1976d2"
                />
              </Grid>
              {/* eslint-disable-next-line @typescript-eslint/no-explicit-any */}
              <Grid item xs={12} sm={6} md={3} {...({} as any)}>
                <StatCard
                  title="Total de Professores"
                  value={stats.totalProfessores}
                  icon={<PersonIcon sx={{ color: 'white' }} />}
                  color="#2e7d32"
                />
              </Grid>
              {/* eslint-disable-next-line @typescript-eslint/no-explicit-any */}
              <Grid item xs={12} sm={6} md={3} {...({} as any)}>
                <StatCard
                  title="Total de Disciplinas"
                  value={stats.totalDisciplinas}
                  icon={<BookIcon sx={{ color: 'white' }} />}
                  color="#ed6c02"
                />
              </Grid>
              {/* eslint-disable-next-line @typescript-eslint/no-explicit-any */}
              <Grid item xs={12} sm={6} md={3} {...({} as any)}>
                <StatCard
                  title="Total de Frequências"
                  value={stats.totalFrequencias}
                  icon={<AssessmentIcon sx={{ color: 'white' }} />}
                  color="#9c27b0"
                />
              </Grid>
            </Grid>
          )}

          <Box sx={{ mt: 4 }}>
            <Typography variant="h5" gutterBottom>
              Funcionalidades Disponíveis
            </Typography>
            <Typography variant="body1" color="text.secondary">
              Use o menu lateral para navegar entre as diferentes seções do sistema:
            </Typography>
            <Box component="ul" sx={{ mt: 2, pl: 2 }}>
              <Typography component="li" variant="body1" sx={{ mb: 1 }}>
                <strong>Alunos:</strong> Gerenciar cadastro de alunos e vincular disciplinas
              </Typography>
              <Typography component="li" variant="body1" sx={{ mb: 1 }}>
                <strong>Professores:</strong> Cadastrar e gerenciar professores
              </Typography>
              <Typography component="li" variant="body1" sx={{ mb: 1 }}>
                <strong>Disciplinas:</strong> Criar e gerenciar disciplinas
              </Typography>
              <Typography component="li" variant="body1" sx={{ mb: 1 }}>
                <strong>Frequências:</strong> Registrar e consultar frequências dos alunos
              </Typography>
            </Box>
          </Box>
        </Box>
      </Layout>
    </ProtectedRoute>
  );
}