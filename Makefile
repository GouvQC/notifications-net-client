.DEFAULT_GOAL := help

.PHONY: help
help:
	@cat $(MAKEFILE_LIST) | grep -E '^[a-zA-Z_-]+:.*?## .*$$' | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-30s\033[0m %s\n", $$1, $$2}'

.PHONY: build
build: ## Construire le projet
	dotnet build -f net6.0

.PHONY: format
format:
	dotnet format

.PHONY: integration-test
integration-test: test=TestCategory=Integration ## Exécuter les tests d'intégration
integration-test: single-test

.PHONY: test
test: test=TestCategory=Unit
test: single-test

.PHONY: single-test
single-test: build ## Exécuter un test unique. Utilisation : "make single-test test=[nom du test]"
	dotnet test ./src/PgnNotifications.Client.Tests/PgnNotifications.Client.Tests.csproj -f net6.0 --no-build -v n --filter $(test)

.PHONY: build-release
build-release: ## Construire la version de production
	dotnet build -c Release -f net6.0

.PHONY: build-package
build-package: build-release ## Construire et empaqueter le NuGet
	dotnet pack -c Release ./src/PgnNotifications.Client/PgnNotifications.Client.csproj /p:TargetFrameworks=net6 -o publish

.PHONY: bootstrap-with-docker
bootstrap-with-docker:  ## Préparer l'image Docker de construction
	docker build -t notifications-net-client .

.PHONY: build-with-docker
build-with-docker: ## Construire avec Docker
	docker build -t notifications-net-client -f Dockerfile .

.PHONY: test-with-docker
test-with-docker: build-with-docker ## Tester avec Docker
	./scripts/run_with_docker.sh make test

.PHONY: integration-test-with-docker
integration-test-with-docker: build-with-docker ## Test d'intégration avec Docker
	./scripts/run_with_docker.sh make integration-test

.PHONY: format-with-docker
format-with-docker: ## Exécuter dotnet format pour formater le code
	./scripts/run_with_docker.sh make format
