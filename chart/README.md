## DataCatalog-backend Chart

### Usage
This chart comes unconfigured and will need to be configured with the following values to work.

Undefined values:
* ```secretName```
* ```apiDeployment.image.repository```
* ```drmDeployment.image.repository```
* ```ingress.host```
* ```job.image.repository```

### Values


| Parameter | Description | Default |
|-----------|-------------|---------|
| `secretName` | Name of a Hashicorp Vault secret | nil
| `environment` | ASPNETCORE_ENVIRONMENT | test
| `appName` | The overall name | datacatalog-backend
| `tag` | The tag of the API and migrator images (assumed to have same tag) | latest
| `apiDeployment.replicas` | Number of nodes for the Api | 1
| `apiDeployment.image.repository` | The repository of the API image | nil
| `drmDeployment.replicas` | Number of nodes for the DataResourceManagement service | 1
| `drmDeployment.image.repository` | The repository of the DataResourceManagement image | nil
| `service.type` | The type of service | ClusterIP
| `ingress.enabled` | Enables ingress | true
| `ingress.host` | Ingress accepted host | []
| `ingress.path` | Ingress accepted path | /
| `ingress.annotations` | Ingress annotations | ```nginx.ingress.kubernetes.io/rewrite-target: /$2, cert-manager.io/cluster-issuer: letsencrypt-prod, kubernetes.io/ingress.class: nginx```
| `job.image.repository` | The repository of the Migrator image | nil