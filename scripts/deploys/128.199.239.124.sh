#!/bin/sh
# chmod +x ./scripts/deploys/128.199.239.124.sh
# ./scripts/deploys/128.199.239.124.sh

ip_address=$(hostname -I | awk '{print $1}')

# Dashboard
echo "[Together.NET $ip_address]"
echo "Select containers to build:"
echo "(0) ALL CONTAINER"
echo "(1) tnprod-api-gateway"
echo "(2) tnprod-web-client"
echo "(3) tnprod-service-identity"
echo "(4) tnprod-service-community"
echo "(5) tnprod-service-chat"
echo "(6) tnprod-service-notification"
echo "(7) tnprod-service-socket"
read -p "Enter selection: " -a choices

containers=()

# Handle
for choice in "${choices[@]}"; do
  case $choice in
    0)
      containers=("tnprod-api-gateway" "tnprod-web-client" "tnprod-service-identity" "tnprod-service-community" "tnprod-service-chat" "tnprod-service-notification" "tnprod-service-socket")
        break
        ;;
    1)
      containers+=("tnprod-api-gateway")
      ;;
    2)
      containers+=("tnprod-web-client")
      ;;
    3)
      containers+=("tnprod-service-identity")
      ;;
    4)
      containers+=("tnprod-service-community")
      ;;
    5)
      containers+=("tnprod-service-chat")
      ;;
    6)
      containers+=("tnprod-service-notification")
      ;;
    7)
      containers+=("tnprod-service-socket")
      ;;
    *)
      echo "Error: Invalid selection!"
      ;;
    esac
done

# Check
if [ ${#containers[@]} -eq 0 ]; then
    echo "No container are selected. Exit."
    exit 1
fi

# Convert to string
container_strings="${containers[@]}"

# Execute
echo "Execute: docker compose -f dockers/docker-compose.cluster-1.yml up -d --force-recreate --no-deps --build $container_strings"
docker compose -f dockers/docker-compose.cluster-1.yml up -d --force-recreate --no-deps --build "$container_strings"