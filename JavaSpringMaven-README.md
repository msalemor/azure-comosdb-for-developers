# Java 8, Spring Boot 4.0.0 and Maven 3.6 Docker Image Build

## Docker file

```yml
FROM maven:3.6-jdk-8 AS build  
COPY src /usr/src/app/src  
COPY pom.xml /usr/src/app  
RUN mvn -f /usr/src/app/pom.xml clean package

FROM openjdk:8-jdk-alpine
VOLUME /tmp  
COPY --from=build /usr/src/app/target/contosoCrmService-0.0.1-SNAPSHOT.jar contosoCrmService-0.0.1-SNAPSHOT.jar  
EXPOSE 8080  
ENTRYPOINT ["java","-jar","contosoCrmService-0.0.1-SNAPSHOT.jar"] 
```