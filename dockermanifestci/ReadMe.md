# Multi architecture builds with Azure DevOps

When building docker images we target a specific architecture (amd64, windows-amd64, arm32v7). A common practice is to tag the architecture into your image:

* myapp:1.0-amd64 for linux
* myapp:1.0-amd64-windows for windows
* myapp:1.0-arm32v7 if you intend to run it on PI?

Have you ever wonder why you can execute `docker run nginx` in an PI or Ubuntu and it will work?

## Docker Manifest

Docker manifest provides metadata about a docker image, pointing to the correct image based on the architecture. The docker cli offers a way to create and publish such metadata. This options is currently only available if you enabled the experimental features in the docker cli.

To enable them modify the `$HOME/.docker/config.json` adding the following property:

```json
{
    "experimental": "enabled"
}
```
