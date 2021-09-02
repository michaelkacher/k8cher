# allow for remote helm charts
load('ext://helm_remote', 'helm_remote')

# Create a local kubernetes secret store. This is only for local development and not secure!!!
# For production implement a a dapr secret store component (https://docs.dapr.io/reference/components-reference/supported-secret-stores/)
# such as HashiCorp Vault, Azure Key Vault, GPC Secret Manager, or AWS Secret Manager
k8s_yaml(local(
    ["kubectl", "create", "secret", "generic", "secret-store", 
    "--from-literal=pgadmin-password=bouncingcow",
    "--from-literal=pgadmin-emailuser=user@domain.com",
    "--from-literal=pg-host=auth-db-postgresql",
    "--from-literal=pg-connection-string=Host=auth-db-postgresql;Port=5432;Database=authdb;Username=postgres;Password=postgres;",
    "--from-literal=password=Y4nys7f11",
    "--from-literal=signing-key=NO5U308H!@#SI2ZXCVSDSDNDln",
    "--from-literal=jwt-issuer=http://localhost",
    "--from-literal=jwt-audience=http://localhost",
    "-o=yaml", "--dry-run=client"]
    ))

# Proxy for incoming traffic
include('./src/k8cher.proxy/Tiltfile')

# Auth Service
include('./src/k8cher.auth/Tiltfile')

# Auth Service postgresql database and tools
include('./src/k8cher.auth.db/Tiltfile')

# Service to sync Svelte Store database
include('./src/k8cher.store/Tiltfile')

# Dapr
helm_remote('dapr', release_name='dapr', repo_name='dapr', repo_url='https://dapr.github.io/helm-charts/')
k8s_resource('dapr-dashboard', port_forwards='8080:8080', labels=['dapr'])
k8s_resource('dapr-operator', labels=['dapr'])
k8s_resource('dapr-sentry', labels=['dapr'])
k8s_resource('dapr-placement-server', labels=['dapr'])
k8s_resource('dapr-sidecar-injector', labels=['dapr'])