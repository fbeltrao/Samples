# Multi architecture builds with Azure DevOps

When building docker images we target a specific architecture (amd64, windows-amd64, arm32v7). A common practice is to tag the architecture into your image:

* myapp:1.0-amd64 for linux
* myapp:1.0-amd64-windows for windows
* myapp:1.0-arm32v7 if you intend to run it on linux ARM (Raspberry PI for instance)?

Have you ever wonder why you can execute `docker run nginx` in an [Raspberry PI or Ubuntu](https://github.com/docker-library/official-images#architectures-other-than-amd64) and it will work?

## Docker Manifest

Docker manifest provides metadata about a list of image, pointing to the correct image based on the architecture. The docker cli offers a way to create and publish such metadata. This options is currently only available if you enabled the experimental features in the docker cli.

To enable them modify the `$HOME/.docker/config.json` adding the following property:

```json
{
    "experimental": "enabled"
}
```

## Building in Azure Dev Ops

Since building such manifests requires the usage of an experimental build, to use them requires a few workarounds when defining your pipeline. This [blog post](
https://www.axians-infoma.de/techblog/creating-a-multi-arch-docker-image-with-azure-devops/) explains how to do it. Basically we create a docker config file and use docker with it.

```yaml
steps:
  - bash: |
     mkdir -p ~/.docker
     echo '{ "experimental": "enabled" }' > ~/.docker/config.json
    displayName: Enabled docker experimental cli features
  
  - bash: |
     docker --config ~/.docker manifest create $(imageName) '$(imageName)-amd64' '$(imageName)-arm32v7' '$(imageName)-windows-amd64'
    displayName: Create manifest
  
  # docker_user_secret and docker_pwd_secret are variables
  # defined in the pipeline in Azure DevOps as secrets.
  # Need to explicitly use them
  - bash: |
     docker login -u $docker_user -p $docker_pwd
     docker --config ~/.docker manifest push $(imageName)
    env:
      docker_user: $(docker_user_secret)
      docker_pwd: $(docker_pwd_secret)
    displayName: Push manifest
```
