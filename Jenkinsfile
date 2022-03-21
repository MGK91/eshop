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
             docker tag eshop-demo:$BUILD_NUMBER "831089310150.dkr.ecr.us-east-2.amazonaws.com/microservice:${env.BUILD_NUMBER}"
             export AWS_ACCESS_KEY_ID=${env.AWS_ACCESS_KEY_ID}
             export AWS_SECRET_ACCESS_KEY=${env.AWS_SECRET_ACCESS_KEY}
             docker push "831089310150.dkr.ecr.us-east-2.amazonaws.com/microservice:${env.BUILD_NUMBER}"
             curl -LO https://storage.googleapis.com/kubernetes-release/release/v1.18.0/bin/linux/amd64/kubectl
             mv ./kubectl /usr/local/bin/kubectl
             chmod +x /usr/local/bin/kubectl
             /usr/bin/aws eks update-kubeconfig --region ${env.AWS_DEFAULT_REGION} --name ${env.CLUSTER}
             /usr/local/bin/kubectl set image deployment/eshpwebmvc-deployment eshopwebmvc="831089310150.dkr.ecr.us-east-2.amazonaws.com/microservice:${env.BUILD_NUMBER}" --record
          """
        }
      }
    }
  }
}
