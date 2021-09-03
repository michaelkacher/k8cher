# K8cher
An opinionated/experimental getting started project leveraging Kubernetes, Tilt, Dapr, and SvelteKit. This is a work in progress!

Skip to the [link](#quick-start)!

I always enjoy "developer experience" and this repository is being leveraged to experiment with different technologies and streamling workflows. I hope to learn valuable lessons and share with others what I am doing. 

When you execute `tilt up` this project currently creates the following in a Kubernetes cluster:
* [Proxy Service - .NET 6 preview 7 Minimal API](./src/k8cher.proxy/README.md)
* Auth service .NET 6 preview 7
* 'Store' service (expirmenting using Dapr actor to sync svelte frontend store)
* Dapr for sidecars, plugin components, and secret management
* PostgreSQL
* pgAdmin
* Database migration with Kubernetes Job
* Local Kubernetes secret store (intended to leverage Dapr secret component for a Key vault in production)

There is a svelteKit frontend that can currently be run seperate `npm run dev`

# Goals
This starter kit leverages [Tilt](https://tilt.dev/) to provide a productive environment for development which includes:
* The ability to start all services and databases with a single command (after initial prereqs installed)
* Automatically reload all services on file changes
* Ability to be deployed to production with minimal changes
* Secrets integration
* Authn/z integration
* Database and pgAdmin tools setup
* Ability to easily integrate your own services and focus on business logic

Get started fast! The tools are preconfigured and kubectl, helm, and other tools can be learned over time.

# Prerequisits
* [Docker Desktop with Kubernetes enabled](https://docs.docker.com/desktop/)
* [Kubectl](https://kubernetes.io/docs/tasks/tools/)
* [Helm CLI](https://helm.sh/docs/intro/install/)
* [Tilt](https://github.com/tilt-dev/tilt/releases) - This is the link to the release download page. On Windows be sure to rename to tilt.exe. [If tools are not accessible on command line follow these steps.](./docs/setup-path.md)


# Quick Start
1) Clone repository `git clone https://github.com/michaelkacher/k8cher`
2) cd into directory `cd k8cher`
3) `tilt up` and press space bar to open the browser to watch the status of the services spinning up.
* Note: There is an intermittent first time bug I am chasing down that will only occur with the bitnami helm chart for postgres (has never occured with other helm charts). When browsing the Tilt Dashboard (the one accessed by pressing space bar) if there is an error in the Tiltfile, execute the following in the terminal: `helm repo add bitnami https://charts.bitnami.com/bitnami` and run again. 

Once the services are all ready (green in the browser Tilt dashboard) it is ready to go! Explore and make changes to code--the services will automatically rebuild and deploy.

The proxy is currently setup to localhost:80. If this conflicts with existing ports, navigate to the [helm chart values](./src/k8cher.proxy/values.yaml) and change the `port` under `service` from 80 to desired port.

## Some items to explose
Visit the swagger: http://localhost/auth/swagger


Register a user: POST localhost/auth/register
```
{
  "user": {
    "userName": "michael.kacher@gmail.com",
    "email": "michael.kacher@gmail.com"
  },
  "password": "Passw0rd_"
}
```

Get a JWT: POST localhost/auth/login
{
  "email": "michael.kacher@gmail.com",
  "password": "Passw0rd_"
}

View PG Admin at localhost:5555 and login
* Email Address / Username: `user@domain.com`
* Password: `bouncingcow` (set in TiltFile for local development)
Then select the database and enter password `postgres`. Now you can explore the schema and run queries against all the data.


## What does tilt up execute?
[Tilt](https://tilt.dev/) is a developer productivity tool that integrates with Kubernetes and provides live updates.

Some items to try:
TODO - mbk: break out into differnt mini-tutorial files
* open swagger
* curl to register user
* curl to login and get jwt
* open database admin
* explorse users
* viewing logs


# Database 


Reacreate migrations - delete Migrations folder content $`dotnet ef migrations add InitialCreate -- --environment Migration`.