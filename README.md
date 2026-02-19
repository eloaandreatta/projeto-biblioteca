📚 Biblioteca API

API desenvolvida em ASP.NET Core (.NET) com PostgreSQL, responsável pelo gerenciamento de usuários, livros, empréstimos, reservas e multas.

O projeto aplica:

Programação Orientada a Objetos (POO)

Arquitetura em Camadas

Entity Framework Core

Testes Unitários (NUnit + Moq)

🚀 Funcionalidades
👤 Usuário

Cadastro de usuário

Atualização de cadastro

Consulta de dados

Exclusão de cadastro

Consulta de multas

Pagamento de multa

📖 Livros

Consulta de livros disponíveis

📦 Empréstimos

Solicitação de empréstimo

Registro de devolução

Consulta de empréstimos ativos

Consulta de histórico

Renovação de empréstimo

📌 Reservas

Realizar reserva quando o livro estiver emprestado

Cancelar reserva

Visualizar posição na fila

📏 Regras de Negócio

Não permitir empréstimo se o livro não estiver disponível.

O usuário não pode emprestar mais de 1 livro simultaneamente.

O empréstimo possui duração de 14 dias.

O usuário pode renovar o empréstimo apenas uma vez.

Não é permitido renovar empréstimo em atraso.

Se a devolução ocorrer após o prazo, a multa será gerada automaticamente.

A multa é gerada quando returnDate > dueDate.

Usuário com multa não paga não pode realizar novos empréstimos nem reservas.

Se o livro estiver emprestado, o usuário entra em fila de reserva.

A prioridade da fila é definida pela data de entrada (ordem cronológica).

A reserva possui prazo de 3 dias para retirada; após isso, expira automaticamente.

🏗 Arquitetura

O sistema segue arquitetura em camadas:

Controllers → Endpoints HTTP

Services → Regras de negócio

Repositories → Acesso ao banco

Models → Entidades

DTOs → Transferência de dados

Essa organização garante:

Baixo acoplamento

Alta coesão

Melhor testabilidade

Facilidade de manutenção

🗄 Banco de Dados

PostgreSQL

Entity Framework Core

Principais entidades:

User

Book

Loan

Fine

Reservation

🧪 Testes

Testes unitários implementados para:

Services

Controllers

Ferramentas utilizadas:

NUnit

Moq

Os testes validam regras de negócio, comportamentos e tratamento de erros.

🎯 Objetivo

Projeto desenvolvido com fins acadêmicos e práticos, aplicando:

Boas práticas de arquitetura

Separação de responsabilidades

Implementação de regras de negócio

Integração com banco relacional

Testes automatizados
