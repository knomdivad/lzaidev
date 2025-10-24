import React, { useState, useEffect } from 'react';
import {
  Box,
  Button,
  Grid,
  Typography,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Chip,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  IconButton,
  Tabs,
  Tab,
  Card,
  CardContent
} from '@mui/material';
import {
  Add as AddIcon,
  Edit as EditIcon,
  CloudQueue as CloudIcon,
  Code as CodeIcon
} from '@mui/icons-material';

interface Customer {
  id: string;
  name: string;
  contactEmail: string;
  companyName?: string;
  status: 'Active' | 'Inactive' | 'Suspended' | 'Trial';
  createdAt: string;
  lastUpdated: string;
  cloudProviders: CloudProvider[];
  sourceControlProviders: SourceControlProvider[];
  landingZoneCount: number;
}

interface CloudProvider {
  id: string;
  type: 'AWS' | 'Azure' | 'GCP';
  displayName: string;
  isDefault: boolean;
  configuration: Record<string, string>;
}

interface SourceControlProvider {
  id: string;
  type: 'GitHub' | 'GitLab' | 'AzureDevOps' | 'Bitbucket';
  displayName: string;
  repositoryUrl?: string;
  organization?: string;
  isDefault: boolean;
}

const AdminDashboard: React.FC = () => {
  const [activeTab, setActiveTab] = useState(0);
  const [customers, setCustomers] = useState<Customer[]>([]);
  const [openDialog, setOpenDialog] = useState<'create' | 'edit' | 'cloud' | 'git' | null>(null);
  const [selectedCustomer, setSelectedCustomer] = useState<Customer | null>(null);
  const [formData, setFormData] = useState<any>({});

  // Mock data - replace with API calls
  useEffect(() => {
    const mockCustomers: Customer[] = [
      {
        id: '1',
        name: 'John Doe',
        contactEmail: 'john@example.com',
        companyName: 'Example Corp',
        status: 'Active',
        createdAt: '2024-01-15',
        lastUpdated: '2024-10-20',
        cloudProviders: [
          {
            id: '1',
            type: 'AWS',
            displayName: 'Production AWS',
            isDefault: true,
            configuration: { accountId: '123456789012', region: 'us-east-1' }
          }
        ],
        sourceControlProviders: [
          {
            id: '1',
            type: 'GitHub',
            displayName: 'Company GitHub',
            repositoryUrl: 'https://github.com/example-corp',
            organization: 'example-corp',
            isDefault: true
          }
        ],
        landingZoneCount: 3
      },
      {
        id: '2',
        name: 'Jane Smith',
        contactEmail: 'jane@techstart.io',
        companyName: 'TechStart',
        status: 'Trial',
        createdAt: '2024-10-01',
        lastUpdated: '2024-10-24',
        cloudProviders: [],
        sourceControlProviders: [],
        landingZoneCount: 1
      }
    ];
    setCustomers(mockCustomers);
  }, []);

  const handleCreateCustomer = () => {
    setFormData({ name: '', contactEmail: '', companyName: '', status: 'Active' });
    setOpenDialog('create');
  };

  const handleEditCustomer = (customer: Customer) => {
    setSelectedCustomer(customer);
    setFormData({
      name: customer.name,
      contactEmail: customer.contactEmail,
      companyName: customer.companyName,
      status: customer.status
    });
    setOpenDialog('edit');
  };

  const handleConfigureCloud = (customer: Customer) => {
    setSelectedCustomer(customer);
    setFormData({ type: 'AWS', displayName: '', accountId: '', region: 'us-east-1' });
    setOpenDialog('cloud');
  };

  const handleConfigureGit = (customer: Customer) => {
    setSelectedCustomer(customer);
    setFormData({ type: 'GitHub', displayName: '', repositoryUrl: '', organization: '' });
    setOpenDialog('git');
  };

  const handleSaveCustomer = () => {
    if (openDialog === 'create') {
      console.log('Creating customer:', formData);
    } else if (openDialog === 'edit') {
      console.log('Updating customer:', selectedCustomer?.id, formData);
    }
    setOpenDialog(null);
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Active': return 'success';
      case 'Trial': return 'warning';
      case 'Suspended': return 'error';
      case 'Inactive': return 'default';
      default: return 'default';
    }
  };

  return (
    <Box sx={{ width: '100%' }}>
      <Typography variant="h4" gutterBottom>
        Customer Management
      </Typography>
      
      <Box sx={{ borderBottom: 1, borderColor: 'divider', mb: 3 }}>
        <Tabs value={activeTab} onChange={(_, newValue) => setActiveTab(newValue)}>
          <Tab label="Customers" />
          <Tab label="Analytics" />
        </Tabs>
      </Box>

      {activeTab === 0 && (
        <Box>
          <Box sx={{ mb: 3, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
            <Typography variant="h6">Customer List</Typography>
            <Button
              variant="contained"
              startIcon={<AddIcon />}
              onClick={handleCreateCustomer}
            >
              Add Customer
            </Button>
          </Box>

          <TableContainer component={Paper}>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell>Customer</TableCell>
                  <TableCell>Company</TableCell>
                  <TableCell>Status</TableCell>
                  <TableCell>Cloud Providers</TableCell>
                  <TableCell>Source Control</TableCell>
                  <TableCell>Landing Zones</TableCell>
                  <TableCell>Actions</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {customers.map((customer) => (
                  <TableRow key={customer.id}>
                    <TableCell>
                      <Box>
                        <Typography variant="subtitle2">{customer.name}</Typography>
                        <Typography variant="body2" color="text.secondary">
                          {customer.contactEmail}
                        </Typography>
                      </Box>
                    </TableCell>
                    <TableCell>{customer.companyName || '-'}</TableCell>
                    <TableCell>
                      <Chip
                        label={customer.status}
                        color={getStatusColor(customer.status) as any}
                        size="small"
                      />
                    </TableCell>
                    <TableCell>
                      <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.5 }}>
                        {customer.cloudProviders.map((provider) => (
                          <Chip
                            key={provider.id}
                            label={provider.type}
                            size="small"
                            icon={<CloudIcon />}
                          />
                        ))}
                        {customer.cloudProviders.length === 0 && (
                          <Typography variant="body2" color="text.secondary">
                            Not configured
                          </Typography>
                        )}
                      </Box>
                    </TableCell>
                    <TableCell>
                      <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.5 }}>
                        {customer.sourceControlProviders.map((provider) => (
                          <Chip
                            key={provider.id}
                            label={provider.type}
                            size="small"
                            icon={<CodeIcon />}
                          />
                        ))}
                        {customer.sourceControlProviders.length === 0 && (
                          <Typography variant="body2" color="text.secondary">
                            Not configured
                          </Typography>
                        )}
                      </Box>
                    </TableCell>
                    <TableCell>{customer.landingZoneCount}</TableCell>
                    <TableCell>
                      <IconButton
                        size="small"
                        onClick={() => handleEditCustomer(customer)}
                        title="Edit Customer"
                      >
                        <EditIcon />
                      </IconButton>
                      <IconButton
                        size="small"
                        onClick={() => handleConfigureCloud(customer)}
                        title="Configure Cloud"
                      >
                        <CloudIcon />
                      </IconButton>
                      <IconButton
                        size="small"
                        onClick={() => handleConfigureGit(customer)}
                        title="Configure Git"
                      >
                        <CodeIcon />
                      </IconButton>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </TableContainer>
        </Box>
      )}

      {activeTab === 1 && (
        <Box>
          <Typography variant="h6" gutterBottom>Platform Analytics</Typography>
          <Box sx={{ display: 'flex', gap: 3, flexWrap: 'wrap' }}>
            <Box sx={{ minWidth: 200, flex: 1 }}>
              <Card>
                <CardContent>
                  <Typography color="text.secondary" gutterBottom>Total Customers</Typography>
                  <Typography variant="h4">{customers.length}</Typography>
                </CardContent>
              </Card>
            </Box>
            <Box sx={{ minWidth: 200, flex: 1 }}>
              <Card>
                <CardContent>
                  <Typography color="text.secondary" gutterBottom>Active Landing Zones</Typography>
                  <Typography variant="h4">
                    {customers.reduce((sum, c) => sum + c.landingZoneCount, 0)}
                  </Typography>
                </CardContent>
              </Card>
            </Box>
            <Box sx={{ minWidth: 200, flex: 1 }}>
              <Card>
                <CardContent>
                  <Typography color="text.secondary" gutterBottom>Trial Customers</Typography>
                  <Typography variant="h4">
                    {customers.filter(c => c.status === 'Trial').length}
                  </Typography>
                </CardContent>
              </Card>
            </Box>
          </Box>
        </Box>
      )}

      {/* Create/Edit Customer Dialog */}
      <Dialog open={openDialog === 'create' || openDialog === 'edit'} onClose={() => setOpenDialog(null)} maxWidth="sm" fullWidth>
        <DialogTitle>
          {openDialog === 'create' ? 'Create Customer' : 'Edit Customer'}
        </DialogTitle>
        <DialogContent>
          <Box sx={{ pt: 1 }}>
            <TextField
              fullWidth
              label="Name"
              value={formData.name || ''}
              onChange={(e) => setFormData({ ...formData, name: e.target.value })}
              margin="normal"
            />
            <TextField
              fullWidth
              label="Email"
              type="email"
              value={formData.contactEmail || ''}
              onChange={(e) => setFormData({ ...formData, contactEmail: e.target.value })}
              margin="normal"
            />
            <TextField
              fullWidth
              label="Company Name"
              value={formData.companyName || ''}
              onChange={(e) => setFormData({ ...formData, companyName: e.target.value })}
              margin="normal"
            />
            <FormControl fullWidth margin="normal">
              <InputLabel>Status</InputLabel>
              <Select
                value={formData.status || 'Active'}
                onChange={(e) => setFormData({ ...formData, status: e.target.value })}
                label="Status"
              >
                <MenuItem value="Active">Active</MenuItem>
                <MenuItem value="Trial">Trial</MenuItem>
                <MenuItem value="Inactive">Inactive</MenuItem>
                <MenuItem value="Suspended">Suspended</MenuItem>
              </Select>
            </FormControl>
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenDialog(null)}>Cancel</Button>
          <Button onClick={handleSaveCustomer} variant="contained">
            {openDialog === 'create' ? 'Create' : 'Save'}
          </Button>
        </DialogActions>
      </Dialog>

      {/* Configure Cloud Provider Dialog */}
      <Dialog open={openDialog === 'cloud'} onClose={() => setOpenDialog(null)} maxWidth="sm" fullWidth>
        <DialogTitle>Configure Cloud Provider for {selectedCustomer?.name}</DialogTitle>
        <DialogContent>
          <Box sx={{ pt: 1 }}>
            <FormControl fullWidth margin="normal">
              <InputLabel>Provider Type</InputLabel>
              <Select
                value={formData.type || 'AWS'}
                onChange={(e) => setFormData({ ...formData, type: e.target.value })}
                label="Provider Type"
              >
                <MenuItem value="AWS">Amazon Web Services</MenuItem>
                <MenuItem value="Azure">Microsoft Azure</MenuItem>
                <MenuItem value="GCP">Google Cloud Platform</MenuItem>
              </Select>
            </FormControl>
            <TextField
              fullWidth
              label="Display Name"
              value={formData.displayName || ''}
              onChange={(e) => setFormData({ ...formData, displayName: e.target.value })}
              margin="normal"
            />
            {formData.type === 'AWS' && (
              <>
                <TextField
                  fullWidth
                  label="Account ID"
                  value={formData.accountId || ''}
                  onChange={(e) => setFormData({ ...formData, accountId: e.target.value })}
                  margin="normal"
                />
                <TextField
                  fullWidth
                  label="Default Region"
                  value={formData.region || ''}
                  onChange={(e) => setFormData({ ...formData, region: e.target.value })}
                  margin="normal"
                />
              </>
            )}
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenDialog(null)}>Cancel</Button>
          <Button onClick={() => setOpenDialog(null)} variant="contained">Configure</Button>
        </DialogActions>
      </Dialog>

      {/* Configure Source Control Dialog */}
      <Dialog open={openDialog === 'git'} onClose={() => setOpenDialog(null)} maxWidth="sm" fullWidth>
        <DialogTitle>Configure Source Control for {selectedCustomer?.name}</DialogTitle>
        <DialogContent>
          <Box sx={{ pt: 1 }}>
            <FormControl fullWidth margin="normal">
              <InputLabel>Provider Type</InputLabel>
              <Select
                value={formData.type || 'GitHub'}
                onChange={(e) => setFormData({ ...formData, type: e.target.value })}
                label="Provider Type"
              >
                <MenuItem value="GitHub">GitHub</MenuItem>
                <MenuItem value="GitLab">GitLab</MenuItem>
                <MenuItem value="AzureDevOps">Azure DevOps</MenuItem>
                <MenuItem value="Bitbucket">Bitbucket</MenuItem>
              </Select>
            </FormControl>
            <TextField
              fullWidth
              label="Display Name"
              value={formData.displayName || ''}
              onChange={(e) => setFormData({ ...formData, displayName: e.target.value })}
              margin="normal"
            />
            <TextField
              fullWidth
              label="Repository URL"
              value={formData.repositoryUrl || ''}
              onChange={(e) => setFormData({ ...formData, repositoryUrl: e.target.value })}
              margin="normal"
            />
            <TextField
              fullWidth
              label="Organization"
              value={formData.organization || ''}
              onChange={(e) => setFormData({ ...formData, organization: e.target.value })}
              margin="normal"
            />
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenDialog(null)}>Cancel</Button>
          <Button onClick={() => setOpenDialog(null)} variant="contained">Configure</Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};

export default AdminDashboard;