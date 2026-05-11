# FindLiteAI Coding Standards

## Overview

FindLiteAI is a lightweight offline AI-powered semantic, keyword, and hybrid retrieval engine for .NET.

The codebase must remain:

- lightweight
- maintainable
- production-oriented
- dependency-conscious
- offline-first
- developer-friendly
- extensible
- infrastructure-simple

---

## General Principles

### Prefer Simplicity

Prefer simple, readable implementations over complex abstractions.

Avoid:

- premature optimization
- overengineering
- unnecessary design patterns
- unnecessary frameworks

### Keep Dependencies Minimal

Dependencies should only be added when they provide clear long-term value.

Avoid:

- large frameworks
- unnecessary transitive dependencies
- AI frameworks not required for core functionality

### Maintain Offline-First Design

FindLiteAI must work without:

- cloud APIs
- OpenAI services
- external AI servers
- Docker
- Python runtimes
- Ollama

---

## Repository Structure

```text
FindLiteAI/
 ├── src/
 ├── samples/
 ├── tests/
 ├── docs/
 ├── tools/
 └── assets/
```

---

## Project Responsibilities

### FindLiteAI.Core

Must contain ONLY:

- interfaces
- abstractions
- models
- enums
- contracts
- options
- shared primitives

Must NOT reference:

- LiteDB
- ONNX Runtime
- ASP.NET Core
- WPF
- UI frameworks
- storage providers

---

### FindLiteAI

Contains:

- orchestration logic
- indexing engine
- search engine
- hybrid ranking
- collection coordination

---

### FindLiteAI.Embeddings.Onnx

Contains:

- ONNX Runtime integration
- tokenizer handling
- embedding generation
- model loading
- model warmup

---

### FindLiteAI.Storage.LiteDb

Contains:

- LiteDB persistence
- vector storage
- metadata persistence
- collection storage

---

### FindLiteAI.Extensions.DependencyInjection

Contains:

- IServiceCollection extensions
- registration helpers
- dependency injection setup

---

### FindLiteAI.AspNetCore

Contains:

- endpoint mapping
- minimal APIs
- HTTP DTOs
- ASP.NET Core integration

---

### FindLiteAI.Cli

Contains:

- CLI commands
- model install tools
- diagnostics
- benchmark utilities

---

## Naming Conventions

| Type | Convention |
|---|---|
| Interfaces | `IName` |
| Classes | `PascalCase` |
| Methods | `PascalCase` |
| Properties | `PascalCase` |
| Fields | `_camelCase` |
| Local variables | `camelCase` |
| Async methods | `Async` suffix |
| Constants | `PascalCase` |

---

## File Organization

Prefer one public type per file.

File name should match type name.

Example:

```text
SemanticDocument.cs
```

contains:

```csharp
public sealed class SemanticDocument
```

---

## XML Documentation

All public:

- classes
- interfaces
- enums
- methods
- properties

must include XML documentation.

Example:

```csharp
/// <summary>
/// Represents a searchable document within FindLiteAI.
/// </summary>
public sealed class SemanticDocument
{
}
```

Public methods should document:

- parameters
- return values
- exceptions when appropriate

Example:

```csharp
/// <summary>
/// Searches documents within a collection.
/// </summary>
/// <param name="collection">The collection name.</param>
/// <param name="query">The search query.</param>
/// <param name="cancellationToken">
/// A token used to cancel the operation.
/// </param>
/// <returns>
/// A list of ranked search results.
/// </returns>
Task<IReadOnlyList<SearchResult>> SearchAsync(
    string collection,
    string query,
    CancellationToken cancellationToken = default);
```

---

## Nullability

Nullable reference types remain enabled permanently.

Do NOT use:

```csharp
#nullable disable
```

Always prefer explicit nullability handling.

Example:

```csharp
string? optionalValue
```

instead of suppressing warnings.

---

## Async Guidelines

### Use Async APIs for I/O

Use:

- Task
- ValueTask

for:

- database operations
- model loading
- indexing
- search operations

---

### Async Naming

Async methods MUST end with:

```text
Async
```

Example:

```csharp
SearchAsync
LoadModelAsync
AddDocumentAsync
```

---

### CancellationToken

CancellationToken should always be the LAST parameter.

Example:

```csharp
Task SearchAsync(
    string query,
    CancellationToken cancellationToken = default)
```

---

## Collection Guidelines

Prefer immutable/read-only collections.

Use:

- IReadOnlyList<T>
- IReadOnlyCollection<T>

Avoid exposing mutable collections publicly.

---

## Class Design

### Prefer Sealed Classes

Use:

```csharp
public sealed class
```

unless inheritance is intentionally required.

---

### Keep Classes Focused

Each class should have a single responsibility.

Avoid:

- giant service classes
- multi-purpose utility classes
- mixed infrastructure/business logic

---

## Dependency Rules

Dependencies should flow inward.

Preferred dependency direction:

```text
Core
  ↑
Main Engine
  ↑
Providers
  ↑
ASP.NET / CLI / Samples
```

Core should remain dependency-light and framework-agnostic.

---

## Exception Handling

Use exceptions for exceptional situations only.

Avoid:

- swallowing exceptions
- empty catch blocks

Always preserve useful context.

Example:

```csharp
throw new InvalidOperationException(
    "Failed to load embedding model.",
    exception);
```

---

## Logging Guidelines

Logs should be:

- actionable
- concise
- structured

Avoid noisy logs.

Good:

```csharp
_logger.LogInformation(
    "Loaded embedding model '{ModelName}'.",
    modelName);
```

Bad:

```csharp
_logger.LogInformation("Done.");
```

---

## Comments

Write comments only when they add value.

Avoid redundant comments.

Good:

```csharp
// Warm up ONNX session during startup to reduce first-query latency.
```

Bad:

```csharp
// Create variable
var value = 1;
```

---

## Testing Guidelines

### Unit Tests

Unit tests should:

- validate behavior
- avoid unnecessary mocks
- remain deterministic

---

### Integration Tests

Integration tests should validate:

- ONNX runtime behavior
- LiteDB persistence
- hybrid search flow
- indexing/search pipelines

---

## Performance Principles

Avoid:

- unnecessary allocations
- excessive LINQ chaining
- unnecessary boxing
- repeated model loads

Prefer:

- caching
- pooling
- batching where useful

---

## Public API Design

Public APIs should be:

- simple
- predictable
- discoverable
- IntelliSense-friendly

Avoid:

- overly generic APIs
- complex inheritance hierarchies
- unnecessary abstractions

---

## Versioning Principles

Breaking API changes should be minimized.

NuGet package versioning should follow:

```text
MAJOR.MINOR.PATCH
```

Example:

```text
1.0.0
1.1.0
1.1.1
2.0.0
```

---

## Git Commit Guidelines

Prefer small focused commits.

Good examples:

```text
Add ONNX embedding provider abstraction
Add LiteDB collection persistence
Add semantic search ranking pipeline
```

Avoid:

```text
Fixed stuff
Updates
Changes
```

---

## Pull Request Guidelines

Pull requests should:

- stay focused
- include clear descriptions
- avoid unrelated changes
- maintain architecture boundaries

---

## Guiding Philosophy

Build FindLiteAI as:

> Lightweight offline AI-powered semantic, keyword, and hybrid retrieval for .NET.

Do NOT build it as:

> A massive AI framework requiring complex infrastructure.

The project should always remain:

- practical
- lightweight
- offline-first
- maintainable
- production-oriented
- developer-friendly