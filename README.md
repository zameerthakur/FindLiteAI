# FindLiteAI

![Build](https://img.shields.io/github/actions/workflow/status/zameerthakur/FindLiteAI/ci.yml)
![License](https://img.shields.io/github/license/zameerthakur/FindLiteAI)
![NuGet](https://img.shields.io/nuget/v/FindLiteAI)
![Downloads](https://img.shields.io/nuget/dt/FindLiteAI)

Embedded offline AI-powered semantic, keyword, and hybrid search for .NET desktop and server applications.

FindLiteAI is a lightweight, offline-first search engine for .NET applications, enabling semantic, keyword, and hybrid search without Python, Docker, cloud APIs, external vector databases, or AI infrastructure.

It is built for developers who want practical AI-powered search inside ASP.NET Core, Worker Services, WPF, WinForms, desktop tools, intranet systems, enterprise applications, and offline environments.

Designed for lightweight embedded AI retrieval scenarios where simplicity, offline capability, and low operational overhead are important.

---

## NuGet Packages

Most developers should start with the main package:

```bash
dotnet add package FindLiteAI
```

The main `FindLiteAI` package includes the core engine and pulls in the required supporting packages for the default offline search setup.

Package list:

- [FindLiteAI](https://www.nuget.org/packages/FindLiteAI) — main package; start here
- [FindLiteAI.Core](https://www.nuget.org/packages/FindLiteAI.Core) — core abstractions, models, and contracts
- [FindLiteAI.Embeddings.Onnx](https://www.nuget.org/packages/FindLiteAI.Embeddings.Onnx) — ONNX embedding provider
- [FindLiteAI.Storage.LiteDb](https://www.nuget.org/packages/FindLiteAI.Storage.LiteDb) — LiteDB embedded storage provider
- [FindLiteAI.Extensions.DependencyInjection](https://www.nuget.org/packages/FindLiteAI.Extensions.DependencyInjection) — dependency injection helpers
- [FindLiteAI.AspNetCore](https://www.nuget.org/packages/FindLiteAI.AspNetCore) — ASP.NET Core integration

---

# Package Architecture

| Package | Purpose |
|---|---|
| FindLiteAI | Main package. Add this first for the default offline search engine setup. |
| FindLiteAI.Core | Core abstractions, models, contracts, and search primitives. |
| FindLiteAI.Embeddings.Onnx | ONNX-based embedding provider used for local AI embeddings. |
| FindLiteAI.Storage.LiteDb | LiteDB-based embedded storage provider for documents and embeddings. |
| FindLiteAI.Extensions.DependencyInjection | Dependency injection extensions for easy .NET application setup. |
| FindLiteAI.AspNetCore | Optional ASP.NET Core integration for Minimal API search endpoints. |

---

# Why FindLiteAI?

Most AI search stacks require:

- Python environments
- Docker infrastructure
- external vector databases
- OpenAI or cloud APIs
- GPU infrastructure
- complex orchestration tools

FindLiteAI takes a different approach.

It provides:

- embedded offline AI-powered search
- lightweight local storage
- local ONNX models
- pure .NET integration
- simple NuGet-based setup
- zero external infrastructure after model download

---

# Key Features

- Semantic search
- Keyword search
- Hybrid search
- Offline-first architecture
- Local ONNX embedding models
- LiteDB embedded storage
- ASP.NET Core integration
- WPF desktop integration
- Worker Service support
- Automatic model package installation
- No OpenAI dependency
- No Python dependency
- No Docker dependency
- No GPU required
- Cross-platform .NET support
- Lightweight deployment model

---

# Supported Search Modes

| Search Mode | Description |
|---|---|
| Semantic | Finds meaning-based matches using AI embeddings |
| Keyword | Finds exact keyword matches |
| Hybrid | Combines semantic and keyword ranking |

---

# Built-In Models

| Model | Profile | Dimensions | Recommended RAM |
|---|---|---|---|
| all-MiniLM-L6-v2 | Fast | 384 | 4 GB+ |
| all-mpnet-base-v2 | Balanced | 768 | 8 GB+ |
| Snowflake Arctic Embed XS | Advanced | 384 | 8 GB+ |

All models run locally using ONNX Runtime.

---

# Installation

## Default Installation

### Desktop Applications (WPF, WinForms, Console, Worker Services)

For most desktop and general .NET applications, install the main package:

```bash
dotnet add package FindLiteAI
```

This package includes the core search engine and automatically installs the required ONNX embedding and LiteDB storage dependencies.

Suitable for:

- WPF applications
- WinForms applications
- Console applications
- Worker Services
- Background processing applications
- Offline desktop tools
- Local enterprise utilities

---

### ASP.NET Core Applications

For ASP.NET Core applications, install the ASP.NET integration package:

```bash
dotnet add package FindLiteAI.AspNetCore
```

This package includes:

- FindLiteAI
- ASP.NET Core endpoint integration
- dependency injection extensions
- required core dependencies

Suitable for:

- ASP.NET Core Web APIs
- internal enterprise APIs
- intranet systems
- web-based knowledge systems
- AI-powered search APIs

---

## Optional Packages

Install these packages only if you need direct access to a specific component.

### ONNX Embedding Provider

```bash
dotnet add package FindLiteAI.Embeddings.Onnx
```

Use when:

- building custom embedding pipelines
- using ONNX embeddings independently
- creating custom search architectures

---

### LiteDB Storage Provider

```bash
dotnet add package FindLiteAI.Storage.LiteDb
```

Use when:

- accessing the LiteDB storage provider directly
- building custom persistence implementations
- extending storage behavior

---

### Dependency Injection Extensions

```bash
dotnet add package FindLiteAI.Extensions.DependencyInjection
```

Use when:

- configuring services manually
- integrating with custom host architectures
- building advanced dependency injection setups

---

### Core Abstractions and Models

```bash
dotnet add package FindLiteAI.Core
```

Use when:

- building custom providers
- implementing custom storage engines
- creating alternative embedding providers
- extending FindLiteAI internals

# Quick Start

## ASP.NET Core Example

```csharp
using FindLiteAI.Core.Abstractions;
using FindLiteAI.Core.Models;
using FindLiteAI.Embeddings.Onnx;
using FindLiteAI.Extensions.DependencyInjection;

WebApplicationBuilder builder =
    WebApplication.CreateBuilder(args);

string cacheDirectory =
    Path.Combine(
        Path.GetTempPath(),
        "FindLiteAI",
        "Models");

await ModelInstallService.InstallAsync(
    FindLiteAIModels.MiniLm,
    cacheDirectory);

builder.Services.AddFindLiteAI(options =>
{
    options.DatabasePath = "findliteai.db";

    options.ModelCacheDirectory =
        Path.Combine(
            cacheDirectory,
            FindLiteAIModels.MiniLm.Id);
});

WebApplication app =
    builder.Build();

app.MapGet(
    "/",
    () => "FindLiteAI running.");

app.Run();
```

---

# Adding Documents

```csharp
await engine.AddAsync(
    "logs",
    new SemanticDocument
    {
        Text = "SMTP email relay timeout occurred."
    });
```

Document identifiers are automatically generated if not provided.

---

# Semantic Search

```csharp
IReadOnlyList<SearchResult> results =
    await engine.SearchAsync(
        "logs",
        "email sending issue",
        new SearchOptions
        {
            SearchMode = SearchMode.Semantic,
            MaxResults = 5,
            MinimumScore = 0.10
        });
```

---

# Hybrid Search

```csharp
IReadOnlyList<SearchResult> results =
    await engine.SearchAsync(
        "logs",
        "smtp issue",
        new SearchOptions
        {
            SearchMode = SearchMode.Hybrid,
            MaxResults = 5,
            MinimumScore = 0.10
        });
```

---

# Example Use Cases

## Log Search

Stored log:

```text
SMTP relay timeout occurred.
```

User searches:

```text
email sending issue
```

FindLiteAI can return semantically related results without exact keyword matching.

---

## Helpdesk Systems

Stored ticket:

```text
VPN authentication failed during remote access.
```

User searches:

```text
cannot connect remotely
```

FindLiteAI can retrieve related support tickets using semantic similarity.

---

## Enterprise Knowledge Bases

Stored document:

```text
Annual leave approval workflow.
```

User searches:

```text
vacation request process
```

FindLiteAI can retrieve semantically related policies and procedures.

---

# Architecture

```text
Application
    ↓
FindLiteAI Engine
    ↓
ONNX Embedding Provider
    ↓
LiteDB Embedded Storage
```

Documents are converted into embeddings using local ONNX models and stored alongside metadata inside LiteDB.

Search queries generate embeddings which are compared against stored vectors to retrieve similar content.

---

# Offline-First Design

After model packages are downloaded once:

- all searches run locally
- all embeddings run locally
- no cloud calls are required
- no external AI services are required

This makes FindLiteAI suitable for:

- intranet systems
- enterprise desktop applications
- restricted networks
- government environments
- industrial applications
- offline-capable systems

---

# Who Is This For?

FindLiteAI is designed for:

- .NET developers
- ASP.NET Core developers
- WPF developers
- Worker Service developers
- internal enterprise tools
- desktop applications
- intranet systems
- offline business applications
- embedded search scenarios

---

# Samples

The repository includes:

| Sample | Description |
|---|---|
| Console Sample | Validates all official model packages |
| ASP.NET Core Sample | Demonstrates API integration |
| Worker Service Sample | Demonstrates background service integration |
| WPF Sample | Demonstrates desktop application integration |

See:

```text
samples/README.md
```

for detailed setup instructions.

---

# Model Packages

Official model packages are distributed as ZIP packages through GitHub Releases.

The package system supports:

- automatic download
- automatic extraction
- local caching
- offline reuse

---

# Storage

FindLiteAI currently uses LiteDB for embedded local storage.

LiteDB stores:

- document text
- metadata
- embeddings
- indexes

No external database server is required.

---

# Current Scope

FindLiteAI focuses on:

- lightweight embedded AI search
- practical .NET integration
- offline-first deployment
- simple developer experience

It is intentionally not:

- a chatbot framework
- a vector database server
- an LLM platform
- a cloud AI orchestration system

---

# Limitations

FindLiteAI currently focuses on lightweight embedded retrieval scenarios.

Current limitations include:

- no distributed clustering
- no GPU acceleration
- no incremental model training
- no built-in reranking models
- no distributed vector database support

The project intentionally prioritizes simplicity, portability, and local deployment.

---

# Roadmap

Planned future improvements may include:

- metadata filtering
- additional embedding providers
- custom model registration
- batch optimization
- reranking improvements
- optional SQLite provider
- additional storage providers

---

# Requirements

- .NET 8
- ONNX Runtime compatible environment

No GPU required.

---

# License

MIT License

---

# Contributing

Contributions, issues, feature requests, and improvements are welcome.

---

# Status

Current version:

```text
v0.1.0
```

FindLiteAI is under active development.

---

# Open Source Credits

FindLiteAI is built using several excellent open source projects and publicly available embedding models.

## Embedding Models

### all-MiniLM-L6-v2

Source:

```text
https://huggingface.co/sentence-transformers/all-MiniLM-L6-v2
```

License:

```text
Apache-2.0
```

---

### all-mpnet-base-v2

Source:

```text
https://huggingface.co/sentence-transformers/all-mpnet-base-v2
```

License:

```text
Apache-2.0
```

---

### Snowflake Arctic Embed XS

Source:

```text
https://huggingface.co/Snowflake/snowflake-arctic-embed-xs
```

License:

```text
Apache-2.0
```

---

## Runtime and Libraries

### ONNX Runtime

Source:

```text
https://github.com/microsoft/onnxruntime
```

License:

```text
MIT
```

---

### LiteDB

Source:

```text
https://github.com/litedb-org/LiteDB
```

License:

```text
MIT
```

---

### Microsoft.ML.Tokenizers

Source:

```text
https://github.com/dotnet/machinelearning
```

License:

```text
MIT
```

Used during development and related tooling workflows.

---

FindLiteAI itself is released under the MIT License.

---

# Links

- Repository: https://github.com/zameerthakur/FindLiteAI
- NuGet: https://www.nuget.org/packages/FindLiteAI
- ONNX Runtime: https://onnxruntime.ai
- LiteDB: https://www.litedb.org
