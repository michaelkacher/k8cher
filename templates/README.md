# Steps
1) Navigate to the correct directory `cd k8cher\templates`
2) Install the template `dotnet new -i .\K8cherService\`
3) Create a new service named MyService `dotnet new K8cherService -n MyService`

This scaffolds out the files with the proper service name including dockerfile and tiltfile.