# FindLiteAI Samples

This folder contains sample applications that demonstrate how to use FindLiteAI in different .NET application types.

---

# Console Sample

## Project

```text
FindLiteAI.Sample.Console
```

## What It Does

The console sample validates all official FindLiteAI model packages.

It will:

- download model ZIP packages from GitHub Releases
- extract models into a local cache folder
- load ONNX models
- generate embeddings
- verify expected embedding dimensions

Models validated:

| Model | Profile | Dimensions |
|---|---|---|
| all-MiniLM-L6-v2 | Fast | 384 |
| all-mpnet-base-v2 | Balanced | 768 |
| Snowflake Arctic Embed XS | Advanced | 384 |

## How To Run

1. Open the solution in Visual Studio 2022.

2. Right-click this project:

```text
FindLiteAI.Sample.Console
```

3. Click:

```text
Set as Startup Project
```

4. Press:

```text
F5
```

or click:

```text
Start Debugging
```

## Expected Output

```text
All FindLiteAI models validated successfully.
```

First run may take time because model packages are downloaded.

---

# ASP.NET Core Sample

## Project

```text
FindLiteAI.Sample.AspNetCore
```

## What It Does

The ASP.NET Core sample demonstrates FindLiteAI as a web API.

It supports:

- document indexing
- semantic search
- keyword search
- hybrid search
- similar document search
- document deletion

## How To Run

1. Open the solution in Visual Studio 2022.

2. Right-click this project:

```text
FindLiteAI.Sample.AspNetCore
```

3. Click:

```text
Set as Startup Project
```

4. Press:

```text
F5
```

5. Browser should open:

```text
https://localhost:7014/
```

Expected response:

```text
FindLiteAI ASP.NET Core sample is running.
```

---

# Testing ASP.NET Core API Requests

## HTTP Request File

Open this file:

```text
FindLiteAI.Sample.AspNetCore.http
```

Full path:

```text
samples/FindLiteAI.Sample.AspNetCore/FindLiteAI.Sample.AspNetCore.http
```

## How To Send Requests

Inside Visual Studio:

1. Open the `.http` file.
2. Keep the ASP.NET Core sample running.
3. Click:

```text
Send Request
```

above each request block.

## Recommended Request Order

Run the requests in this order:

1. Root Health Check
2. Add Document - Login Failure
3. Add Document - Database Timeout
4. Add Document - Email Relay
5. Hybrid Search - Login Issue
6. Semantic Search - Database Problem
7. Keyword Search - SMTP
8. Similar Search - Find documents similar to Login Failure
9. Delete Document - Email Relay
10. Search After Delete - SMTP

## Important

Make sure this line matches the running port:

```http
@FindLiteAI_HostAddress = https://localhost:7014
```

If Visual Studio uses a different port, update this value.

---

# Model Cache

Samples use a temporary cache directory:

```text
%TEMP%\FindLiteAI\Models
```

Example:

```text
C:\Users\<user>\AppData\Local\Temp\FindLiteAI\Models
```

First run downloads models. Later runs reuse the cached models.

To force re-download, delete:

```text
%TEMP%\FindLiteAI\Models
```

---

# Notes

FindLiteAI runs fully offline after model packages are downloaded and extracted.

No Python, Docker, Ollama, OpenAI API, or external AI server is required.
