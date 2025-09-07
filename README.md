# 📖 Guia de Instalação e Execução - Hypesoft Challenge
🎯 Pré-requisitos do Sistema
Requisitos Mínimos

Sistema Operacional: Windows 10/11, macOS 10.15+, ou Linux Ubuntu 18.04+
RAM: 8GB mínimo, 16GB recomendado
Storage: 10GB livres
Conexão: Internet para downloads

Software Necessário

Docker Desktop 4.0+ (Download)
Node.js 18+ (Download)
.NET 9 SDK (Download)
Git (Download)

⚡ Execução Rápida (5 minutos)
Método 1: Docker Compose (Recomendado)
bash# 1. Clone o repositório
git clone https://github.com/seu-usuario/hypesoft-challenge.git
cd hypesoft-challenge

# 2. Configure ambiente
cp .env.example .env

# 3. Execute todos os serviços
docker-compose up -d

# 4. Aguarde todos os serviços iniciarem (2-3 minutos)
docker-compose logs -f

# 5. Acesse a aplicação
# Frontend: http://localhost:3000
# API: http://localhost:5000
# Keycloak: http://localhost:8080
Verificação dos Serviços
bash# Status dos containers
docker-compose ps

# Health checks
curl http://localhost:5000/health    # API
curl http://localhost:3000           # Frontend
curl http://localhost:8080           # Keycloak
🛠️ Execução para Desenvolvimento
1. Preparar Backend
bash# Navegar para o backend
cd backend

# Restaurar dependências
dotnet restore

# Configurar banco (MongoDB via Docker)
docker run -d --name mongodb -p 27017:27017 \
  -e MONGO_INITDB_ROOT_USERNAME=admin \
  -e MONGO_INITDB_ROOT_PASSWORD=admin123 \
  mongo:7.0

# Executar API
cd Hypesoft.API
dotnet run
# API disponível em: http://localhost:5000
2. Preparar Frontend
bash# Navegar para o frontend (novo terminal)
cd frontend

# Instalar dependências
npm install

# Executar em modo desenvolvimento
npm run dev
# Frontend disponível em: http://localhost:3000
3. Configurar Keycloak (Opcional)
bash# Keycloak com Docker
docker run -d --name keycloak -p 8080:8080 \
  -e KEYCLOAK_ADMIN=admin \
  -e KEYCLOAK_ADMIN_PASSWORD=admin123 \
  quay.io/keycloak/keycloak:23.0 start-dev

# Acessar: http://localhost:8080
# Login: admin / admin123
📊 Configuração do Banco de Dados
MongoDB - Dados de Exemplo
O sistema automaticamente popula o banco com dados de exemplo:
bash# Verificar dados via MongoDB Express
# http://localhost:8081
# Login: admin / admin123

# Ou via CLI
docker exec -it hypesoft-mongodb mongosh -u admin -p admin123
use HypesoftDB
db.Products.find().limit(5)
db.Categories.find()
Estrutura dos Dados
Categorias de Exemplo:

Eletrônicos (45 produtos)
Roupas (38 produtos)
Casa & Jardim (28 produtos)
Esportes (22 produtos)
Livros (15 produtos)
Beleza (8 produtos)

Produtos de Exemplo:

iPhone 14 - R$ 4.999,99 (25 unidades)
Samsung Galaxy S23 - R$ 3.899,99 (15 unidades)
Notebook Dell - R$ 2.499,99 (8 unidades - estoque baixo)
E mais 150+ produtos variados

🔐 Sistema de Autenticação
Usuários Pré-configurados
UsuárioSenhaRoleDescriçãoadminadmin123AdministratorAcesso total - CRUD completomanagermanager123ManagerGestão de produtos e categoriasuseruser123UserVisualização e relatórios
Configuração Manual do Keycloak
Se não usar Docker Compose, configure manualmente:

Acessar Admin Console: http://localhost:8080
Login: admin / admin123
Criar Realm: hypesoft
Configurar Clients:

hypesoft-frontend (Public)
hypesoft-api (Confidential)


Criar Roles: admin, manager, user
Criar Usuários conforme tabela acima

🧪 Executando Testes
Backend (.NET)
bashcd backend

# Testes unitários
dotnet test

# Com cobertura
dotnet test --collect:"XPlat Code Coverage"
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coverage" -reporttypes:Html
Frontend (React)
bashcd frontend

# Testes unitários
npm test

# Testes com cobertura
npm run test:coverage

# Testes E2E (se configurado)
npm run test:e2e
🔧 Troubleshooting
Problemas Comuns
1. Porta já em uso
bash# Verificar portas ocupadas
netstat -ano | findstr :3000  # Windows
lsof -i :3000                 # macOS/Linux

# Matar processos
taskkill /PID <PID> /F        # Windows
kill -9 <PID>                # macOS/Linux
2. MongoDB não conecta
bash# Verificar se está rodando
docker ps | grep mongo

