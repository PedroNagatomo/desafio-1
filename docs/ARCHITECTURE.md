# 🏛️ Documentação de Arquitetura - Hypesoft Challenge

## 📐 Visão Geral da Arquitetura

O sistema foi projetado seguindo os princípios de **Clean Architecture**, **Domain-Driven Design (DDD)** e **CQRS**, garantindo alta manutenibilidade, testabilidade e escalabilidade.

## 🎯 Decisões Arquiteturais

### ADR-001: Clean Architecture + DDD
**Status**: Aprovada  
**Data**: Janeiro 2024  
**Contexto**: Necessidade de uma arquitetura que permita fácil manutenção e evolução  
**Decisão**: Implementar Clean Architecture com DDD para separar claramente as responsabilidades  
**Consequências**: 
- ✅ Código mais testável e manutenível
- ✅ Regras de negócio isoladas
- ❌ Maior complexidade inicial

### ADR-002: MongoDB como Banco Principal
**Status**: Aprovada  
**Data**: Janeiro 2024  
**Contexto**: Flexibilidade no schema e performance para leitura  
**Decisão**: MongoDB para armazenamento principal dos dados  
**Consequências**:
- ✅ Schema flexível para evolução
- ✅ Performance excelente para leitura
- ✅ Suporte nativo a JSON
- ❌ Menor consistência ACID comparado a SQL

### ADR-003: React + TypeScript Frontend
**Status**: Aprovada  
**Data**: Janeiro 2024  
**Contexto**: Interface moderna e tipagem forte  
**Decisão**: React 18 com TypeScript e TailwindCSS  
**Consequências**:
- ✅ Desenvolvimento mais produtivo
- ✅ Fewer bugs com TypeScript
- ✅ Ecossistema rico
- ❌ Bundle size maior

### ADR-004: Keycloak para Autenticação
**Status**: Aprovada  
**Data**: Janeiro 2024  
**Contexto**: Necessidade de autenticação enterprise-grade  
**Decisão**: Keycloak com OAuth2/OpenID Connect  
**Consequências**:
- ✅ Padrões de segurança modernos
- ✅ SSO e federação de identidade
- ✅ Gestão centralizada de usuários
- ❌ Complexidade adicional de deployment

## 🏗️ Arquitetura de Alto Nível

```
┌─────────────────────────────────────────────────────────────┐
│                    PRESENTATION LAYER                        │
├─────────────────────────────────────────────────────────────┤
│  React Frontend     │  .NET API        │  Nginx Proxy       │
│  - Components       │  - Controllers   │  - Load Balancer   │
│  - Pages            │  - Middlewares   │  - SSL Termination │
│  - Services         │  - Validation    │  - Caching         │
└─────────────────────────────────────────────────────────────┘
                                │
┌─────────────────────────────────────────────────────────────┐
│                   APPLICATION LAYER                         │
├─────────────────────────────────────────────────────────────┤
│  Commands & Queries │  Handlers        │  DTOs              │
│  - CQRS Pattern     │  - MediatR       │  - AutoMapper      │
│  - Validation       │  - Use Cases     │  - Serialization   │
└─────────────────────────────────────────────────────────────┘
                                │
┌─────────────────────────────────────────────────────────────┐
│                    DOMAIN LAYER                             │
├─────────────────────────────────────────────────────────────┤
│  Entities           │  Value Objects   │  Domain Services   │
│  - Product          │  - Money         │  - Business Rules  │
│  - Category         │  - StockQuantity │  - Domain Events   │
│  - Aggregates       │  - Validation    │  - Policies        │
└─────────────────────────────────────────────────────────────┘
                                │
┌─────────────────────────────────────────────────────────────┐
│                 INFRASTRUCTURE LAYER                        │
├─────────────────────────────────────────────────────────────┤
│  Data Access        │  External APIs   │  Cross-Cutting     │
│  - MongoDB          │  - Keycloak      │  - Logging         │
│  - Repositories     │  - HTTP Clients  │  - Caching         │
│  - Migrations       │  - Third-party   │  - Monitoring      │
└─────────────────────────────────────────────────────────────┘
```

## 🎭 Padrões Implementados

### 1. Repository Pattern
```csharp
public interface IProductRepository : IBaseRepository<Product>
{
    Task<Product?> GetByIdAsync(string id);
    Task<PagedResult<Product>> GetPagedAsync(int page, int pageSize);
    // Operações específicas do domínio
}
```

### 2. CQRS (Command Query Responsibility Segregation)
```csharp
// Command para escrita
public class CreateProductCommand : IRequest<ProductDto>
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    // Propriedades de comando
}

// Query para leitura
public class GetProductsQuery : IRequest<PagedResult<ProductDto>>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    // Parâmetros de consulta
}
```

### 3. Domain-Driven Design
```csharp
public class Product : BaseEntity
{
    public Money Price { get; private set; }
    public StockQuantity Stock { get; private set; }
    
    // Métodos que expressam o negócio
    public void UpdateStock(int quantity) { /* regras */ }
    public bool IsLowStock(int threshold = 10) { /* lógica */ }
}
```

### 4. Value Objects
```csharp
public class Money : IEquatable<Money>
{
    public decimal Amount { get; private set; }
    public string Currency { get; private set; }
    
    // Comportamentos e validações
    public Money Add(Money other) { /* implementação */ }
}
```

## 🔄 Fluxo de Dados

### 1. Fluxo de Comando (Escrita)
```
Frontend → API Controller → Command → Handler → Domain → Repository → Database
    ↑                                                                        │
    └── Response ← DTO ← AutoMapper ← Domain Object ←─────────────────────────┘
```

