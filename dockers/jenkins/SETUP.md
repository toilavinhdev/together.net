# Local
scp -r ../jenkins/* root@157.245.53.60:~/togethernet/jenkins
# Server
mkdir -p ~/togethernet/jenkins
cd ~/togethernet/jenkins
docker build -t toilavinhdev-jenkins:jcasc .
docker run \
    -dp 8080:8080 \
    --name prod-togethernet-jenkins \
    --restart unless-stopped \
    --env JENKINS_ADMIN_ID=admin \
    --env JENKINS_ADMIN_PASSWORD=password
    toilavinhdev-jenkins:jcasc