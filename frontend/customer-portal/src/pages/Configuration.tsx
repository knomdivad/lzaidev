import React, { useState, useEffect } from 'react';
import {
  Container,
  Typography,
  Box,
  Card,
  CardContent,
  CardHeader,
  TextField,
  Button,
  Switch,
  FormControlLabel,
  Divider,
  Alert,
  Snackbar,
  Tabs,
  Tab,
  Paper,
  List,
  ListItem,
  ListItemText,
  ListItemSecondaryAction,
  IconButton,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Chip,
} from '@mui/material';
import {
  CloudQueue as CloudIcon,
  Code as CodeIcon,
  Security as SecurityIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
  Add as AddIcon,
} from '@mui/icons-material';

interface CloudProvider {
  id: string;
  name: 'Azure' | 'AWS' | 'GCP';
  enabled: boolean;
  credentials: {
    [key: string]: string;
  };
  defaultRegion: string;
  budget?: {
    monthly: number;
    alertThreshold: number;
  };
}

interface SourceControlProvider {
  id: string;
  name: 'GitHub' | 'Azure DevOps' | 'GitLab';
  enabled: boolean;
  credentials: {
    [key: string]: string;
  };
  defaultOrganization?: string;
}

interface NotificationSettings {
  emailNotifications: boolean;
  deploymentAlerts: boolean;
  costAlerts: boolean;
  securityAlerts: boolean;
  email: string;
}

interface TabPanelProps {
  children?: React.ReactNode;
  index: number;
  value: number;
}

const TabPanel: React.FC<TabPanelProps> = ({ children, value, index }) => {
  return (
    <div role="tabpanel" hidden={value !== index}>
      {value === index && <Box sx={{ p: 3 }}>{children}</Box>}
    </div>
  );
};

