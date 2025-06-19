"use client";

import api from "@/services/api";
import { Item } from "@/types/aluno";
import {
  Container,
  Typography,
  TableContainer,
  Paper,
  Table,
  TableHead,
  TableRow,
  TableCell,
  TableBody,
  TablePagination,
  Link,
} from "@mui/material";
import axios from "axios";
import { useEffect, useState } from "react";

function AlunoListar() {
  const [alunos, setAlunos] = useState<Aluno[]>([]);

  useEffect(() => {
    api
      .get<Aluno[]>("/Aluno/listar")
      .then((resposta) => {
        setItens(resposta.data);
        console.table(resposta.data);
      })
      .catch((erro) => {
        console.log(erro);
      });
  }, []);

  return (
    <Container maxWidth="md" sx={{ mt: 4 }}>
      <Typography variant="h4" component="h1" gutterBottom>
        Listar Produtos
      </Typography>
      <TableContainer component={Paper} elevation={10}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>#</TableCell>
              <TableCell>Nome</TableCell>
              <TableCell>Criado Em</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {alunos.map((aluno) => (
              <TableRow key={aluno.alunoId}>
                <TableCell>{aluno.alunoId}</TableCell>
                <TableCell>{aluno.nome}</TableCell>
                <TableCell>{aluno.criadoEm}</TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>
    </Container>
  );
}

export default AlunoListar;