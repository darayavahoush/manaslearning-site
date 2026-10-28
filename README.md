# Manas Learning - Site and Platform

.NET solution for manaslearning.com, imported from Azure DevOps (project `Apps`, repo `manaslearning`, branch `master`).

## Projects
- `manaslearning.UI` - user-facing web app (login, dashboard, assessments, checkout)
- `manaslearning.Admin.UI` / `manaslearning.Admin.API` / `manaslearning.Admin.Auth.API` - admin portal and APIs
- `manaslearning.Auth.API` - authentication API
- `manaslearning.Business`, `manaslearning.Models`, `manaslearning.Helpers`, `manaslearning.Shared` - core libraries
- `manaslearning.Payment` - payment handling
- `manaslearning.T4`, `manaslearning.UI.UnitTests` - templates and tests

## Deployment
Originally deployed to Azure App Service `prod-ml2` via Azure Pipelines (`azure-pipelines.yml`).

## Current work
Replace the Google Calendar and Calendly links with cal.com booking:
1. Free consultation (15 min, round robin across staff)
2. IEP consultation (30 min)
3. Paid doubt-clearing consultation (Rs 999, tied to the payment gateway)

## Secrets
Do not commit connection strings or keys. Use Azure Key Vault or environment variables.
