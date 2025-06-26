# Arquivos de Teste da API

Este diretório contém arquivos de teste `.http` para todas as funcionalidades da API do sistema de assiduidade escolar.

## 📁 Arquivos Disponíveis

### 1. `aluno.http` - Testes da API de Alunos
- ✅ Listar todos os alunos
- ✅ Buscar aluno por ID
- ✅ Cadastrar novo aluno
- ✅ Atualizar aluno
- ✅ Excluir aluno
- ✅ Listar alunos por turma
- ✅ Listar alunos por nome da turma
- ✅ Testes de validação e cenários de erro
- ✅ Testes de mudança de turma
- ✅ Testes de vinculação automática a disciplinas

### 2. `disciplina.http` - Testes da API de Disciplinas
- ✅ Listar todas as disciplinas
- ✅ Buscar disciplina por ID
- ✅ Cadastrar nova disciplina (vincula alunos automaticamente)
- ✅ Atualizar disciplina
- ✅ Excluir disciplina
- ✅ Listar disciplinas por turma
- ✅ Listar disciplinas por professor
- ✅ Vincular alunos existentes a disciplina
- ✅ Testes de validação e cenários de erro
- ✅ Testes de mudança de turma

### 3. `frequencia.http` - Testes da API de Frequências
- ✅ Listar todas as frequências
- ✅ Buscar frequência por ID
- ✅ Registrar frequência individual
- ✅ Atualizar frequência
- ✅ Excluir frequência
- ✅ Listar frequências por aluno
- ✅ Listar frequências por disciplina
- ✅ Listar frequências por data
- ✅ Registrar frequência em lote
- ✅ Testes de validação e cenários de erro

### 4. `professor.http` - Testes da API de Professores
- ✅ Login de professores
- ✅ Listar todos os professores
- ✅ Cadastrar novo professor
- ✅ Testes de autenticação
- ✅ Testes de validação e cenários de erro
- ✅ Testes de formato de email e senha

### 5. `turma.http` - Testes da API de Turmas
- ✅ Listar todas as turmas
- ✅ Buscar turma por ID
- ✅ Cadastrar nova turma
- ✅ Atualizar turma
- ✅ Excluir turma
- ✅ Listar alunos de uma turma
- ✅ Listar disciplinas de uma turma
- ✅ Testes de validação e cenários de erro

### 6. `dashboard.http` - Testes de Funcionalidades Específicas
- ✅ Teste de conexão com banco de dados
- ✅ Fluxo completo: Turma → Alunos → Disciplinas → Frequências
- ✅ Testes de mudança de turma
- ✅ Testes de exclusão em cascata
- ✅ Testes de validações de negócio
- ✅ Testes de performance
- ✅ Testes de autenticação e autorização
- ✅ Testes de relacionamentos entre entidades

## 🚀 Como Usar

### Pré-requisitos
1. **VS Code** com a extensão **REST Client** instalada
2. **API rodando** em `http://localhost:5268`
3. **Token de autenticação** válido (obtenha fazendo login)

### Configuração do Token
1. Faça login usando o endpoint de professor:
   ```http
   POST http://localhost:5268/api/professor/login
   Content-Type: application/json

   {
       "email": "admin@escola.com",
       "senha": "123456"
   }
   ```

2. Copie o token retornado e atualize a variável `@authToken` em todos os arquivos de teste

### Executando os Testes
1. Abra qualquer arquivo `.http` no VS Code
2. Clique em **"Send Request"** acima de cada requisição
3. Ou use **Ctrl+Alt+R** para executar todas as requisições do arquivo

## 🔧 Configurações

### Variáveis de Ambiente
Todos os arquivos usam as seguintes variáveis:
- `@baseUrl = http://localhost:5268` - URL base da API
- `@authToken = [seu-token-aqui]` - Token de autenticação

### Ordem Recomendada de Execução
1. `professor.http` - Para obter token de autenticação
2. `turma.http` - Criar turmas base
3. `aluno.http` - Criar alunos
4. `disciplina.http` - Criar disciplinas
5. `frequencia.http` - Registrar frequências
6. `dashboard.http` - Testes de integração

## 📋 Cenários de Teste

### Cenários Positivos
- ✅ Cadastro, atualização e exclusão de entidades
- ✅ Listagem e busca por ID
- ✅ Relacionamentos entre entidades
- ✅ Vinculação automática de alunos a disciplinas
- ✅ Registro de frequência em lote

### Cenários Negativos
- ✅ Validação de campos obrigatórios
- ✅ Validação de dados inválidos
- ✅ Teste de entidades inexistentes
- ✅ Teste de relacionamentos inválidos
- ✅ Teste de autenticação sem token
- ✅ Teste de autorização insuficiente

### Cenários de Integração
- ✅ Fluxo completo de criação de dados
- ✅ Mudança de turma com atualização de frequências
- ✅ Exclusão em cascata
- ✅ Validações de negócio

## 🐛 Solução de Problemas

### Erro 401 - Unauthorized
- Verifique se o token está válido
- Faça login novamente para obter um novo token

### Erro 404 - Not Found
- Verifique se a API está rodando
- Verifique se os IDs usados nos testes existem

### Erro 500 - Internal Server Error
- Verifique os logs da API
- Verifique se o banco de dados está acessível

### Token Expirado
- Faça login novamente para obter um novo token
- Atualize a variável `@authToken` em todos os arquivos

## 📝 Notas Importantes

1. **Dados de Teste**: Os testes criam dados reais no banco. Use um banco de teste se possível.

2. **Ordem de Execução**: Alguns testes dependem de outros. Execute na ordem recomendada.

3. **Limpeza**: Use os testes de limpeza no final para remover dados de teste.

4. **IDs Dinâmicos**: Alguns testes usam IDs fixos. Ajuste conforme necessário.

5. **Validações**: Os testes incluem validações de negócio implementadas na API.

## 🔄 Atualizações

- **v1.0**: Criação inicial dos arquivos de teste
- **v1.1**: Adição de testes de validação e cenários de erro
- **v1.2**: Adição de testes de integração e dashboard
- **v1.3**: Melhoria na organização e documentação 