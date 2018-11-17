# Container File Share

Goal is to demonstrate how two containers can have a pub/sub using volumes

## Testing the reader with docker

```bash
docker run -v /Users/fbeltrao/dev/github.com/fbeltrao/Samples/ContainerFileShare/:/app/data fbeltrao/containerfileshare-reader:1.0
```

## Testing writer and reader with docker

```bash
docker run -v /Users/fbeltrao/dev/github.com/fbeltrao/Samples/ContainerFileShare/:/app/data fbeltrao/containerfileshare-writer:1.0

docker run -v /Users/fbeltrao/dev/github.com/fbeltrao/Samples/ContainerFileShare/:/app/data fbeltrao/containerfileshare-reader:1.0
```

## Test using IoT Edge

[todo]