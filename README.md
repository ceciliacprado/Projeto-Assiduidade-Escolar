# 🏫 Sistema de Assiduidade Escolar

Sistema completo para controle de frequência escolar com API .NET Core e frontend React/Next.js.

## 📋 Pré-requisitos

- **.NET 8.0 SDK** - [Download aqui](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Node.js 18+** - [Download aqui](https://nodejs.org/)

## 🚀 Como Iniciar

### 1. Clone o Repositório
```bash
git clone [URL_DO_REPOSITORIO]
cd Projeto-Assiduidade-Escolar
```

### 2. Configurar e Executar a API

```bash
# Navegar para a pasta da API
cd Sistema/API

# Restaurar dependências
dotnet restore

# Executar migrações do banco de dados
dotnet ef database update

# Executar a API
dotnet run
```

A API estará disponível em: `http://localhost:5268`

### 3. Configurar e Executar o Frontend

```bash
# Abrir novo terminal e navegar para a pasta do frontend
cd Sistema/front

# Instalar dependências
npm i

# Executar o frontend
npm run dev
```

O frontend estará disponível em: `http://localhost:3000`

## 🔐 Login Inicial

Use as credenciais padrão:
- **Email:** `admin@escola.com`
- **Senha:** `123456`

## 📁 Estrutura do Projeto

```
Projeto-Assiduidade-Escolar/
├── Sistema/
│   ├── API/                    # Backend .NET Core
│   │   ├── Controllers/        # Controladores da API
│   │   ├── Models/            # Modelos de dados
│   │   ├── Services/          # Serviços de negócio
│   │   ├── Data/              # Contexto do banco de dados
│   │   └── Tests/             # Arquivos de teste .http
│   └── front/                 # Frontend React/Next.js
│       ├── src/
│       │   ├── app/           # Páginas da aplicação
│       │   ├── components/    # Componentes reutilizáveis
│       │   ├── services/      # Serviços de API
│       │   └── types/         # Tipos TypeScript
│       └── package.json
└── README.md
```

## 🎯 Funcionalidades Principais

### 👨‍🏫 **Gestão de Professores**
- Cadastro e login de professores
- Autenticação JWT
- Controle de acesso por roles

### 👥 **Gestão de Alunos**
- Cadastro de alunos
- Vinculação automática a disciplinas
- Listagem por turma

### 📚 **Gestão de Disciplinas**
- Cadastro de disciplinas
- Vinculação automática de alunos
- Associação a turmas e professores

### 🏫 **Gestão de Turmas**
- Cadastro de turmas
- Visualização de alunos e disciplinas
- Organização por ano e série

### ✅ **Controle de Frequência**
- Registro de presença/ausência
- Registro em lote
- Dashboard com estatísticas
- Filtros por turma, disciplina e data

## 📊 Dashboard

O dashboard principal oferece:
- **Estatísticas gerais** (alunos, professores, disciplinas, frequências)
- **Filtros** por turma e disciplina
- **Lista de chamada** interativa
- **Registro de frequência** em lote
- **Visualização de dados** em tempo real

## 🧪 Testes

### Testes da API
Os arquivos de teste estão em `Sistema/API/Tests/`:
- `aluno.http` - Testes de alunos
- `disciplina.http` - Testes de disciplinas
- `frequencia.http` - Testes de frequências
- `professor.http` - Testes de professores
- `turma.http` - Testes de turmas
- `dashboard.http` - Testes de integração

### Como Executar os Testes
1. Instale a extensão **REST Client** no VS Code
2. Abra qualquer arquivo `.http`
3. Clique em "Send Request" para executar os testes

## 🔧 Configurações

### Banco de Dados
O sistema usa MySql por padrão
