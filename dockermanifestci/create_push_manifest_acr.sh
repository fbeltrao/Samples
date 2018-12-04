#! /bin/bash
docker manifest create frbeltra.azurecr.io/dockermanifestci:1.0 \
  frbeltra.azurecr.io/dockermanifestci:1.0-amd64 \
  frbeltra.azurecr.io/dockermanifestci:1.0-arm32v7
  
docker manifest push frbeltra.azurecr.io/dockermanifestci:1.0