# Logs do MongoDB  
docker logs hypesoft-mongodb

# Reiniciar container
docker restart hypesoft-mongodb
3. Keycloak demora para iniciar
bash# Aguardar até 3 minutos na primeira execução
docker-compose logs -f keycloak

# Verificar saúde
curl http://localhost:8080/health
4. Frontend não carrega
bash# Verificar se API está respondendo
curl http://localhost:5000/health

# Limpar cache do Node
cd frontend
rm -rf node_modules package-lock.json
npm install
5. Erro de CORS
bash# Verificar configuração no backend
# appsettings.json > CORS > AllowedOrigins

# Temporariamente desabilitar no browser
chrome --disable-web-security --user-data-dir="C:/ChromeDevSession"
Logs Detalhados
bash# Todos os serviços
docker-compose logs -f

# Serviço específico
docker-compose logs -f backend
docker-compose logs -f keycloak

# Últimas 100 linhas
docker-compose logs --tail=100 frontend
🚀 Deployment
Preparação para Produção
bash# 1. Build de produção
docker-compose -f docker-compose.yml build

# 2. Configurar variáveis de ambiente
cp .env.example .env.production
# Editar .env.production com valores seguros

# 3. Deploy
docker-compose --env-file .env.production up -d
Verificação de Saúde
bash# Health checks automatizados
curl http://localhost/health      # Nginx
curl http://localhost:5000/health # API
curl http://localhost:8080/health # Keycloak

# Métricas
curl http://localhost:5000/metrics
📈 Monitoramento
Logs Centralizados
bash# Structured logs com Serilog
docker-compose logs -f backend | grep ERROR
docker-compose logs -f backend | grep WARNING
Performance
bash# Métricas de containers
docker stats

# Uso de recursos
docker-compose top
⚙️ Configurações Avançadas
Variáveis de Ambiente
bash# .env personalizado
MONGO_ROOT_PASSWORD=sua_senha_segura
KEYCLOAK_ADMIN_PASSWORD=sua_senha_segura
ASPNET_ENVIRONMENT=Production
FRONTEND_TARGET=production
Customização
bash# Themes do Keycloak
./docker/keycloak/themes/

# Configurações do Nginx
./docker/nginx/nginx.conf

# Scripts de inicialização
./docker/scripts/
📋 Checklist de Instalação
Pré-Instalação

 Docker Desktop instalado e rodando
 Node.js 18+ instalado
 .NET 9 SDK instalado
 Git configurado
 Portas livres: 3000, 5000, 8080, 27017

Instalação

 Repositório clonado
 Arquivo .env configurado
 Docker Compose executado
 Todos os containers rodando (docker-compose ps)
 Health checks passando

