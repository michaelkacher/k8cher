# Reverse Proxy
* leveraging Microsoft Reverse Proxy
* Experimenting with .NET 6 new Minimal APIs
* the [configuration](./values.yaml) is setup to be a Kubernetes service of type `LoadBalancer` that is defaulting on localhost:8088
* all /auth/* traffic sent to k8cher.auth service
* all /store/* traffic sent to k8cher.store service
