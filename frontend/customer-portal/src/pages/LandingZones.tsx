import React, { useState, useEffect } from 'react';
import {
  Container,
  Typography,
  Box,
  Card,
  CardContent,
  Button,
  Chip,
  IconButton,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  LinearProgress,
  Fab,
} from '@mui/material';
import {
  Add as AddIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
  Visibility as ViewIcon,
  PlayArrow as DeployIcon,
  Stop as StopIcon,
  Refresh as RefreshIcon,
} from '@mui/icons-material';

interface LandingZone {
  id: string;
  name: string;
  description: string;
  cloudProvider: 'Azure' | 'AWS' | 'GCP';
  status: 'draft' | 'deploying' | 'deployed' | 'failed' | 'stopped';
  template: string;
  createdAt: string;
  lastModified: string;
  deployedAt?: string;
  cost: number;
  region: string;
  resourceCount: number;
}

const LandingZones: React.FC = () => {
  const [landingZones, setLandingZones] = useState<LandingZone[]>([]);
  const [loading, setLoading] = useState(true);
  const [selectedLandingZone, setSelectedLandingZone] = useState<LandingZone | null>(null);
  const [detailsDialogOpen, setDetailsDialogOpen] = useState(false);
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);

  useEffect(() => {
    fetchLandingZones();
  }, []);

  const fetchLandingZones = async () => {
    try {
      // Mock data - replace with actual API call
      const mockData: LandingZone[] = [
        {
          id: '1',
          name: 'Production Web App',
          description: 'Main production environment for customer-facing web application',
          cloudProvider: 'Azure',
          status: 'deployed',
          template: 'Azure Web App with Database',
          createdAt: '2024-01-15',
          lastModified: '2024-01-20',
          deployedAt: '2024-01-16',
          cost: 890.50,
          region: 'East US',
          resourceCount: 12,
        },
        {
          id: '2',
          name: 'Development Environment',
          description: 'Development and testing environment',
          cloudProvider: 'AWS',
          status: 'deploying',
          template: 'AWS Development Stack',
          createdAt: '2024-01-18',
          lastModified: '2024-01-21',
          cost: 245.25,
          region: 'us-east-1',
          resourceCount: 8,
        },
        {
          id: '3',
          name: 'Analytics Platform',
          description: 'Data analytics and machine learning platform',
          cloudProvider: 'GCP',
          status: 'deployed',
          template: 'GCP Analytics Suite',
          createdAt: '2024-01-10',
          lastModified: '2024-01-19',
          deployedAt: '2024-01-11',
          cost: 1315.00,
          region: 'us-central1',
          resourceCount: 24,
        },
        {
          id: '4',
          name: 'Staging Environment',
          description: 'Pre-production staging environment',
          cloudProvider: 'Azure',
          status: 'stopped',
          template: 'Azure Web App Basic',
          createdAt: '2024-01-12',
          lastModified: '2024-01-18',
          cost: 0.00,
          region: 'West US',
          resourceCount: 6,
        },
        {
          id: '5',
          name: 'Mobile Backend',
          description: 'Backend services for mobile application',
          cloudProvider: 'AWS',
          status: 'failed',
          template: 'AWS Serverless API',
          createdAt: '2024-01-20',
          lastModified: '2024-01-21',
          cost: 0.00,
          region: 'us-west-2',
          resourceCount: 0,
        },
      ];

      setLandingZones(mockData);
    } catch (error) {
      console.error('Error fetching landing zones:', error);
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
      case 'stopped':
        return 'default';
      case 'draft':
        return 'info';
      default:
        return 'default';
    }
  };

  const handleViewDetails = (landingZone: LandingZone) => {
    setSelectedLandingZone(landingZone);
    setDetailsDialogOpen(true);
  };

  const handleDelete = (landingZone: LandingZone) => {
    setSelectedLandingZone(landingZone);
    setDeleteDialogOpen(true);
  };

  const confirmDelete = async () => {
    if (!selectedLandingZone) return;
    
    try {
      // TODO: API call to delete landing zone
      setLandingZones(prev => prev.filter(lz => lz.id !== selectedLandingZone.id));
      setDeleteDialogOpen(false);
      setSelectedLandingZone(null);
    } catch (error) {
      console.error('Error deleting landing zone:', error);
    }
  };

  const handleDeploy = async (landingZoneId: string) => {
    try {
      // TODO: API call to deploy landing zone
      setLandingZones(prev =>
        prev.map(lz =>
          lz.id === landingZoneId
            ? { ...lz, status: 'deploying' as const }
            : lz
        )
      );
    } catch (error) {
      console.error('Error deploying landing zone:', error);
    }
  };

  const handleStop = async (landingZoneId: string) => {
    try {
      // TODO: API call to stop landing zone
      setLandingZones(prev =>
        prev.map(lz =>
          lz.id === landingZoneId
            ? { ...lz, status: 'stopped' as const, cost: 0 }
            : lz
        )
      );
    } catch (error) {
      console.error('Error stopping landing zone:', error);
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
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
        <Box>
          <Typography variant="h4" component="h1" gutterBottom>
            Landing Zones
          </Typography>
          <Typography variant="body1" color="text.secondary">
            Manage your deployed infrastructure and environments
          </Typography>
        </Box>
        <Box display="flex" gap={2}>
          <Button
            variant="outlined"
            startIcon={<RefreshIcon />}
            onClick={fetchLandingZones}
          >
            Refresh
          </Button>
          <Button
            variant="contained"
            startIcon={<AddIcon />}
            href="/ai-chat"
          >
            Create New
          </Button>
        </Box>
      </Box>

      {/* Landing Zones Table */}
      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Name</TableCell>
              <TableCell>Cloud Provider</TableCell>
              <TableCell>Status</TableCell>
              <TableCell>Template</TableCell>
              <TableCell>Region</TableCell>
              <TableCell>Resources</TableCell>
              <TableCell>Monthly Cost</TableCell>
              <TableCell>Last Modified</TableCell>
              <TableCell>Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {landingZones.map((landingZone) => (
              <TableRow key={landingZone.id} hover>
                <TableCell>
                  <Box>
                    <Typography variant="subtitle2">
                      {landingZone.name}
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                      {landingZone.description}
                    </Typography>
                  </Box>
                </TableCell>
                <TableCell>
                  <Chip
                    label={landingZone.cloudProvider}
                    size="small"
                    variant="outlined"
                  />
                </TableCell>
                <TableCell>
                  <Chip
                    label={landingZone.status}
                    size="small"
                    color={getStatusColor(landingZone.status) as any}
                  />
                </TableCell>
                <TableCell>{landingZone.template}</TableCell>
                <TableCell>{landingZone.region}</TableCell>
                <TableCell>{landingZone.resourceCount}</TableCell>
                <TableCell>
                  ${landingZone.cost.toFixed(2)}
                </TableCell>
                <TableCell>{landingZone.lastModified}</TableCell>
                <TableCell>
                  <Box display="flex" gap={1}>
                    <IconButton
                      size="small"
                      onClick={() => handleViewDetails(landingZone)}
                      title="View Details"
                    >
                      <ViewIcon />
                    </IconButton>
                    
                    {landingZone.status === 'deployed' || landingZone.status === 'failed' ? (
                      <IconButton
                        size="small"
                        onClick={() => handleStop(landingZone.id)}
                        title="Stop"
                        color="warning"
                      >
                        <StopIcon />
                      </IconButton>
                    ) : (
                      <IconButton
                        size="small"
                        onClick={() => handleDeploy(landingZone.id)}
                        title="Deploy"
                        color="primary"
                        disabled={landingZone.status === 'deploying'}
                      >
                        <DeployIcon />
                      </IconButton>
                    )}
                    
                    <IconButton
                      size="small"
                      title="Edit"
                    >
                      <EditIcon />
                    </IconButton>
                    
                    <IconButton
                      size="small"
                      onClick={() => handleDelete(landingZone)}
                      title="Delete"
                      color="error"
                    >
                      <DeleteIcon />
                    </IconButton>
                  </Box>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>

      {/* Details Dialog */}
      <Dialog
        open={detailsDialogOpen}
        onClose={() => setDetailsDialogOpen(false)}
        maxWidth="md"
        fullWidth
      >
        <DialogTitle>Landing Zone Details</DialogTitle>
        <DialogContent>
          {selectedLandingZone && (
            <Box>
              <Typography variant="h6" gutterBottom>
                {selectedLandingZone.name}
              </Typography>
              <Typography variant="body1" paragraph>
                {selectedLandingZone.description}
              </Typography>
              
              <Box display="grid" gridTemplateColumns="1fr 1fr" gap={2} mt={2}>
                <Box>
                  <Typography variant="subtitle2" color="text.secondary">
                    Cloud Provider
                  </Typography>
                  <Typography>{selectedLandingZone.cloudProvider}</Typography>
                </Box>
                
                <Box>
                  <Typography variant="subtitle2" color="text.secondary">
                    Template
                  </Typography>
                  <Typography>{selectedLandingZone.template}</Typography>
                </Box>
                
                <Box>
                  <Typography variant="subtitle2" color="text.secondary">
                    Region
                  </Typography>
                  <Typography>{selectedLandingZone.region}</Typography>
                </Box>
                
                <Box>
                  <Typography variant="subtitle2" color="text.secondary">
                    Resource Count
                  </Typography>
                  <Typography>{selectedLandingZone.resourceCount}</Typography>
                </Box>
                
                <Box>
                  <Typography variant="subtitle2" color="text.secondary">
                    Monthly Cost
                  </Typography>
                  <Typography>${selectedLandingZone.cost.toFixed(2)}</Typography>
                </Box>
                
                <Box>
                  <Typography variant="subtitle2" color="text.secondary">
                    Status
                  </Typography>
                  <Chip
                    label={selectedLandingZone.status}
                    size="small"
                    color={getStatusColor(selectedLandingZone.status) as any}
                  />
                </Box>
                
                <Box>
                  <Typography variant="subtitle2" color="text.secondary">
                    Created
                  </Typography>
                  <Typography>{selectedLandingZone.createdAt}</Typography>
                </Box>
                
                <Box>
                  <Typography variant="subtitle2" color="text.secondary">
                    Last Modified
                  </Typography>
                  <Typography>{selectedLandingZone.lastModified}</Typography>
                </Box>
                
                {selectedLandingZone.deployedAt && (
                  <Box>
                    <Typography variant="subtitle2" color="text.secondary">
                      Deployed
                    </Typography>
                    <Typography>{selectedLandingZone.deployedAt}</Typography>
                  </Box>
                )}
              </Box>
            </Box>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDetailsDialogOpen(false)}>
            Close
          </Button>
        </DialogActions>
      </Dialog>

      {/* Delete Confirmation Dialog */}
      <Dialog
        open={deleteDialogOpen}
        onClose={() => setDeleteDialogOpen(false)}
      >
        <DialogTitle>Confirm Delete</DialogTitle>
        <DialogContent>
          <Typography>
            Are you sure you want to delete "{selectedLandingZone?.name}"? 
            This action cannot be undone and will permanently remove all associated resources.
          </Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDeleteDialogOpen(false)}>
            Cancel
          </Button>
          <Button onClick={confirmDelete} color="error" variant="contained">
            Delete
          </Button>
        </DialogActions>
      </Dialog>
    </Container>
  );
};

export default LandingZones;