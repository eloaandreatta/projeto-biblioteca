📚 Biblioteca API

API Web desenvolvida em ASP.NET Core (.NET) com banco de dados PostgreSQL, responsável pelo gerenciamento de usuários, livros, empréstimos, reservas e multas.

O projeto aplica conceitos de:

Programação Orientada a Objetos (POO)

Arquitetura em Camadas

Entity Framework Core

Testes Unitários com NUnit e Moq

🚀 Funcionalidades
👤 Usuário

Cadastro de usuário

Atualização de dados cadastrais

Consulta de dados do usuário

Exclusão de cadastro

Consulta de multas

Pagamento de multa

📖 Livros

Consulta de livros disponíveis para empréstimo

📦 Empréstimos

Solicitação de empréstimo

Registro de devolução

Consulta de empréstimos ativos

Consulta de histórico de empréstimos

Renovação de empréstimo

📌 Reservas

Realização de reserva quando o livro estiver emprestado

Cancelamento de reserva

Visualização da posição na fila de espera

📏 Regras de Negócio

O sistema implementa as seguintes regras:

Não permitir empréstimo se o livro não estiver disponível.

O usuário não pode emprestar mais de 1 livro simultaneamente.

O empréstimo possui duração de 14 dias.

O usuário pode renovar o empréstimo apenas uma vez.

Não é permitido renovar o empréstimo se estiver em atraso.

Se a devolução ocorrer após o prazo, a multa será gerada automaticamente.

A multa é gerada quando returnDate > dueDate.

Usuário com multa não paga não pode realizar novos empréstimos nem reservas.

Se o livro estiver emprestado, o usuário entra em uma fila de reserva.

A prioridade da fila é definida pela data de entrada (ordem cronológica).

A reserva possui prazo de 3 dias para retirada; após isso, expira automaticamente.

🏗 Arquitetura

O sistema segue arquitetura em camadas:

Controllers → Responsáveis pelos endpoints HTTP

Services → Contêm as regras de negócio

Repositories → Responsáveis pelo acesso ao banco de dados

Models → Representação das entidades

DTOs → Transporte de dados entre camadas

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

O projeto possui testes unitários para:

Services

Controllers

Tecnologias utilizadas:

NUnit

Moq

Os testes validam:

Regras de negócio

Comportamento dos endpoints

Tratamento de erros

🎯 Objetivo do Projeto

Este projeto foi desenvolvido com fins acadêmicos e práticos, com o objetivo de aplicar:

Conceitos de arquitetura limpa

Boas práticas de desenvolvimento

Validação de regras de negócio

Integração com banco relacional

Testes automatizados
