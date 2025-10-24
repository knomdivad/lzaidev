# 🚀 AI Landing Zone - Docker Setup

This guide explains how to run the complete AI Landing Zone platform using Docker and OrbStack for local development and testing.

## 📋 Prerequisites

- **Docker Desktop** or **OrbStack** installed and running
- **Make** (optional, for simplified commands)
- At least 4GB RAM available for containers
- Ports 3000, 5000, and 6379 available

## 🏗️ Architecture

The Docker setup includes:

- **Backend**: .NET 9.0 Web API (Port 5000)
- **Frontend**: React TypeScript app with nginx (Port 3000)
- **Redis**: Caching and session storage (Port 6379)
- **Network**: Custom bridge network for service communication

## 🚀 Quick Start

### Option 1: Using Make (Recommended)

```bash
# Start in development mode (with hot reload)
make dev

# Start in production mode
make prod

# Stop all services
make stop

# View logs
make logs

# Check service health
make health

# Clean everything
make clean
```

### Option 2: Using Scripts

```bash
# Start development environment
./run-docker.sh

# Start production environment
./run-docker.sh --prod

# Rebuild and start
./run-docker.sh --rebuild --logs

# Stop services
./stop-docker.sh
```

### Option 3: Using Docker Compose Directly

```bash
# Development mode
docker-compose -f docker-compose.dev.yml up -d

# Production mode
docker-compose -f docker-compose.yml up -d

# Stop
docker-compose -f docker-compose.dev.yml down
```

## 🌐 Access Points

Once running, access the application at:

| Service | URL | Description |
|---------|-----|-------------|
| **Frontend** | http://localhost:3000 | Customer portal UI |
| **Backend API** | http://localhost:5000 | REST API endpoints |
| **API Docs** | http://localhost:5000/swagger | Swagger documentation |
| **Redis** | localhost:6379 | Redis cache (internal) |

## 🔧 Development vs Production

### Development Mode (`make dev`)
- **Hot Reload**: Code changes auto-refresh
- **Source Volumes**: Live file mounting
- **Debug Logging**: Verbose output
- **Development Tools**: Includes dev dependencies

### Production Mode (`make prod`)
- **Optimized Builds**: Minified assets
- **Multi-stage Builds**: Smaller images
- **Health Checks**: Service monitoring
- **Security**: Non-root users

## 📊 Monitoring

### Check Service Status
```bash
make status
# or
docker-compose ps
```

### View Logs
```bash
make logs
# or
docker-compose logs -f
```

### Health Checks
```bash
make health
# or manually:
curl http://localhost:5000/health
curl http://localhost:3000/health
```

## 🔍 Troubleshooting

### Services Won't Start
1. Check Docker is running: `docker info`
2. Check port availability: `lsof -i :3000,5000,6379`
3. View service logs: `make logs`

### Frontend Can't Connect to Backend
1. Check backend health: `curl http://localhost:5000/health`
2. Verify network connectivity: `docker network ls`
3. Check API proxy in nginx.conf

### Build Failures
1. Clean and rebuild: `make clean && make rebuild`
2. Check Dockerfile syntax
3. Verify base image availability

### OrbStack Specific Issues
1. Restart OrbStack: `orb restart`
2. Check resource limits in OrbStack settings
3. Clear Docker cache: `docker system prune`

## 🗂️ File Structure

```
├── docker-compose.yml              # Production configuration
├── docker-compose.dev.yml          # Development configuration
├── run-docker.sh                   # Main startup script
├── stop-docker.sh                  # Stop script
├── Makefile                        # Simplified commands
├── src/LandingZoneAI.Portal/
│   ├── Dockerfile                  # Production backend
│   └── Dockerfile.dev             # Development backend
└── frontend/customer-portal/
    ├── Dockerfile                  # Production frontend
    ├── Dockerfile.dev             # Development frontend
    └── nginx.conf                 # Nginx configuration
```

## 🎯 Usage Examples

### Customer Workflow Testing
1. Start services: `make dev`
2. Open http://localhost:3000
3. Navigate through customer portal
4. Test AI chat interface
5. Configure cloud providers
6. Create landing zones

### API Testing
1. Open http://localhost:5000/swagger
2. Test customer endpoints
3. Test AI assistant endpoints
4. Test template management

### Development Workflow
1. Start dev mode: `make dev`
2. Edit source code
3. Changes auto-reload
4. Test in browser
5. Check logs: `make logs`

## 🧹 Cleanup

```bash
# Stop services only
make stop

# Stop and clean volumes
make clean

# Remove all Docker artifacts
docker system prune -a --volumes
```

## 📝 Notes

- Redis data persists in Docker volumes
- Development volumes mount source code
- Production builds are optimized for size
- Health checks ensure service reliability
- CORS is configured for local development

For issues or questions, check the logs first: `make logs`