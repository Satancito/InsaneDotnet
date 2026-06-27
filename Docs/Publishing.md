# Publishing

This repository supports two NuGet publishing flows for `InsaneIO.Insane`:

- GitHub Actions trusted publishing with NuGet.org OIDC

The GitHub Actions flow is the recommended path for repeatable releases because it does not require storing a long-lived NuGet API key in the repository.

## Trusted Publishing

Trusted publishing uses GitHub Actions OpenID Connect to obtain a short-lived NuGet.org API key during the workflow run.

Repository workflow:

- [InsaneIO.Insane-TrustedPublish.yml](../.github/workflows/InsaneIO.Insane-TrustedPublish.yml)

NuGet.org setup:

1. Sign in to `nuget.org`.
2. Open `Trusted Publishing`.
3. Create a policy for this repository.
4. Use:
   - Repository owner: `Satancito`
   - Repository: `InsaneDotnet`
   - Workflow file: `InsaneIO.Insane-TrustedPublish.yml`
5. Leave environment empty unless you later add a GitHub Actions environment restriction.

GitHub repository setup:

1. Open repository settings.
2. Add a repository variable named `NUGET_ORG_USER`.
3. Set it to your NuGet.org profile name.

The workflow requests `id-token: write`, builds the package, runs the test suite, packs `InsaneIO.Insane`, logs in with `NuGet/login@v1`, and pushes both `.nupkg` and `.snupkg` artifacts to `https://api.nuget.org/v3/index.json`.

## Triggering The Workflow

The workflow can run in two ways:

- Manually through `workflow_dispatch`
- Automatically when you push to `main`

Manual run:

1. Open the `Actions` tab in GitHub.
2. Open `Publish InsaneIO.Insane to NuGet.org`.
3. Click `Run workflow`.
4. Select the `main` branch.

Automatic example:

```powershell
git push origin main
```

The publish job is guarded with:

```yaml
if: github.ref == 'refs/heads/main'
```

That means the package publish path only runs when the workflow reference is the `main` branch.

## Recommendation

Use trusted publishing as the primary release path for this repository.
