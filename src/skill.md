# skill.md

## Skill Name
RepoMap-Orchestrator

## Version
1.0.0

## Description
Executable skill for large-repo analysis using top-down context mapping and delegated sub-agent processing. Designed for deterministic behavior, token efficiency, and composable outputs.

---

## Invocation

### Triggers
Invoke this skill when any of the following intents are detected:
- "analyze repo", "understand codebase", "map project"
- "where is X implemented?"
- "summarize architecture"
- "find bugs / risks in repo"

### Required Inputs
```json
{
  "repo_root": "<path>",
  "goal": "<analysis_goal>",
  "constraints": {
    "max_files_per_chunk": 15,
    "max_tokens": "optional"
  }
}
```

### Optional Inputs
```json
{
  "focus_paths": ["/auth", "/api"],
  "exclude_paths": ["/node_modules", "/dist"],
  "output_format": "markdown | json"
}
```

---

## Outputs

### Primary Output (Markdown)
- Repository Overview
- Architecture Summary
- Key Modules & Responsibilities
- Data Flow
- Dependencies
- Risks / Code Smells
- Recommendations

### Secondary Output (Machine-Readable)
```json
{
  "architecture": "",
  "modules": [],
  "data_flow": [],
  "risks": [],
  "recommendations": []
}
```

---

## Tooling Interface

### Required Tools
- `list_dir(path)` → returns directory contents
- `read_file(path)` → returns file content
- `search(pattern)` → optional, for discovery

---

## Execution Engine

### Phase 1: Repository Mapping

**Algorithm:**
```
queue = [repo_root]
structure = {}

while queue not empty:
  path = queue.pop()
  items = list_dir(path)
  structure[path] = items

  for item in items:
    if item is directory and not excluded:
      queue.push(item)
```

**Outputs:**
- Condensed tree
- Entry point candidates
- Module groupings

---

### Phase 2: Planning

**Chunking Logic:**
```
files = collect_files(structure)
chunks = split(files, size=10-15)
```

**Task Template:**
```json
{
  "task_id": "",
  "files": [],
  "objective": "",
  "expected_output": "structured_summary"
}
```

---

### Phase 3: Sub-Agent Execution

#### Sub-Agent Prompt Template
```
You are a code analysis agent.

Scope:
- Only analyze the provided files
- Do not assume context outside them

Objective:
{objective}

Files:
{file_list}

Output JSON ONLY:
{
  "module_name": "",
  "responsibilities": [],
  "key_functions": [],
  "dependencies": [],
  "notable_patterns": [],
  "issues_or_risks": []
}
```

#### Execution Rules
- Max 15 files per task
- Cache results
- No cross-task leakage

---

### Phase 4: Aggregation

**Merge Strategy:**
```
for each sub_output:
  merge into global_model
  deduplicate modules
  reconcile dependencies
```

**Conflict Resolution:**
- If conflicting interpretations → include both
- Mark with `uncertainty: high`

---

### Phase 5: Synthesis

Generate:
- Architecture classification
- Data flow graph (textual)
- Dependency relationships

---

## Constraints

### Hard
- Never read full repo blindly
- Max 15 files per sub-agent
- Structured outputs only

### Soft
- Prefer summaries over raw code
- Highlight uncertainty

---

## Heuristics

- >200 files → aggressive chunking
- Config files first
- Entry points: main/app/index/server
- Infer architecture from imports + folder naming

---

## Failure Modes

### Unknown Structure
- Request directory listing

### Token Exhaustion
- Stop reading
- Return partial + next steps

### Conflicts
- Surface both interpretations

---

## Example Execution

### Input
```json
{
  "repo_root": "/project",
  "goal": "understand authentication system"
}
```

### Generated Task
```json
{
  "task_id": "auth-1",
  "files": [
    "/auth/login.py",
    "/auth/middleware.py",
    "/auth/models.py"
  ],
  "objective": "Analyze authentication flow"
}
```

---

## Extensibility Hooks

- Add refactoring module
- Add test coverage analyzer
- Add API doc generator

---

## End of Skill

