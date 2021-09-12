# allow for remote helm charts
load('ext://helm_remote', 'helm_remote')

# don't hide secrets for development only environment
secret_settings ( disable_scrub=True ) 

# Create a local kubernetes secret store. This is only for local development and not secure!!!
# For production implement a a dapr secret store component (https://docs.dapr.io/reference/components-reference/supported-secret-stores/)
# such as HashiCorp Vault, Azure Key Vault, GPC Secret Manager, or AWS Secret Manager
k8s_yaml(local(
    ["kubectl", "create", "secret", "generic", "secret-store", 
    "--from-literal=pgadmin-password=bouncingcow",
    "--from-literal=pgadmin-emailuser=user@domain.com",
    "--from-literal=pg-host=k8cher-db-postgresql",
    "--from-literal=pg-connection-string=Host=k8cher-db-postgresql;Port=5432;Database=k8cher;Username=postgres;Password=postgres;",
    "--from-literal=password=Y4nys7f11",
    "--from-literal=signing-key=NO5U308H!@#SI2ZXCVSDSDNDln",
    "--from-literal=jwt-issuer=http://localhost:8088",
    "--from-literal=jwt-audience=http://localhost:8088",
    "--from-literal=ingress=http://localhost:8088/",
    "--from-literal=smtp-user=mike",
    "--from-literal=smtp-password=Password_",
    "--from-literal=mail-server-email-from=donotreply@domain.com",
    "--from-literal=web-login-redirect=http://localhost:3000/login",
    "--from-literal=web-confirmation-expired=http://localhost:3000/tokenexpired",
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

# maildev development mail server
include('./src/maildev/Tiltfile')


# Dapr
helm_remote('dapr', release_name='dapr', repo_name='dapr', repo_url='https://dapr.github.io/helm-charts/')
k8s_resource('dapr-dashboard', port_forwards=[port_forward(8080, 8080, name='dapr dashboard')], labels=['dapr'])
k8s_resource('dapr-operator', labels=['dapr'])
k8s_resource('dapr-sentry', labels=['dapr'])
k8s_resource('dapr-placement-server', labels=['dapr'])
k8s_resource('dapr-sidecar-injector', labels=['dapr'])

# Dapr Components
k8s_yaml('./daprComponents/pg-store.yaml')
k8s_yaml('./daprComponents/email.yaml')