### 2. Fluxo de Query (Leitura)
```
Frontend → API Controller → Query → Handler → Repository → Database
    ↑                                              │
    └── Response ← PagedResult<DTO> ←─────────────┘
```

### 3. Fluxo de Autenticação
```
Frontend → Keycloak → JWT Token → API → JWT Validation → Protected Resource
```

## 📊 Modelo de Dados

### Core Entities

```typescript
// Product Entity
interface Product {
  id: string;
  name: string;
  description?: string;
  price: Money;
  categoryId: string;
  stock: StockQuantity;
  isActive: boolean;
  sku?: string;
  createdAt: DateTime;
  updatedAt: DateTime;
}

// Category Entity  
interface Category {
  id: string;
  name: string;
  description?: string;
  isActive: boolean;
  createdAt: DateTime;
  updatedAt: DateTime;
}
```

### Value Objects
```typescript
interface Money {
  amount: decimal;
  currency: string; // "BRL", "USD", etc.
}

interface StockQuantity {
  value: number;
}
```

### MongoDB Collections
```javascript
// Products Collection
{
  "_id": ObjectId,
  "name": "string",
  "description": "string?",
  "price": {
    "amount": NumberDecimal,
    "currency": "string"
  },
  "categoryId": "string",
  "stock": {
    "value": NumberInt32
  },
  "isActive": Boolean,
  "sku": "string?",
  "createdAt": ISODate,
  "updatedAt": ISODate
}

// Categories Collection
{
  "_id": ObjectId,
  "name": "string",
  "description": "string?", 
  "isActive": Boolean,
  "createdAt": ISODate,
  "updatedAt": ISODate
}
```

## 🔐 Segurança

### Autenticação Flow
```
1. User → Frontend → Keycloak Login
2. Keycloak → JWT Token → Frontend
3. Frontend → API (with Bearer Token)
4. API → Validate JWT → Keycloak
5. API → Authorize based on roles → Response
```

### Authorization Matrix
| Role    | Products        | Categories      | Users    | Settings |
|---------|----------------|----------------|----------|----------|
| Admin   | Full CRUD      | Full CRUD      | Manage   | Full     |
| Manager | Create/Update  | Full CRUD      | View     | Limited  |
| User    | View Only      | View Only      | None     | None     |

## 🚀 Escalabilidade

### Horizontal Scaling
```
Load Balancer (Nginx)
├── Frontend Instance 1
├── Frontend Instance 2
└── Frontend Instance N

API Gateway  
├── Backend Instance 1
├── Backend Instance 2
└── Backend Instance N

Database Cluster
├── MongoDB Primary
├── MongoDB Secondary 1
└── MongoDB Secondary N
```

### Caching Strategy
```
Level 1: HTTP Cache (Nginx)
Level 2: Application Cache (Memory)
Level 3: Database Query Cache (MongoDB)
Level 4: CDN Cache (Static Assets)
```

## 📈 Performance Considerations

### Database Optimization
```javascript
// Indexes criados
db.products.createIndex({ "name": "text", "description": "text" })
db.products.createIndex({ "categoryId": 1 })
db.products.createIndex({ "isActive": 1 })
db.products.createIndex({ "stock.value": 1 })
db.products.createIndex({ "createdAt": -1 })

db.categories.createIndex({ "name": 1 }, { unique: true })
db.categories.createIndex({ "isActive": 1 })
```

### API Performance
- **Response Time Target**: < 500ms para 95% das requests
- **Throughput Target**: 1000 requests/second
- **Cache Hit Ratio**: > 80% para queries frequentes

### Frontend Performance
- **First Contentful Paint**: < 2s
- **Largest Contentful Paint**: < 3s
- **Bundle Size**: < 1MB gzipped
- **Tree Shaking**: Habilitado para redução de bundle

## 🔍 Monitoramento e Observabilidade

### Métricas Coletadas
```
Business Metrics:
- Total de produtos cadastrados
- Produtos com estoque baixo
- Valor total do inventário
- Usuários ativos

Technical Metrics:
- Response time por endpoint
- Error rate por serviço
- Database connection pool
- Memory e CPU usage
```

### Logging Strategy
```csharp
// Structured logging com Serilog
Log.Information("Product created with {ProductId} by {UserId}", 
    productId, userId);

Log.Warning("Low stock alert for {ProductName}: {CurrentStock}", 
    product.Name, product.Stock);

Log.Error(ex, "Failed to process order {OrderId}", orderId);
```

### Health Checks
```
/health - Overall application health
/health/ready - Readiness probe
/health/live - Liveness probe
/health/db - Database connectivity
/health/keycloak - Auth service status
```

## 📚 Testing Strategy

### Test Pyramid
```
    E2E Tests (5%)
    ├── Critical user journeys
    └── Integration between all systems

  Integration Tests (25%)  
  ├── API endpoints
  ├── Database integration
  └── External service integration

Unit Tests (70%)
├── Domain logic
├── Business rules
├── Validation logic
└── Component behavior
```

### Coverage Targets
- **Unit Tests**: > 90% para Domain layer
- **Integration Tests**: 100% dos endpoints críticos
- **E2E Tests**: Fluxos principais de usuário
- **Overall Coverage**: > 85%

## 🔮 Roadmap Futuro

### Fase 2: Melhorias
- Event Sourcing para auditoria completa
- GraphQL API como alternativa
- Real-time updates com SignalR
- Advanced Analytics Dashboard

### Fase 3: Scalability
- Microservices decomposition
- Kubernetes deployment
- Multi-region setup
- API versioning strategy

### Fase 4: AI/ML
- Product recommendation engine
- Demand forecasting
- Automated inventory management
- Predictive analytics

---

Esta arquitetura foi projetada para atender aos requisitos atuais enquanto permite evolução futura sem quebrar mudanças.
