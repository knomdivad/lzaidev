import React, { useEffect, useState } from 'react';
import {
  Container,
  Paper,
  Typography,
  Box,
  Card,
  CardContent,
  LinearProgress,
  Chip,
  List,
  ListItem,
  ListItemText,
  ListItemIcon,
  Button,
} from '@mui/material';
import {
  TrendingUp,
  Cloud,
  AttachMoney,
  PlayArrow,
  CheckCircle,
  Error,
  Warning,
} from '@mui/icons-material';

interface DashboardStats {
  totalLandingZones: number;
  activeLandingZones: number;
  monthlyCost: number;
  pendingDeployments: number;
}

interface LandingZone {
  id: string;
  name: string;
  cloudProvider: string;
  status: 'deployed' | 'deploying' | 'failed' | 'pending';
  createdAt: string;
  lastModified: string;
  cost: number;
}

const Dashboard: React.FC = () => {
  const [stats, setStats] = useState<DashboardStats>({
    totalLandingZones: 0,
    activeLandingZones: 0,
    monthlyCost: 0,
    pendingDeployments: 0,
  });

  const [recentLandingZones, setRecentLandingZones] = useState<LandingZone[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    // TODO: Replace with actual API calls
    fetchDashboardData();
  }, []);

  const fetchDashboardData = async () => {
    try {
      // Mock data - replace with actual API calls
      setStats({
        totalLandingZones: 12,
        activeLandingZones: 10,
        monthlyCost: 2450.75,
        pendingDeployments: 2,
      });

      setRecentLandingZones([
        {
          id: '1',
          name: 'Production Web App',
          cloudProvider: 'Azure',
          status: 'deployed',
          createdAt: '2024-01-15',
          lastModified: '2024-01-20',
          cost: 890.50,
        },
        {
          id: '2',
          name: 'Development Environment',
          cloudProvider: 'AWS',
          status: 'deploying',
          createdAt: '2024-01-18',
          lastModified: '2024-01-21',
          cost: 245.25,
        },
        {
          id: '3',
          name: 'Analytics Platform',
          cloudProvider: 'GCP',
          status: 'deployed',
          createdAt: '2024-01-10',
          lastModified: '2024-01-19',
          cost: 1315.00,
        },
      ]);
    } catch (error) {
      console.error('Error fetching dashboard data:', error);
    } finally {
      setLoading(false);
    }
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'deployed':
        return 'success';
      case 'deploying':
        return 'warning';
      case 'failed':
        return 'error';
      case 'pending':
        return 'info';
      default:
        return 'default';
    }
  };

  const getStatusIcon = (status: string) => {
    switch (status) {
      case 'deployed':
        return <CheckCircle color="success" />;
      case 'deploying':
        return <PlayArrow color="warning" />;
      case 'failed':
        return <Error color="error" />;
      case 'pending':
        return <Warning color="info" />;
      default:
        return <Warning />;
    }
  };

  if (loading) {
    return (
      <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
        <Box display="flex" justifyContent="center" alignItems="center" minHeight="200px">
          <LinearProgress sx={{ width: '50%' }} />
        </Box>
      </Container>
    );
  }

  return (
    <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
      <Typography variant="h4" component="h1" gutterBottom>
        Dashboard
      </Typography>
      <Typography variant="body1" color="text.secondary" gutterBottom>
        Welcome to your AI Landing Zone portal. Monitor your infrastructure, track deployments, and manage costs.
      </Typography>

      {/* Stats Cards */}
      <Box display="flex" flexWrap="wrap" gap={3} sx={{ mt: 3, mb: 4 }}>
        {/* Total Landing Zones */}
        <Card sx={{ minWidth: 250, flex: '1 1 250px' }}>
          <CardContent>
            <Box display="flex" alignItems="center" justifyContent="space-between">
              <Box>
                <Typography color="text.secondary" gutterBottom>
                  Total Landing Zones
                </Typography>
                <Typography variant="h4" component="div">
                  {stats.totalLandingZones}
                </Typography>
              </Box>
              <TrendingUp color="primary" sx={{ fontSize: 40 }} />
            </Box>
          </CardContent>
        </Card>

        {/* Active Landing Zones */}
        <Card sx={{ minWidth: 250, flex: '1 1 250px' }}>
          <CardContent>
            <Box display="flex" alignItems="center" justifyContent="space-between">
              <Box>
                <Typography color="text.secondary" gutterBottom>
                  Active Landing Zones
                </Typography>
                <Typography variant="h4" component="div">
                  {stats.activeLandingZones}
                </Typography>
              </Box>
              <CheckCircle color="success" sx={{ fontSize: 40 }} />
            </Box>
          </CardContent>
        </Card>

        {/* Monthly Cost */}
        <Card sx={{ minWidth: 250, flex: '1 1 250px' }}>
          <CardContent>
            <Box display="flex" alignItems="center" justifyContent="space-between">
              <Box>
                <Typography color="text.secondary" gutterBottom>
                  Monthly Cost
                </Typography>
                <Typography variant="h4" component="div">
                  ${stats.monthlyCost.toFixed(2)}
                </Typography>
              </Box>
              <AttachMoney color="warning" sx={{ fontSize: 40 }} />
            </Box>
          </CardContent>
        </Card>

        {/* Pending Deployments */}
        <Card sx={{ minWidth: 250, flex: '1 1 250px' }}>
          <CardContent>
            <Box display="flex" alignItems="center" justifyContent="space-between">
              <Box>
                <Typography color="text.secondary" gutterBottom>
                  Pending Deployments
                </Typography>
                <Typography variant="h4" component="div">
                  {stats.pendingDeployments}
                </Typography>
              </Box>
              <PlayArrow color="info" sx={{ fontSize: 40 }} />
            </Box>
          </CardContent>
        </Card>
      </Box>

      {/* Recent Landing Zones */}
      <Box display="flex" flexWrap="wrap" gap={3}>
        {/* Landing Zones List */}
        <Paper sx={{ flex: '2 1 400px', p: 3 }}>
          <Box display="flex" justifyContent="space-between" alignItems="center" mb={2}>
            <Typography variant="h6" component="h2">
              Recent Landing Zones
            </Typography>
            <Button variant="outlined" size="small">
              View All
            </Button>
          </Box>
          <List>
            {recentLandingZones.map((landingZone) => (
              <ListItem key={landingZone.id} divider>
                <ListItemIcon>
                  {getStatusIcon(landingZone.status)}
                </ListItemIcon>
                <ListItemText
                  primary={landingZone.name}
                  secondary={
                    <Box>
                      <Typography variant="body2" color="text.secondary">
                        {landingZone.cloudProvider} • Last modified: {landingZone.lastModified}
                      </Typography>
                      <Box display="flex" alignItems="center" gap={1} mt={1}>
                        <Chip
                          label={landingZone.status}
                          size="small"
                          color={getStatusColor(landingZone.status) as any}
                        />
                        <Typography variant="body2" color="text.secondary">
                          ${landingZone.cost.toFixed(2)}/month
                        </Typography>
                      </Box>
                    </Box>
                  }
                />
              </ListItem>
            ))}
          </List>
        </Paper>

        {/* Quick Actions */}
        <Paper sx={{ flex: '1 1 300px', p: 3 }}>
          <Typography variant="h6" component="h2" gutterBottom>
            Quick Actions
          </Typography>
          <Box display="flex" flexDirection="column" gap={2}>
            <Button variant="contained" color="primary" fullWidth>
              Create New Landing Zone
            </Button>
            <Button variant="outlined" fullWidth>
              Chat with AI Assistant
            </Button>
            <Button variant="outlined" fullWidth>
              View Templates
            </Button>
            <Button variant="outlined" fullWidth>
              Generate Cost Report
            </Button>
          </Box>
        </Paper>
      </Box>
    </Container>
  );
};

export default Dashboard;