Validação

 Frontend carrega (http://localhost:3000)
 API responde (http://localhost:5000/health)
 Swagger acessível (http://localhost:5000/swagger)
 Keycloak funcional (http://localhost:8080)
 Login com usuários de teste funcionando
 Dashboard exibe dados corretamente

Desenvolvimento

 Hot reload funcionando (frontend)
 Logs aparecendo corretamente
 Testes passando
 Code coverage > 80%

🔗 Links Úteis

Documentação .NET 9: https://docs.microsoft.com/dotnet/
React Documentation: https://react.dev/
Docker Compose: https://docs.docker.com/compose/
Keycloak Admin: https://www.keycloak.org/documentation
MongoDB Manual: https://docs.mongodb.com/
TailwindCSS: https://tailwindcss.com/docs


# Desafio Técnico Hypesoft - Sistema de Gestão de Produtos

## Visão Geral

Bem-vindo ao desafio técnico da Hypesoft! Este projeto consiste no desenvolvimento de um sistema completo de gestão de produtos, demonstrando suas habilidades em arquitetura moderna, boas práticas de desenvolvimento e tecnologias de ponta.

## Referência Visual

O design da aplicação deve seguir o padrão visual moderno demonstrado neste protótipo:

**ShopSense Dashboard - Product Page**: https://dribbble.com/shots/24508262-ShopSense-Dashboard-Product-Page

## Requisitos do Sistema

### Funcionalidades Principais

#### Gestão de Produtos
- Criar, listar, editar e excluir produtos
- Cada produto deve conter: nome, descrição, preço, categoria, quantidade em estoque
- Validação básica de dados obrigatórios
- Busca simples por nome do produto

#### Sistema de Categorias
- Criar e gerenciar categorias de produtos (lista simples)
- Associar produtos a uma categoria
- Filtrar produtos por categoria

#### Controle de Estoque
- Controlar quantidade em estoque de cada produto
- Atualização manual de estoque
- Exibir produtos com estoque baixo (menor que 10 unidades)

#### Dashboard Simples
- Total de produtos cadastrados
- Valor total do estoque
- Lista de produtos com estoque baixo
- Gráfico básico de produtos por categoria

#### Sistema de Autenticação
- Integração com **Keycloak** para autenticação
- Login via Keycloak (OAuth2/OpenID Connect)
- Proteção de rotas no frontend
- Autorização baseada em roles do Keycloak
- Logout integrado com Keycloak

### Requisitos Técnicos

#### Performance
- Resposta da API em menos de 500ms para consultas simples
- Paginação eficiente para grandes volumes
- Cache para consultas frequentes
- Otimização de queries no banco

#### Escalabilidade
- Arquitetura preparada para crescimento horizontal
- Separação clara entre camadas
- Padrões que facilitem manutenção e evolução
- Código limpo e bem estruturado

#### Segurança
- Rate limiting para prevenir abuso
- Validação e sanitização de entradas
- Headers de segurança adequados
- Tratamento seguro de dados sensíveis

#### Disponibilidade
- Health checks implementados
- Tratamento adequado de erros
- Mensagens de erro claras e úteis
- Logs estruturados para monitoramento

#### Usabilidade
- Interface responsiva (desktop e mobile)
- Validação em tempo real nos formulários
- Feedback visual para ações do usuário
- Experiência intuitiva e consistente

## Stack Tecnológica

### Frontend
- **React 18** com TypeScript
- **Vite** ou **Next.js 14** (App Router)
- **TailwindCSS** + **Shadcn/ui** para estilização
- **React Query/TanStack Query** para gerenciamento de estado
- **React Hook Form** + **Zod** para validação
- **Recharts** ou **Chart.js** para dashboards
- **React Testing Library** + **Vitest** para testes

### Backend
- **.NET 9** com C#
- **Clean Architecture** + **DDD** (Domain-Driven Design)
- **CQRS** + **MediatR** pattern
- **Entity Framework Core** com MongoDB provider
- **FluentValidation** para validação
- **AutoMapper** para mapeamento
- **Serilog** para logging estruturado
- **xUnit** + **FluentAssertions** para testes

### Infraestrutura
- **MongoDB** como banco principal
- **Keycloak** para autenticação e autorização
- **Docker** + **Docker Compose** para containerização
- **Nginx** como reverse proxy

## Arquitetura do Sistema

### Backend - Clean Architecture + DDD

```
src/
├── Hypesoft.Domain/              # Camada de Domínio
│   ├── Entities/                 # Entidades do domínio
│   ├── ValueObjects/             # Objetos de valor
│   ├── DomainEvents/            # Eventos de domínio
│   ├── Repositories/            # Interfaces dos repositórios
│   └── Services/                # Serviços de domínio
├── Hypesoft.Application/         # Camada de Aplicação
│   ├── Commands/                # Comandos CQRS
│   ├── Queries/                 # Consultas CQRS
│   ├── Handlers/                # Handlers MediatR
│   ├── DTOs/                    # Data Transfer Objects
│   ├── Validators/              # Validadores FluentValidation
│   └── Interfaces/              # Interfaces da aplicação
├── Hypesoft.Infrastructure/      # Camada de Infraestrutura
│   ├── Data/                    # Contexto e configurações EF
│   ├── Repositories/            # Implementação dos repositórios
│   ├── Services/                # Serviços externos
│   └── Configurations/          # Configurações de DI
└── Hypesoft.API/                # Camada de Apresentação
    ├── Controllers/             # Controllers da API
    ├── Middlewares/             # Middlewares customizados
    ├── Filters/                 # Filtros de ação
    └── Extensions/              # Extensões de configuração
```

### Frontend - Arquitetura Modular

```
src/
├── components/                   # Componentes reutilizáveis
│   ├── ui/                      # Componentes base (shadcn/ui)
│   ├── forms/                   # Componentes de formulário
│   ├── charts/                  # Componentes de gráficos
│   └── layout/                  # Componentes de layout
├── pages/                       # Páginas da aplicação
├── hooks/                       # Custom hooks
├── services/                    # Serviços de API
├── stores/                      # Stores de estado global
├── types/                       # Definições de tipos
├── utils/                       # Funções utilitárias
└── lib/                         # Configurações de bibliotecas
```

## Diferenciais

#### Testes Abrangentes
- Cobertura mínima de 85% no backend
- Testes E2E com Playwright ou Cypress
- Testes de integração para todos os endpoints
- Testes unitários para regras de negócio
- Testes de mutação para validar qualidade

#### Observabilidade Completa
- Logs estruturados com correlationId
- Métricas customizadas para monitoramento
- Health checks detalhados para todos os serviços
- Tratamento adequado de erros com contexto
- Monitoring de performance da aplicação

#### Performance e Otimização
- Server-side rendering (Next.js)
- Code splitting e lazy loading
- Estratégias de caching (Redis + HTTP cache)
- Indexação otimizada do banco de dados
- Otimização de imagens e assets
- Compressão de responses

#### Segurança Avançada
- Integração completa com Keycloak
- Proteção de rotas baseada em roles
- Token JWT validado adequadamente
- CORS configurado adequadamente
- Headers de segurança implementados
- Validação em múltiplas camadas

#### Qualidade de Código
- Princípios SOLID aplicados consistentemente
- Clean Code em todas as camadas
- Padrões de design bem implementados
- Documentação inline adequada
- Tratamento de exceções robusto

#### Documentação Excepcional
- OpenAPI/Swagger com exemplos detalhados
- Documentação de arquitetura (C4 Model)
- ADRs (Architecture Decision Records)
- Guias de instalação e execução completos
- Collection do Postman atualizada

### Pontos Extras (Opcionais)

- **Roles avançadas no Keycloak** (Admin, Manager, User)
- **GraphQL** como alternativa à REST API
- **Real-time updates** via SignalR/WebSockets
- **Exportação de relatórios** em PDF
- **Internacionalização** (i18n) básica
- **PWA** com capacidades offline
- **Docker multi-stage builds** otimizados

## Como Executar

### Pré-requisitos
- Docker Desktop 4.0+
- Node.js 18+
- .NET 9 SDK
- Git

### Instalação e Execução

```bash
# Clone o repositório
git clone https://github.com/seu-usuario/hypesoft-challenge.git
cd hypesoft-challenge

# Copie as variáveis de ambiente
cp .env.example .env

# Execute toda a aplicação com Docker Compose
docker-compose up -d

# Aguarde alguns segundos para os serviços iniciarem
# Verifique se todos os containers estão rodando
docker-compose ps
```

### URLs de Acesso
- **Frontend**: http://localhost:3000
- **API**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger
- **MongoDB Express**: http://localhost:8081
- **Keycloak**: http://localhost:8080

### Desenvolvimento Local

```bash
# Para desenvolvimento do frontend
cd frontend
npm install
npm run dev

# Para desenvolvimento do backend
cd backend
dotnet restore
dotnet run

# Para executar testes
dotnet test
cd ../frontend
npm test
```

## Padrões de Commit

Este projeto utiliza [Conventional Commits](https://conventionalcommits.org/):

```bash
# Exemplos de commits
feat(products): add bulk import functionality
fix(api): resolve pagination issue in products endpoint
docs(readme): update installation instructions
test(products): add unit tests for product service
refactor(auth): improve JWT token validation
perf(database): optimize product search queries
style(frontend): apply consistent spacing in components
chore(deps): update dependencies to latest versions
```

### Tipos de Commit
- `feat`: Nova funcionalidade
- `fix`: Correção de bug
- `docs`: Documentação
- `style`: Formatação, ponto e vírgula, etc
- `refactor`: Refatoração de código
- `test`: Adição ou correção de testes
- `chore`: Tarefas de manutenção
- `perf`: Melhorias de performance
- `build`: Build e dependências

## Critérios de Avaliação

### Técnico (60%)
- **Arquitetura**: Clean Architecture, DDD, CQRS implementados corretamente
- **Qualidade de Código**: SOLID, Clean Code, padrões consistentes
- **Testes**: Cobertura, qualidade dos testes, cenários bem cobertos
- **Performance**: Otimizações, caching, queries eficientes
- **Segurança**: Implementação adequada de autenticação/autorização

### Funcional (25%)
- **Completude**: Todas as funcionalidades implementadas
- **UX/UI**: Interface intuitiva e responsiva
- **Validações**: Tratamento adequado de erros
- **Regras de Negócio**: Implementação correta dos requisitos

### Profissional (15%)
- **Documentação**: README completo, código bem documentado
- **Git Flow**: Commits organizados, branches bem estruturadas
- **Docker**: Compose funcionando perfeitamente
- **Extras**: Funcionalidades que demonstram expertise avançada

## Entregáveis

### Código Fonte
- Repositório GitHub público
- README detalhado (este arquivo)
- Docker Compose funcional
- Testes automatizados com boa cobertura

### Aplicação Funcionando
- Todos os serviços rodando via Docker Compose
- Banco de dados populado com dados de exemplo
- Interface funcional e responsiva

### Documentação
- API documentada com Swagger
- Guia de instalação e execução
- Documentação das decisões arquiteturais

### Apresentação
- Vídeo de 5-10 minutos demonstrando a aplicação
- Explicação das decisões técnicas tomadas
- Showcase das funcionalidades implementadas
- Demonstração dos diferenciais implementados


---

**Boa sorte e mostre do que você é capaz!**

---

*Este desafio foi criado para identificar desenvolvedores excepcionais que compartilham nossa paixão por tecnologia e excelência técnica. Estamos ansiosos para ver sua solução!*
