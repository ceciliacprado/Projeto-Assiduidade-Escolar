"use client";

import api from "@/services/api";
import { Usuario } from "@/types/usuario";
import {
  Box,
  Button,
  Container,
  Paper,
  TextField,
  Typography,
} from "@mui/material";
import { useRouter } from "next/navigation";
import React, { useState } from "react";

function Login() {
  const [email, setEmail] = useState("");
  const [senha, setSenha] = useState("");
  const router = useRouter();

  async function efetuarLogin(e: React.FormEvent) {
    e.preventDefault();
    const usuario: Usuario = {
      email,
      senha,
    };
    try {
      const resposta = await api.post("usuario/login", usuario);
      localStorage.setItem("token", resposta.data);
      router.push("/item/listar");
    } catch (erro) {
      console.error(erro);
      // alert("Login ou senha incorretos!");
    }
  }
  return (
    <Container maxWidth="sm">
      <Paper elevation={3} sx={{ padding: 4, mt: 8 }}>
        <Typography variant="h5" component="h1" gutterBottom>
          Login
        </Typography>
        <Box component="form" onSubmit={efetuarLogin}>
          <TextField
            fullWidth
            margin="normal"
            label="E-mail"
            type="text"
            required
            value={email}
            onChange={(e) => setEmail(e.target.value)}
          />

          <TextField
            fullWidth
            margin="normal"
            label="Senha"
            type="text"
            required
            value={senha}
            onChange={(e) => setSenha(e.target.value)}
          />
          <Button
            type="submit"
            variant="contained"
            color="primary"
            fullWidth
            sx={{ mt: 2 }}
          >
            Login
          </Button>
        </Box>
      </Paper>
    </Container>
  );
}

export default Login;