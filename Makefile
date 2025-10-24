# AI Landing Zone - Makefile for Docker Management

.PHONY: help dev prod stop logs clean rebuild status health

# Default target
help:
	@echo "🚀 AI Landing Zone - Docker Management"
	@echo "====================================="
	@echo ""
	@echo "Available commands:"
	@echo "  make dev      - Start in development mode (hot reload)"
	@echo "  make prod     - Start in production mode"
	@echo "  make stop     - Stop all services"
	@echo "  make logs     - Show logs from all services"
	@echo "  make clean    - Stop services and clean volumes"
	@echo "  make rebuild  - Rebuild and start in dev mode"
	@echo "  make status   - Show service status"
	@echo "  make health   - Check service health"
	@echo ""

# Development mode with hot reload
dev:
	@echo "🔥 Starting in development mode..."
	@./run-docker.sh

# Production mode
prod:
	@echo "🚀 Starting in production mode..."
	@./run-docker.sh --prod

# Stop all services
stop:
	@echo "🛑 Stopping all services..."
	@./stop-docker.sh

# Show logs
logs:
	@echo "📋 Showing logs..."
	@docker-compose -f docker-compose.dev.yml logs -f 2>/dev/null || docker-compose -f docker-compose.yml logs -f

# Clean everything
clean:
	@echo "🧹 Cleaning up..."
	@./stop-docker.sh --clean

# Rebuild and start
rebuild:
	@echo "🔨 Rebuilding and starting..."
	@./run-docker.sh --rebuild

# Check status
status:
	@echo "📊 Service Status:"
	@echo "=================="
	@docker-compose -f docker-compose.dev.yml ps 2>/dev/null || docker-compose -f docker-compose.yml ps || echo "No services running"

# Health check
health:
	@echo "🏥 Health Check:"
	@echo "==============="
	@echo -n "Backend:  "; curl -f http://localhost:5001/health > /dev/null 2>&1 && echo "✅ Healthy" || echo "❌ Unhealthy"
	@echo -n "Frontend: "; curl -f http://localhost:3000/health > /dev/null 2>&1 && echo "✅ Healthy" || echo "❌ Unhealthy"
	@echo -n "Redis:    "; docker exec lzai-redis-dev redis-cli ping > /dev/null 2>&1 && echo "✅ Healthy" || docker exec lzai-redis redis-cli ping > /dev/null 2>&1 && echo "✅ Healthy" || echo "❌ Unhealthy"

# Quick start aliases
start: dev
up: dev
down: stop