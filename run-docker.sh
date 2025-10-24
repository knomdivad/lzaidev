#!/bin/bash

# AI Landing Zone - Docker Development Script
# This script builds and runs the complete stack using Docker Compose

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}🚀 AI Landing Zone - Development Environment${NC}"
echo -e "${BLUE}===========================================${NC}"

# Function to print colored messages
log_info() {
    echo -e "${BLUE}ℹ️  $1${NC}"
}

log_success() {
    echo -e "${GREEN}✅ $1${NC}"
}

log_warning() {
    echo -e "${YELLOW}⚠️  $1${NC}"
}

log_error() {
    echo -e "${RED}❌ $1${NC}"
}

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    log_error "Docker is not running. Please start Docker Desktop or OrbStack."
    exit 1
fi

log_success "Docker is running"

# Parse command line arguments
MODE="dev"
REBUILD=false
LOGS=false

while [[ $# -gt 0 ]]; do
    case $1 in
        --prod|--production)
            MODE="prod"
            shift
            ;;
        --rebuild)
            REBUILD=true
            shift
            ;;
        --logs)
            LOGS=true
            shift
            ;;
        --help|-h)
            echo "Usage: $0 [OPTIONS]"
            echo ""
            echo "Options:"
            echo "  --prod, --production    Run in production mode"
            echo "  --rebuild              Force rebuild of Docker images"
            echo "  --logs                 Show logs after starting"
            echo "  --help, -h             Show this help message"
            echo ""
            echo "Examples:"
            echo "  $0                     # Start in development mode"
            echo "  $0 --prod             # Start in production mode"
            echo "  $0 --rebuild --logs   # Rebuild and show logs"
            exit 0
            ;;
        *)
            log_error "Unknown option: $1"
            echo "Use --help for usage information"
            exit 1
            ;;
    esac
done

# Set compose file based on mode
if [ "$MODE" = "prod" ]; then
    COMPOSE_FILE="docker-compose.yml"
    log_info "Running in PRODUCTION mode"
else
    COMPOSE_FILE="docker-compose.dev.yml"
    log_info "Running in DEVELOPMENT mode with hot reload"
fi

# Stop any running containers
log_info "Stopping any existing containers..."
docker-compose -f "$COMPOSE_FILE" down 2>/dev/null || true

# Rebuild if requested
if [ "$REBUILD" = true ]; then
    log_info "Rebuilding Docker images..."
    docker-compose -f "$COMPOSE_FILE" build --no-cache
fi

# Start the services
log_info "Starting services..."
if [ "$MODE" = "dev" ]; then
    docker-compose -f "$COMPOSE_FILE" up -d
else
    docker-compose -f "$COMPOSE_FILE" up -d
fi

# Wait for services to be healthy
log_info "Waiting for services to start..."
sleep 10

# Check service health
if [ "$MODE" = "prod" ]; then
    # Check backend health
    for i in {1..30}; do
        if curl -f http://localhost:5000/health > /dev/null 2>&1; then
            log_success "Backend is healthy"
            break
        fi
        if [ $i -eq 30 ]; then
            log_error "Backend failed to start"
            docker-compose -f "$COMPOSE_FILE" logs backend
            exit 1
        fi
        sleep 2
    done

    # Check frontend health
    for i in {1..30}; do
        if curl -f http://localhost:3000/health > /dev/null 2>&1; then
            log_success "Frontend is healthy"
            break
        fi
        if [ $i -eq 30 ]; then
            log_error "Frontend failed to start"
            docker-compose -f "$COMPOSE_FILE" logs frontend
            exit 1
        fi
        sleep 2
    done
fi

log_success "All services are running!"

echo ""
echo -e "${GREEN}🎉 AI Landing Zone is ready!${NC}"
echo ""
echo -e "${BLUE}📱 Frontend:${NC} http://localhost:3000"
echo -e "${BLUE}🔧 Backend API:${NC} http://localhost:5000"
echo -e "${BLUE}📚 API Documentation:${NC} http://localhost:5000/swagger"
echo -e "${BLUE}🗄️  Redis:${NC} localhost:6379"
echo ""

if [ "$MODE" = "dev" ]; then
    echo -e "${YELLOW}🔥 Development mode: Changes will auto-reload${NC}"
fi

echo ""
echo -e "${BLUE}Useful commands:${NC}"
echo "  docker-compose -f $COMPOSE_FILE logs -f     # View logs"
echo "  docker-compose -f $COMPOSE_FILE down        # Stop services"
echo "  docker-compose -f $COMPOSE_FILE ps          # Check status"
echo ""

# Show logs if requested
if [ "$LOGS" = true ]; then
    log_info "Showing logs (Press Ctrl+C to exit)..."
    docker-compose -f "$COMPOSE_FILE" logs -f
fi