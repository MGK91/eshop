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
    stage('Build') {
      steps {
        script {
		   dockerImage = docker.build registry + ":$BUILD_NUMBER"
		   }
        }
      }
    }
    stage('Push') {
      steps {
        script {
           docker.withRegistry("https://" + registry, "ecr.us-east-2" + registryCredential) {
                 dockerImage.push()
           }
		}
    }
   }
  }
}
