# GitHub Actions → AWS (OIDC) · ECR

Pipeline: [`.github/workflows/aws-ecr-ecs.yml`](../workflows/aws-ecr-ecs.yml)

| Event | Jobs |
|-------|------|
| **Pull request** → `main` | `validate` (lint, tests, Semgrep, TruffleHog) |
| **Push** → `main` | `validate` → `build-push` (push image to ECR) |

Tools (free / OSS): `dotnet format`, xUnit, [Semgrep](https://semgrep.dev/), [TruffleHog](https://github.com/trufflesecurity/trufflehog). No `AWS_ACCESS_KEY_ID` in GitHub — authentication uses **OIDC**.

---

## 1. AWS — OIDC provider (once per account)

If it does not exist yet:

1. IAM → **Identity providers** → **Add provider** → **OpenID Connect**
2. Provider URL: `https://token.actions.githubusercontent.com`
3. Audience: `sts.amazonaws.com`

---

## 2. IAM role for GitHub Actions

1. Create a **Web identity** role → provider `token.actions.githubusercontent.com`.
2. Attach the trust policy from [`oidc-trust-policy.json`](oidc-trust-policy.json):
   - Replace `ACCOUNT_ID`, `ORG`, `REPO`.
   - Use `ref:refs/heads/main` and/or `environment:production` (the workflow uses the `production` environment on push to `main`).
3. Attach permissions from [`iam-policy-ecr.json`](iam-policy-ecr.json):
   - Replace `AWS_REGION`, `ACCOUNT_ID`, `ECR_REPOSITORY`.
4. Note the role ARN, for example: `arn:aws:iam::ACCOUNT_ID:role/github-actions-templateapi`.

---

## 3. ECR repository

```bash
aws ecr create-repository \
  --repository-name templateapi \
  --image-scanning-configuration scanOnPush=true
```

---

## 4. GitHub configuration

### Variables (`Settings` → `Secrets and variables` → `Actions` → **Variables**)

| Variable | Example | Purpose |
|----------|---------|---------|
| `AWS_REGION` | `us-east-1` | ECR region |
| `AWS_ROLE_ARN` | `arn:aws:iam::123456789012:role/github-actions-templateapi` | Assume role via OIDC |
| `ECR_REPOSITORY` | `templateapi` | ECR repository name |

### Secrets

With **OIDC**, you do not need `AWS_ACCESS_KEY_ID` or `AWS_SECRET_ACCESS_KEY`.

| Secret (optional) | When |
|-------------------|------|
| `SEMGREP_APP_TOKEN` | Semgrep App / Team rules (OSS rules do not require it) |

### Environment `production`

1. `Settings` → `Environments` → **New environment** → `production`
2. Repeat the variables if the trust policy scopes to `environment:production`
3. Recommended: **Required reviewers** and **Deployment branches** = `main` only

---

## 5. Security recommendations

1. **OIDC** instead of long-lived access keys.
2. **Least privilege** — policy scoped to a single ECR repository ARN.
3. **Trust policy** — restrict to `main` or `environment:production`; avoid `repo:ORG/REPO:*`.
4. **Branch protection** — require the `validate` status check before merge.
5. **ECR** — scan on push and a lifecycle policy for untagged images.
6. **TruffleHog** on every PR; use `.trufflehogignore` only for known false positives.
7. **Semgrep** — `p/csharp`, `p/docker`, `p/secrets`.

---

## 6. Local parity

```bash
dotnet format TemplateApi.sln --verify-no-changes
dotnet test TemplateApi.sln -c Release
docker build -t templateapi:local .
```

Semgrep and TruffleHog: install the CLIs locally or rely on CI.
