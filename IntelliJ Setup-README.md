# Ubuntu 18.04 - IntelliJ Java Development Environement

## Download and Install Java JDK

```bash
sudo apt install default-jdk -y
```

Verify the JAVA_HOME and that is in the PATH

## Download and Install Maven

```bash
wget https://www-us.apache.org/dist/maven/maven-3/3.6.0/binaries/apache-maven-3.6.0-bin.tar.gz -P /tmp

sudo tar xf /tmp/apache-maven-*.tar.gz -C /opt

sudo ln -s /opt/apache-maven-3.6.0 /opt/maven
```

Verify the JAVA_HOME and that is in the PATH

```bash
export M2=/opt/maven
export MAVEN_HOME=/opt/maven
```

## Download and Install IntelliJ

### Install Spring Assistant Plug-in

Open the settings in IntelliJ

```
File\Settings or (CTRL+ALT+S)
Click Plugins
Search for and install Spring Assistant
Reboot the IDE
```

### Install Lombok Plug-in

Open the settings in IntelliJ

```
File\Settings or (CTRL+ALT+S)
Click Plugins
Search for and install Lombok
Reboot the IDE
```
