#!/bin/bash

# Test script for Landing Zone AI Portal API
BASE_URL="http://localhost:5000/api"

echo "🧪 Testing Landing Zone AI Portal API"
echo "🔗 Base URL: $BASE_URL"
echo ""

# Test health endpoint
echo "1️⃣ Testing Health Endpoint..."
curl -s "$BASE_URL/../health" | jq '.' || echo "Health endpoint response"
echo ""

# Test resource groups
echo "2️⃣ Testing Resource Groups..."
curl -s "$BASE_URL/resources/subscriptions/mock-subscription-123/resource-groups" | jq '.' || echo "Resource groups response"
echo ""

# Test resources
echo "3️⃣ Testing Resources List..."
curl -s "$BASE_URL/resources/subscriptions/mock-subscription-123/resources" | jq '.[0:2]' || echo "Resources response (first 2 items)"
echo ""

# Test ML Workspace
echo "4️⃣ Testing ML Workspace Info..."
curl -s "$BASE_URL/resources/subscriptions/mock-subscription-123/resource-groups/rg-lzai-dev/ml-workspaces/mlw-lzai-dev" | jq '.' || echo "ML Workspace response"
echo ""

# Test OpenAI Service
echo "5️⃣ Testing OpenAI Service Info..."
curl -s "$BASE_URL/resources/subscriptions/mock-subscription-123/resource-groups/rg-lzai-dev/openai-services/cog-lzai-dev" | jq '.' || echo "OpenAI Service response"
echo ""

# Test AKS Cluster
echo "6️⃣ Testing AKS Cluster Info..."
curl -s "$BASE_URL/resources/subscriptions/mock-subscription-123/resource-groups/rg-lzai-dev/aks-clusters/aks-lzai-dev" | jq '.' || echo "AKS Cluster response"
echo ""

# Test Cost Data
echo "7️⃣ Testing Cost Data..."
curl -s "$BASE_URL/cost/resources/mock-subscription-123" | jq '.[0:2]' || echo "Cost data response (first 2 items)"
echo ""

# Test Cost Summary
echo "8️⃣ Testing Cost Summary..."
curl -s "$BASE_URL/cost/summary/mock-subscription-123" | jq '.' || echo "Cost summary response"
echo ""

# Test Monitoring
echo "9️⃣ Testing Monitoring Health Status..."
curl -s "$BASE_URL/monitoring/health-status/mock-subscription-123/rg-lzai-dev" | jq '.' || echo "Health status response"
echo ""

# Test Deployment Template
echo "🔟 Testing Deployment Template..."
curl -s "$BASE_URL/deployment/template" | jq '.' || echo "Deployment template response"
echo ""

echo "✅ API Testing Complete!"
echo ""
echo "💡 Next Steps:"
echo "   • Open http://localhost:5000/swagger to explore the API interactively"
echo "   • All data is mocked - perfect for testing without Azure resources"
echo "   • Try the deployment endpoints to simulate infrastructure deployment"