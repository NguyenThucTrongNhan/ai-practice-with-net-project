Add workflows: .github/workflows/ci.yml, .github/workflows/cd-deploy.yml, .github/dependabot.yml.

Add K8s manifests under .k8s/.

Add Dockerfile (already done).

Add tests in /tests (you created earlier).

Configure repo secrets in GitHub.

Enable branch protection on main.

Optional: configure SonarCloud + CodeQL in org.

Run PR → CI should run.

Merge to main → CD deploys to staging.

Approve → CD deploys to production.