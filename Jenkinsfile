pipeline {
  environment {
     registry = '831089310150.dkr.ecr.us-east-2.amazonaws.com/microservice'
     registryCredential = 'jenkins-ecr'
     dockerImage = ''
  }
  agent {
    kubernetes {
      defaultContainer 'jnlp'
      yaml """
apiVersion: v1
kind: Pod
metadata:
labels:
  component: ci
spec:
  # Use service account that can deploy to all namespaces
  serviceAccountName: jenkins
  containers:
  - name: docker
    image: docker:latest
    command:
    - cat
    tty: true
    volumeMounts:
    - mountPath: /var/run/docker.sock
      name: docker-sock
  volumes:
    - name: docker-sock
      hostPath:
        path: /var/run/docker.sock
"""
}
  }
  stages {
    stage('Push') {
      steps {
        container('docker') {
          sh """
             apk update
             apk add curl unzip aws-cli
             /usr/bin/aws ecr get-login-password --region us-east-2 | docker login --username AWS --password-stdin 831089310150.dkr.ecr.us-east-2.amazonaws.com
             docker build -t eshop-demo:$BUILD_NUMBER .
             docker tag eshop-demo:$BUILD_NUMBER "831089310150.dkr.ecr.us-east-2.amazonaws.com/microservice:$BUILD_NUMBER"
             #docker push 831089310150.dkr.ecr.us-east-2.amazonaws.com/microservice:$BUILD_NUMBER         
          """
        }
      }
    }
    stage('Deploy') {
      steps {
        container('docker') {
          sh """
             docker.withRegistry($registry, $registryCredential) {
                docker.push("${env.BUILD_NUMBER}")
          """
            }
        }
      }
    }
  }
