# K8cher
An opinionated getting started project leveraging Kubernetes, Tilt, Dapr, and SvelteKit.

I always enjoy "developer experience" and this repository is being leveraged to experiment with different technologies and streamling workflows. I hope to learn valuable lessons and share with others what I am doing. 

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
* [Tilt](https://github.com/tilt-dev/tilt/releases) - This is the link to the release download page. On Windows be sure to rename to tilt.exe and ensure the folder you place it is in in the Windows Environment Variables Path variable.

# Quick Start
1) Clone repository `git clone https://github.com/michaelkacher/k8cher`
2) cd into directory `cd k8cher`
3) `tilt up` and press space bar to open the browser to watch the status of the services spinning up.
* Note: There is an intermittent first time bug I am chasing down that will only occur with the bitnami helm chart for postgres (has never occured with other helm charts). When browsing the Tilt Dashboard (the one accessed by pressing space bar) if there is an error in the Tiltfile, execute the following in the terminal: `helm repo add bitnami https://charts.bitnami.com/bitnami`. 


Once the services are all ready (green in the browser Tilt dashboard) it is ready to go!

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

## Accessing PG Admin
* http://localhost:5555/
* Currently hardcoded:
* Email Address / Username: `user@domain.com`
* Password: `bouncingcow` (set in TiltFile for local development)

Adding migration to authorization database.
* change ./auth-proxy/

Reacreate migrations - delete Migrations folder content $`dotnet ef migrations add InitialCreate -- --environment Migration`.