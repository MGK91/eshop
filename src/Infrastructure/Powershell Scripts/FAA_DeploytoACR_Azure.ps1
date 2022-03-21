#--------------------------------------------------------------------------------------------#
#1.Description:
# The following script deploys a local docker container onto Azure Container Registry
# The App deployed to Azure will pull the latest image onto Azure web app.

#2.Date:09/23/2021

#3.Important:
# Please run the following commands as a local administrator.
#--------------------------------------------------------------------------------------------#

#Define the Azure resource group and location
$ResourceGroup = "FAA"
$Location      = "eastus"

#Login to Azure
az login

#Optional: Use the following command to install Azure CLI if not installed on your machine.
#Invoke-WebRequest -Uri https://aka.ms/installazurecliwindows -OutFile .\AzureCLI.msi; Start-Process msiexec.exe -Wait -ArgumentList '/I AzureCLI.msi /quiet'; rm .\AzureCLI.msi


#--------------------------------- Docker Imgage build, tag and Push to ACR----------------#
#Navigate to right folder with the docker or user -f paramter to specify path to docker file.
cd C:\Users\susheel.dakoju_karsu\source\repos\service-mesh-prototype\eShopOnWeb-master\eShopOnWeb-master\src\Web

#Build an image using docker file.
docker build -t faademoapp:v1 .
docker images

#Login to Azure Container Registry
az acr login --name faademoapp.azurecr.io  -u "faademoapp" -p "cw7dNKO2YExELuo+5OWeCBSWZJOT/pUl"

#Optional: The follow need to run only once if it is not admin enabled
az acr update -n ltupgrade.azurecr.io --admin-enabled true

az acr list --resource-group $ResourceGroup --query "[].{acrLoginServer:loginServer}" --output table

#To be able to push Docker images to Azure Container Registry, they need to be tagged with the loginServer name of the Registry. 
#These tags are used for routing purposes when Docker images are pushed to Azure.
docker tag eshopwebmvc:dev faademoapp.azurecr.io/eshopwebmvc:dev

docker push faademoapp.azurecr.io/eshopwebmvc:dev
#--------------------------------- Docker Imgage build, tag and Push to ACR----------------#