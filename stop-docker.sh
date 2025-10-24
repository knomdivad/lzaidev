#!/bin/bash

# AI Landing Zone - Stop Docker Services

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}🛑 Stopping AI Landing Zone Services${NC}"
echo -e "${BLUE}===================================${NC}"

# Stop development services
if docker-compose -f docker-compose.dev.yml ps -q > /dev/null 2>&1; then
    echo -e "${BLUE}Stopping development services...${NC}"
    docker-compose -f docker-compose.dev.yml down
fi

# Stop production services
if docker-compose -f docker-compose.yml ps -q > /dev/null 2>&1; then
    echo -e "${BLUE}Stopping production services...${NC}"
    docker-compose -f docker-compose.yml down
fi

# Clean up unused volumes if requested
if [ "$1" = "--clean" ]; then
    echo -e "${BLUE}Cleaning up unused volumes...${NC}"
    docker volume prune -f
fi

echo -e "${GREEN}✅ All services stopped${NC}"