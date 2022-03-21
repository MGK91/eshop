pipeline {
  agent {
    kubernetes {
      label 'eshop-demo'
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
    - name: m2
      persistentVolumeClaim:
        claimName: m2
"""
}
   }
    stage('Push') {
      steps {
        container('docker') {
          sh """
             docker build -t eshop-demo:$BUILD_NUMBER .
             curl "https://awscli.amazonaws.com/awscli-exe-linux-x86_64.zip" -o "awscliv2.zip"
             unzip awscliv2.zip
             sudo ./aws/install
             aws ecr get-login
             docker tag eshop-demo:$BUILD_NUMBER 831089310150.dkr.ecr.us-east-2.amazonaws.com/microservice-win:$BUILD_NUMBER
             docker push 831089310150.dkr.ecr.us-east-2.amazonaws.com/microservice-win:$BUILD_NUMBER
          """
        }
      }
    }
}
