PROJECT_NAME=p2p-shopping
PROJECT_CONTAINER_NAME_PREFIX=p2p-shopping

.PHONY: up down sqlup sqldown 

default: up

help : Makefile
	@sed -n 's/^##//p' $<

up:
	@echo "Starting up containers for for $(PROJECT_NAME)"
	docker-compose -f makefile_compose.yml up --build -d --remove-orphans

sqlup: 
	@echo "Starting up containers for for $(PROJECT_NAME)"
	docker-compose -f makefile_compose_sql.yml up --build -d --remove-orphans

down:
	@echo "Stopping containers for $(PROJECT_NAME)..."
	@docker-compose -f makefile_compose.yml stop

sqldown:
	@echo "Stopping containers for $(PROJECT_NAME)..."
	@docker-compose -f makefile_compose_sql.yml stop

%:
	@:
