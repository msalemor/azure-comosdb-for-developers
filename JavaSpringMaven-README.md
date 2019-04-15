# Java 8, Spring Boot 4.0.0 and Maven 3.6 Docker Image Build

## Create the image

```bash
# Dockerfile
# Use the maven inmage and tag it as build
FROM maven:3.6-jdk-8 AS build  
# Copy the source code
COPY src /usr/src/app/src  
# Copy the pom.XML file
COPY pom.xml /usr/src/app  
# Run Maven
# This will generate a file on the target folder based on the Application Identifier
RUN mvn -f /usr/src/app/pom.xml clean package

# Use the alpine Java 8 image
FROM openjdk:8-jdk-alpine
# Set the temp volume
VOLUME /tmp  
# copy the output from the maven image to the alpine image
COPY --from=build /usr/src/app/target/contosoCrmService-0.0.1-SNAPSHOT.jar contosoCrmService-0.0.1-SNAPSHOT.jar  
#expose port 8080
EXPOSE 8080  
# Set the entrypoint
ENTRYPOINT ["java","-jar","contosoCrmService-0.0.1-SNAPSHOT.jar"]  
```

## Build the image

```bash
docker build -t contosocrmservice:dev .
```

## Run the image

```bash
docker run --rm -p 8080:8080 contosocrmservice:dev
```