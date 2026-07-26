# Agent Instructions — Git Workflow

These rules are mandatory for any agent creating branches or commits in this repository.

---

## 1. Always start from the latest changes

Never branch off a stale local state. Before creating a branch, sync with the remote.

```bash
# Make sure nothing is uncommitted first
git status

# Switch to the default branch and pull the latest state
git checkout main
git fetch origin --prune
git pull --ff-only origin main
```

If `git status` shows uncommitted work, stash it before switching:

```bash
git stash push -u -m "wip"
# ... after branching ...
git stash pop
```

> Replace `main` with the repository's actual default branch if it differs
> (`master`, `develop`, …). Verify with `git remote show origin | grep "HEAD branch"`.

---

## 2. Create the branch from the latest changes

```bash
git checkout -b feat/VZ-01
```

Push it and set the upstream on the first push:

```bash
git push -u origin feat/VZ-01
```

One-liner alternative that guarantees the branch is cut from the freshly fetched remote
state, regardless of the local branch position:

```bash
git fetch origin
git checkout -b feat/VZ-01 origin/main
```

---

## 3. Branch naming

| Purpose | Prefix | Example |
|---|---|---|
| New feature | `feat/` | `feat/VZ-00`, `feat/VZ-01`, `feat/VZ-12` |
| Bug fix | `fix/` | `fix/VZ-03`, `fix/VZ-27` |

Rules:

- The ticket key is always uppercase `VZ-` followed by the ticket number: `VZ-00`, `VZ-01`, `VZ-02`, …
- Prefix is always lowercase, separated from the key by a forward slash.
- One branch per ticket. Do not bundle unrelated tickets into a single branch.
- An optional short kebab-case suffix is allowed for readability:
  `feat/VZ-05-exchange-rate-cache`.

---

## 4. Commit messages — Conventional Commits

Follow the [Conventional Commits v1.0.0-beta.2](https://www.conventionalcommits.org/en/v1.0.0-beta.2/#summary) specification.

### Structure

```
<type>[optional scope]: <description>

[optional body]

[optional footer]
```

### Rules

1. Every commit **must** be prefixed with a type — a noun such as `feat` or `fix` — followed by a colon and a space.
2. Use `feat` when the commit adds a new feature (maps to a `MINOR` version bump).
3. Use `fix` when the commit patches a bug (maps to a `PATCH` version bump).
4. A scope **may** follow the type, in parentheses, naming the section of the codebase touched: `fix(parser): …`.
5. A short description **must** immediately follow the type/scope prefix. Use the imperative mood, lowercase, no trailing period.
6. A longer body **may** follow, separated from the description by one blank line, giving additional context.
7. A footer **may** follow the body after one blank line, and should carry issue references such as `Fixes #13`.
8. Breaking changes **must** be flagged at the very start of the body or footer with the uppercase text `BREAKING CHANGE: `, followed by a description of what changed in the API. A breaking change may accompany any type and maps to a `MAJOR` version bump.
9. The footer must contain only `BREAKING CHANGE`, external links, issue references and similar meta-information.

### Allowed types

| Type | Use for |
|---|---|
| `feat` | A new feature |
| `fix` | A bug fix |
| `docs` | Documentation only |
| `style` | Formatting, whitespace, no behaviour change |
| `refactor` | Code change that neither fixes a bug nor adds a feature |
| `perf` | Performance improvement |
| `test` | Adding or correcting tests |
| `chore` | Build process, tooling, dependencies |
| `improvement` | Improves an existing implementation without adding a feature or fixing a bug |

Only `feat` and `fix` carry semantic-versioning meaning. The rest are informational unless they contain a `BREAKING CHANGE`.

### Examples

```
docs: correct spelling of CHANGELOG
```

```
feat(rates): add ability to parse multi-currency responses
```

```
fix(cli): handle empty input on currency prompt

The prompt threw a NullReferenceException when the user pressed Enter
without typing anything.

Fixes VZ-03
```

```
feat: allow provided config object to extend other configs

BREAKING CHANGE: `extends` key in config file is now used for extending other config files
```

---

## 5. Committing

```bash
git add <specific files>          # prefer explicit paths over `git add .`
git commit -m "feat(rates): add currency conversion endpoint"
```

For a body and footer, pass repeated `-m` flags — each becomes a paragraph separated by a blank line:

```bash
git commit -m "fix(cli): handle empty input on currency prompt" \
           -m "The prompt threw a NullReferenceException when the user pressed Enter." \
           -m "Fixes VZ-03"
```

Then push:

```bash
git push
```

---

## 6. Rules for agents

- **Never commit directly to the default branch.** Always work on a `feat/VZ-` or `fix/VZ-` branch.
- **Never force-push** to a shared branch (`main`, or any branch another agent or person is working on).
- If a commit conforms to more than one type, split it into multiple commits.
- If the wrong type was used and the commit has not been merged or released, fix it with `git rebase -i` before opening the pull request.
- Do not amend or rebase commits that have already been pushed and reviewed.
- Do not commit secrets, `.env` files, API keys, `bin/`, or `obj/` directories.
- Verify the build passes before committing (`dotnet build`).
