* slim down cors (have it wide open now)
* add sveltekit to tilt up
* With minimal APIs can I flip it where it always requires authorization unless removed? Right now add .RequireAuthorization() at the end.


* Consider adding issue to Dapr asking for advice where a container restarts and the side car is not injected due to: TLS handshake error from remote error: tls: bad certificate. This only happens if DevelopmentDockerfile or helm chart is changed.
To fix, have to restart dapr side car injector pod then restart container pod.
Similar issue mentioned here https://github.com/dapr/dapr/issues/1621, but closed due to workaround. Maybe there is a way in tilt to auto restart for now? Maybe similar, but when actors added, have to tilt down, tilt up. Dapr environment just not very hot reloadable friendly?

* Add secrets through Dapr? Right now it is working because they are all hardcoded in appsettings.json. This would allow integrating with key vault.
Was able to access loaded secrets from dapr with:
```
app.MapGet("auth/test", async(DaprClient daprClient) => {
    var secrets = await daprClient.GetSecretAsync("kubernetes", "secret-store");
            Console.WriteLine($"length: {secrets.Count}");
            var key = secrets.Keys.First();
            Console.WriteLine($"key: {key}");

            var value = secrets.Values.First();
            Console.WriteLine($"value: {value}");
});
```

* setup authorization policy in reverse proxy
* Hookup CI/CD (argo? tilt itself? Azure DevOps? Github Actions?) deploying to prod like environment (use Dockerfile, fix tagging, tie into vault, etc.)
* add health checks
* Tests (tie into tilt buttons, also integration tests)
* implement e-mail for dev environment. This repo used image named maildev: https://github.com/EdwinVW/dapr-traffic-control/blob/master/src/k8s/maildev.yaml
* add swagger for store service
* Get full login working - Experiment with login flow (brainstorm nice flow)
    * Can potentially have user work and then create account only to store data
    * Potentially not call register until e-mail confirmed link,
        * Flow would be click create account, enter e-mail, go to link, click verification w/ password creates account (don't have to have password if user verifies account each time)
* Instead of using 'ingress controller' can Microsoft reverse proxy be leveraged to autodetect dapr and other services and auto map?
* todo - mbk: add an 'addDapr' flag to helm deployment.yaml chart that only adds when true

* Figure out why 'kind' cluster does not have port:80 work on localhost. Prefer using it as local dev cluster and want to get integration test running because much faster to spin up new database, run tests, destroy. I thought the following would get it working:
// in deployment of proxy, then setting nodeSelector in kind script
nodeSelector:
  ingress-ready: 'true'


  * Should Database Connnection string include namespace allowing it to be placed in different namespace? my-app.default.svc.cluster.local
* Dapr dashboard will not be able to viewed when deployed to cloud. Added Dapr dashboard to proxy at /dapr-dashbard and authorized and tried a PathRemovePrefix. Problem is loading the other assets (js files it requires, etc. ) don't have '/dapr-dashboard' in route, so don't get rerouted by proxy. Would need to change base path of all assets.
* Docs said not to gitignore tilt_modules folder, but seems messy. Believe this is the 'versioning' to ensure users leveraging same modules and does not break.


* links on minimal API to check against how I setup
    * has auth helper - https://dev.to/425show/secure-and-minimal-apis-using-net-6-c-10-and-azure-active-directory-197i
    * really good article comparing to mvc, review: https://benfoster.io/blog/mvc-to-minimal-apis-aspnet-6/
    * authn/authz - https://anthonygiretti.com/2021/08/12/asp-net-core-6-working-with-minimal-apis/
    * decorates functions .Produces(StatusCodes.Status204NoContent) - https://github.com/DamianEdwards/MinimalApiPlayground/blob/main/src/Todo.EFCore/Program.cs