const Configuration: React.FC = () => {
  const [tabValue, setTabValue] = useState(0);
  const [loading, setLoading] = useState(false);
  const [snackbarOpen, setSnackbarOpen] = useState(false);
  const [snackbarMessage, setSnackbarMessage] = useState('');
  
  // Cloud Providers
  const [cloudProviders, setCloudProviders] = useState<CloudProvider[]>([]);
  const [selectedCloudProvider, setSelectedCloudProvider] = useState<CloudProvider | null>(null);
  const [cloudDialogOpen, setCloudDialogOpen] = useState(false);
  
  // Source Control
  const [sourceControlProviders, setSourceControlProviders] = useState<SourceControlProvider[]>([]);
  const [selectedSourceControl, setSelectedSourceControl] = useState<SourceControlProvider | null>(null);
  const [sourceControlDialogOpen, setSourceControlDialogOpen] = useState(false);
  
  // Notifications
  const [notifications, setNotifications] = useState<NotificationSettings>({
    emailNotifications: true,
    deploymentAlerts: true,
    costAlerts: true,
    securityAlerts: true,
    email: '',
  });

  useEffect(() => {
    fetchConfiguration();
  }, []);

  const fetchConfiguration = async () => {
    try {
      // Mock data - replace with actual API calls
      setCloudProviders([
        {
          id: '1',
          name: 'Azure',
          enabled: true,
          credentials: {
            subscriptionId: '12345678-1234-1234-1234-123456789abc',
            tenantId: '87654321-4321-4321-4321-cba987654321',
          },
          defaultRegion: 'East US',
          budget: {
            monthly: 1000,
            alertThreshold: 80,
          },
        },
        {
          id: '2',
          name: 'AWS',
          enabled: false,
          credentials: {},
          defaultRegion: 'us-east-1',
        },
      ]);

      setSourceControlProviders([
        {
          id: '1',
          name: 'GitHub',
          enabled: true,
          credentials: {
            token: 'ghp_****************************',
          },
          defaultOrganization: 'mycompany',
        },
      ]);

      setNotifications({
        emailNotifications: true,
        deploymentAlerts: true,
        costAlerts: true,
        securityAlerts: true,
        email: 'user@company.com',
      });
    } catch (error) {
      console.error('Error fetching configuration:', error);
    }
  };

  const handleSaveNotifications = async () => {
    setLoading(true);
    try {
      // TODO: API call to save notification settings
      setSnackbarMessage('Notification settings saved successfully');
      setSnackbarOpen(true);
    } catch (error) {
      setSnackbarMessage('Error saving notification settings');
      setSnackbarOpen(true);
    } finally {
      setLoading(false);
    }
  };

  const handleEditCloudProvider = (provider: CloudProvider) => {
    setSelectedCloudProvider(provider);
    setCloudDialogOpen(true);
  };

  const handleEditSourceControl = (provider: SourceControlProvider) => {
    setSelectedSourceControl(provider);
    setSourceControlDialogOpen(true);
  };

  const handleSaveCloudProvider = async () => {
    if (!selectedCloudProvider) return;
    
    setLoading(true);
    try {
      // TODO: API call to save cloud provider settings
      setCloudProviders(prev =>
        prev.map(cp =>
          cp.id === selectedCloudProvider.id ? selectedCloudProvider : cp
        )
      );
      setCloudDialogOpen(false);
      setSnackbarMessage('Cloud provider settings saved successfully');
      setSnackbarOpen(true);
    } catch (error) {
      setSnackbarMessage('Error saving cloud provider settings');
      setSnackbarOpen(true);
    } finally {
      setLoading(false);
    }
  };

  const handleSaveSourceControl = async () => {
    if (!selectedSourceControl) return;
    
    setLoading(true);
    try {
      // TODO: API call to save source control settings
      setSourceControlProviders(prev =>
        prev.map(scp =>
          scp.id === selectedSourceControl.id ? selectedSourceControl : scp
        )
      );
      setSourceControlDialogOpen(false);
      setSnackbarMessage('Source control settings saved successfully');
      setSnackbarOpen(true);
    } catch (error) {
      setSnackbarMessage('Error saving source control settings');
      setSnackbarOpen(true);
    } finally {
      setLoading(false);
    }
  };

  return (
    <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
      <Typography variant="h4" component="h1" gutterBottom>
        Configuration
      </Typography>
      <Typography variant="body1" color="text.secondary" paragraph>
        Manage your cloud providers, source control integration, and notification preferences
      </Typography>

      <Paper sx={{ width: '100%' }}>
        <Tabs
          value={tabValue}
          onChange={(_, newValue) => setTabValue(newValue)}
          indicatorColor="primary"
          textColor="primary"
        >
          <Tab label="Cloud Providers" icon={<CloudIcon />} />
          <Tab label="Source Control" icon={<CodeIcon />} />
          <Tab label="Notifications" icon={<SecurityIcon />} />
        </Tabs>

        {/* Cloud Providers Tab */}
        <TabPanel value={tabValue} index={0}>
          <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
            <Typography variant="h6">Cloud Provider Configuration</Typography>
            <Button variant="contained" startIcon={<AddIcon />}>
              Add Provider
            </Button>
          </Box>
          
          <List>
            {cloudProviders.map((provider) => (
              <ListItem key={provider.id} divider>
                <ListItemText
                  primary={
                    <Box display="flex" alignItems="center" gap={2}>
                      <Typography variant="h6">{provider.name}</Typography>
                      <Chip
                        label={provider.enabled ? 'Enabled' : 'Disabled'}
                        color={provider.enabled ? 'success' : 'default'}
                        size="small"
                      />
                    </Box>
                  }
                  secondary={
                    <Box>
                      <Typography variant="body2" color="text.secondary">
                        Default Region: {provider.defaultRegion}
                      </Typography>
                      {provider.budget && (
                        <Typography variant="body2" color="text.secondary">
                          Monthly Budget: ${provider.budget.monthly} (Alert at {provider.budget.alertThreshold}%)
                        </Typography>
                      )}
                    </Box>
                  }
                />
                <ListItemSecondaryAction>
                  <IconButton
                    edge="end"
                    onClick={() => handleEditCloudProvider(provider)}
                  >
                    <EditIcon />
                  </IconButton>
                </ListItemSecondaryAction>
              </ListItem>
            ))}
          </List>
        </TabPanel>

        {/* Source Control Tab */}
        <TabPanel value={tabValue} index={1}>
          <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
            <Typography variant="h6">Source Control Integration</Typography>
            <Button variant="contained" startIcon={<AddIcon />}>
              Add Integration
            </Button>
          </Box>
          
          <List>
            {sourceControlProviders.map((provider) => (
              <ListItem key={provider.id} divider>
                <ListItemText
                  primary={
                    <Box display="flex" alignItems="center" gap={2}>
                      <Typography variant="h6">{provider.name}</Typography>
                      <Chip
                        label={provider.enabled ? 'Connected' : 'Disconnected'}
                        color={provider.enabled ? 'success' : 'default'}
                        size="small"
                      />
                    </Box>
                  }
                  secondary={
                    <Typography variant="body2" color="text.secondary">
                      {provider.defaultOrganization && `Organization: ${provider.defaultOrganization}`}
                    </Typography>
                  }
                />
                <ListItemSecondaryAction>
                  <IconButton
                    edge="end"
                    onClick={() => handleEditSourceControl(provider)}
                  >
                    <EditIcon />
                  </IconButton>
                </ListItemSecondaryAction>
              </ListItem>
            ))}
          </List>
        </TabPanel>

        {/* Notifications Tab */}
        <TabPanel value={tabValue} index={2}>
          <Typography variant="h6" gutterBottom>
            Notification Preferences
          </Typography>
          
          <Card>
            <CardContent>
              <TextField
                fullWidth
                label="Email Address"
                value={notifications.email}
                onChange={(e) =>
                  setNotifications(prev => ({ ...prev, email: e.target.value }))
                }
                margin="normal"
              />
              
              <Divider sx={{ my: 2 }} />
              
              <Typography variant="subtitle2" gutterBottom>
                Alert Types
              </Typography>
              
              <FormControlLabel
                control={
                  <Switch
                    checked={notifications.emailNotifications}
                    onChange={(e) =>
                      setNotifications(prev => ({
                        ...prev,
                        emailNotifications: e.target.checked,
                      }))
                    }
                  />
                }
                label="Email Notifications"
              />
              
              <FormControlLabel
                control={
                  <Switch
                    checked={notifications.deploymentAlerts}
                    onChange={(e) =>
                      setNotifications(prev => ({
                        ...prev,
                        deploymentAlerts: e.target.checked,
                      }))
                    }
                  />
                }
                label="Deployment Alerts"
              />
              
              <FormControlLabel
                control={
                  <Switch
                    checked={notifications.costAlerts}
                    onChange={(e) =>
                      setNotifications(prev => ({
                        ...prev,
                        costAlerts: e.target.checked,
                      }))
                    }
                  />
                }
                label="Cost Alerts"
              />
              
              <FormControlLabel
                control={
                  <Switch
                    checked={notifications.securityAlerts}
                    onChange={(e) =>
                      setNotifications(prev => ({
                        ...prev,
                        securityAlerts: e.target.checked,
                      }))
                    }
                  />
                }
                label="Security Alerts"
              />
              
              <Box mt={3}>
                <Button
                  variant="contained"
                  onClick={handleSaveNotifications}
                  disabled={loading}
                >
                  Save Notification Settings
                </Button>
              </Box>
            </CardContent>
          </Card>
        </TabPanel>
      </Paper>

      {/* Cloud Provider Edit Dialog */}
      <Dialog
        open={cloudDialogOpen}
        onClose={() => setCloudDialogOpen(false)}
        maxWidth="sm"
        fullWidth
      >
        <DialogTitle>Edit Cloud Provider</DialogTitle>
        <DialogContent>
          {selectedCloudProvider && (
            <Box>
              <TextField
                fullWidth
                label="Default Region"
                value={selectedCloudProvider.defaultRegion}
                onChange={(e) =>
                  setSelectedCloudProvider(prev =>
                    prev ? { ...prev, defaultRegion: e.target.value } : null
                  )
                }
                margin="normal"
              />
              
              {selectedCloudProvider.budget && (
                <Box>
                  <TextField
                    fullWidth
                    label="Monthly Budget"
                    type="number"
                    value={selectedCloudProvider.budget.monthly}
                    onChange={(e) =>
                      setSelectedCloudProvider(prev =>
                        prev ? {
                          ...prev,
                          budget: {
                            ...prev.budget!,
                            monthly: parseFloat(e.target.value),
                          }
                        } : null
                      )
                    }
                    margin="normal"
                  />
                  
                  <TextField
                    fullWidth
                    label="Alert Threshold (%)"
                    type="number"
                    value={selectedCloudProvider.budget.alertThreshold}
                    onChange={(e) =>
                      setSelectedCloudProvider(prev =>
                        prev ? {
                          ...prev,
                          budget: {
                            ...prev.budget!,
                            alertThreshold: parseFloat(e.target.value),
                          }
                        } : null
                      )
                    }
                    margin="normal"
                  />
                </Box>
              )}
              
              <FormControlLabel
                control={
                  <Switch
                    checked={selectedCloudProvider.enabled}
                    onChange={(e) =>
                      setSelectedCloudProvider(prev =>
                        prev ? { ...prev, enabled: e.target.checked } : null
                      )
                    }
                  />
                }
                label="Enable Provider"
                sx={{ mt: 2 }}
              />
            </Box>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setCloudDialogOpen(false)}>
            Cancel
          </Button>
          <Button onClick={handleSaveCloudProvider} variant="contained">
            Save
          </Button>
        </DialogActions>
      </Dialog>

      {/* Source Control Edit Dialog */}
      <Dialog
        open={sourceControlDialogOpen}
        onClose={() => setSourceControlDialogOpen(false)}
        maxWidth="sm"
        fullWidth
      >
        <DialogTitle>Edit Source Control</DialogTitle>
        <DialogContent>
          {selectedSourceControl && (
            <Box>
              <TextField
                fullWidth
                label="Default Organization"
                value={selectedSourceControl.defaultOrganization || ''}
                onChange={(e) =>
                  setSelectedSourceControl(prev =>
                    prev ? { ...prev, defaultOrganization: e.target.value } : null
                  )
                }
                margin="normal"
              />
              
              <FormControlLabel
                control={
                  <Switch
                    checked={selectedSourceControl.enabled}
                    onChange={(e) =>
                      setSelectedSourceControl(prev =>
                        prev ? { ...prev, enabled: e.target.checked } : null
                      )
                    }
                  />
                }
                label="Enable Integration"
                sx={{ mt: 2 }}
              />
            </Box>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setSourceControlDialogOpen(false)}>
            Cancel
          </Button>
          <Button onClick={handleSaveSourceControl} variant="contained">
            Save
          </Button>
        </DialogActions>
      </Dialog>

      <Snackbar
        open={snackbarOpen}
        autoHideDuration={6000}
        onClose={() => setSnackbarOpen(false)}
      >
        <Alert severity="success" onClose={() => setSnackbarOpen(false)}>
          {snackbarMessage}
        </Alert>
      </Snackbar>
    </Container>
  );
};

export default Configuration;