# Auth Service
* Experimenting with .NET 6 new Minimal APIs
* Leveraging PostgreSQL and Npgsql.EntityFrameworkCore.PostgreSQL for storage
* Database and migrations information can be found in [k8cher.auth.db](../k8cher.auth.db/README.md)
* Adds an id claim that is leveraged by other services to get user id
* Has an example UserRole claim of admin. Currently on leveraged in SvelteKit web UI to display roles and prevent certain pages from being displayed.
* For local development with Tilt and hot reload the DevelopmentDockerfile is being used. When CI/CD is setup will leverage Dockerfile. The development dockerfile speeds up development time by building code on host machine and mounting build artifacts