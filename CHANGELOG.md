# Changelog

All notable changes to this project will be documented in this file.

This project follows semantic versioning.

---

# [0.1.0] - 2026-05-14

Initial public release.

## Added

### Core Engine

- semantic search support
- keyword search support
- hybrid search support
- local embedding generation
- automatic document embedding indexing
- automatic document identifier generation
- similarity search support
- document deletion support

### Embedding System

- ONNX Runtime embedding provider
- model package metadata system
- automatic model package installation
- automatic ZIP extraction
- local model cache support
- model validation support

### Official Models

Added support for:

- all-MiniLM-L6-v2
- all-mpnet-base-v2
- Snowflake Arctic Embed XS

### Storage

- LiteDB embedded semantic storage provider
- local vector persistence
- embedded deployment support

### ASP.NET Core

- Minimal API integration
- dependency injection support
- health endpoint
- batch document indexing endpoint
- semantic search endpoints
- keyword search endpoints
- hybrid search endpoints
- similar document search endpoint

### Samples

Added:

- Console sample
- ASP.NET Core sample
- Worker Service sample
- WPF desktop sample

### Developer Experience

- automatic model download workflow
- progress reporting support
- XML documentation comments
- integration tests
- end-to-end validation samples
- offline-first architecture

### Documentation

- root README
- sample documentation
- model package documentation
- HTTP request examples

### Infrastructure

- model package registry
- model package resolver
- model package validation
- GitHub Release model package support
