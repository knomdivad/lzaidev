import React, { useState, useEffect } from 'react';
import {
  Container,
  Typography,
  Box,
  Card,
  CardContent,
  CardActions,
  Button,
  Chip,
  TextField,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  LinearProgress,
  Accordion,
  AccordionSummary,
  AccordionDetails,
} from '@mui/material';
import {
  ExpandMore as ExpandMoreIcon,
  CloudQueue as CloudIcon,
  Code as CodeIcon,
  Security as SecurityIcon,
  Speed as SpeedIcon,
  AttachMoney as CostIcon,
} from '@mui/icons-material';

interface Template {
  id: string;
  name: string;
  description: string;
  cloudProvider: 'Azure' | 'AWS' | 'GCP' | 'Multi-Cloud';
  category: 'Web Application' | 'API' | 'Database' | 'Analytics' | 'ML/AI' | 'Storage' | 'Networking';
  complexity: 'Basic' | 'Intermediate' | 'Advanced';
  estimatedCost: {
    min: number;
    max: number;
  };
  deploymentTime: string;
  resources: string[];
  tags: string[];
  popularity: number;
  lastUpdated: string;
  parameters: TemplateParameter[];
}

interface TemplateParameter {
  name: string;
  type: 'string' | 'number' | 'boolean' | 'select';
  description: string;
  required: boolean;
  defaultValue?: any;
  options?: string[];
}

const Templates: React.FC = () => {
  const [templates, setTemplates] = useState<Template[]>([]);
  const [filteredTemplates, setFilteredTemplates] = useState<Template[]>([]);
  const [loading, setLoading] = useState(true);
  const [selectedTemplate, setSelectedTemplate] = useState<Template | null>(null);
  const [detailsDialogOpen, setDetailsDialogOpen] = useState(false);
  
  // Filters
  const [searchQuery, setSearchQuery] = useState('');
  const [selectedCloudProvider, setSelectedCloudProvider] = useState('');
  const [selectedCategory, setSelectedCategory] = useState('');
  const [selectedComplexity, setSelectedComplexity] = useState('');

  useEffect(() => {
    fetchTemplates();
  }, []);

  useEffect(() => {
    filterTemplates();
  }, [templates, searchQuery, selectedCloudProvider, selectedCategory, selectedComplexity]);

  const fetchTemplates = async () => {
    try {
      // Mock data - replace with actual API call
      const mockData: Template[] = [
        {
          id: '1',
          name: 'Azure Web App with Database',
          description: 'Complete web application setup with Azure App Service, SQL Database, and Application Insights',
          cloudProvider: 'Azure',
          category: 'Web Application',
          complexity: 'Intermediate',
          estimatedCost: { min: 75, max: 150 },
          deploymentTime: '15-30 minutes',
          resources: ['App Service', 'SQL Database', 'Application Insights', 'Key Vault'],
          tags: ['webapp', 'database', 'monitoring'],
          popularity: 85,
          lastUpdated: '2024-01-15',
          parameters: [
            {
              name: 'appName',
              type: 'string',
              description: 'Name for your web application',
              required: true,
            },
            {
              name: 'sku',
              type: 'select',
              description: 'App Service pricing tier',
              required: true,
              defaultValue: 'S1',
              options: ['F1', 'B1', 'S1', 'P1V2'],
            },
          ],
        },
        {
          id: '2',
          name: 'AWS Serverless API',
          description: 'Serverless REST API using Lambda, API Gateway, and DynamoDB',
          cloudProvider: 'AWS',
          category: 'API',
          complexity: 'Basic',
          estimatedCost: { min: 25, max: 100 },
          deploymentTime: '10-20 minutes',
          resources: ['Lambda', 'API Gateway', 'DynamoDB', 'CloudWatch'],
          tags: ['serverless', 'api', 'nosql'],
          popularity: 92,
          lastUpdated: '2024-01-18',
          parameters: [
            {
              name: 'apiName',
              type: 'string',
              description: 'Name for your API',
              required: true,
            },
            {
              name: 'runtime',
              type: 'select',
              description: 'Lambda runtime',
              required: true,
              defaultValue: 'nodejs18.x',
              options: ['nodejs18.x', 'python3.9', 'dotnet6'],
            },
          ],
        },
      ];

      setTemplates(mockData);
    } catch (error) {
      console.error('Error fetching templates:', error);
    } finally {
      setLoading(false);
    }
  };

  const filterTemplates = () => {
    let filtered = templates;

    if (searchQuery) {
      filtered = filtered.filter(template =>
        template.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
        template.description.toLowerCase().includes(searchQuery.toLowerCase()) ||
        template.tags.some(tag => tag.toLowerCase().includes(searchQuery.toLowerCase()))
      );
    }

    if (selectedCloudProvider) {
      filtered = filtered.filter(template => template.cloudProvider === selectedCloudProvider);
    }

    if (selectedCategory) {
      filtered = filtered.filter(template => template.category === selectedCategory);
    }

    if (selectedComplexity) {
      filtered = filtered.filter(template => template.complexity === selectedComplexity);
    }

    setFilteredTemplates(filtered);
  };

  const handleViewDetails = (template: Template) => {
    setSelectedTemplate(template);
    setDetailsDialogOpen(true);
  };

  const handleUseTemplate = (template: Template) => {
    // TODO: Navigate to deployment configuration page
    console.log('Using template:', template.id);
  };

  const getComplexityColor = (complexity: string) => {
    switch (complexity) {
      case 'Basic':
        return 'success';
      case 'Intermediate':
        return 'warning';
      case 'Advanced':
        return 'error';
      default:
        return 'default';
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
        Templates
      </Typography>
      <Typography variant="body1" color="text.secondary" paragraph>
        Browse and deploy pre-configured infrastructure templates for common use cases
      </Typography>

      {/* Filters */}
      <Box display="flex" gap={2} mb={3} flexWrap="wrap">
        <TextField
          placeholder="Search templates..."
          variant="outlined"
          size="small"
          value={searchQuery}
          onChange={(e) => setSearchQuery(e.target.value)}
          sx={{ minWidth: 250 }}
        />
        
        <FormControl size="small" sx={{ minWidth: 150 }}>
          <InputLabel>Cloud Provider</InputLabel>
          <Select
            value={selectedCloudProvider}
            onChange={(e) => setSelectedCloudProvider(e.target.value)}
            label="Cloud Provider"
          >
            <MenuItem value="">All</MenuItem>
            <MenuItem value="Azure">Azure</MenuItem>
            <MenuItem value="AWS">AWS</MenuItem>
            <MenuItem value="GCP">GCP</MenuItem>
            <MenuItem value="Multi-Cloud">Multi-Cloud</MenuItem>
          </Select>
        </FormControl>

        <FormControl size="small" sx={{ minWidth: 150 }}>
          <InputLabel>Category</InputLabel>
          <Select
            value={selectedCategory}
            onChange={(e) => setSelectedCategory(e.target.value)}
            label="Category"
          >
            <MenuItem value="">All</MenuItem>
            <MenuItem value="Web Application">Web Application</MenuItem>
            <MenuItem value="API">API</MenuItem>
            <MenuItem value="Database">Database</MenuItem>
            <MenuItem value="Analytics">Analytics</MenuItem>
            <MenuItem value="ML/AI">ML/AI</MenuItem>
            <MenuItem value="Storage">Storage</MenuItem>
            <MenuItem value="Networking">Networking</MenuItem>
          </Select>
        </FormControl>

        <FormControl size="small" sx={{ minWidth: 120 }}>
          <InputLabel>Complexity</InputLabel>
          <Select
            value={selectedComplexity}
            onChange={(e) => setSelectedComplexity(e.target.value)}
            label="Complexity"
          >
            <MenuItem value="">All</MenuItem>
            <MenuItem value="Basic">Basic</MenuItem>
            <MenuItem value="Intermediate">Intermediate</MenuItem>
            <MenuItem value="Advanced">Advanced</MenuItem>
          </Select>
        </FormControl>
      </Box>

      {/* Templates Grid */}
      <Box display="grid" gridTemplateColumns={{ xs: '1fr', md: 'repeat(2, 1fr)', lg: 'repeat(3, 1fr)' }} gap={3}>
        {filteredTemplates.map((template) => (
          <Card key={template.id} sx={{ height: '100%', display: 'flex', flexDirection: 'column' }}>
            <CardContent sx={{ flex: 1 }}>
              <Box display="flex" alignItems="center" gap={1} mb={2}>
                <CloudIcon />
                <Typography variant="h6" component="div">
                  {template.name}
                </Typography>
              </Box>
              
              <Typography variant="body2" color="text.secondary" paragraph>
                {template.description}
              </Typography>

              <Box display="flex" flexWrap="wrap" gap={1} mb={2}>
                <Chip
                  label={template.cloudProvider}
                  size="small"
                  variant="outlined"
                />
                <Chip
                  label={template.category}
                  size="small"
                  variant="outlined"
                />
                <Chip
                  label={template.complexity}
                  size="small"
                  color={getComplexityColor(template.complexity) as any}
                />
              </Box>

              <Box display="flex" alignItems="center" gap={2} mb={2}>
                <Box display="flex" alignItems="center" gap={0.5}>
                  <CostIcon fontSize="small" color="action" />
                  <Typography variant="body2" color="text.secondary">
                    ${template.estimatedCost.min}-${template.estimatedCost.max}/month
                  </Typography>
                </Box>
              </Box>

              <Typography variant="body2" color="text.secondary">
                Deployment: {template.deploymentTime}
              </Typography>
              <Typography variant="body2" color="text.secondary">
                Popularity: {template.popularity}%
              </Typography>
            </CardContent>

            <CardActions>
              <Button
                size="small"
                onClick={() => handleViewDetails(template)}
              >
                View Details
              </Button>
              <Button
                size="small"
                variant="contained"
                onClick={() => handleUseTemplate(template)}
              >
                Use Template
              </Button>
            </CardActions>
          </Card>
        ))}
      </Box>

      {/* Template Details Dialog */}
      <Dialog
        open={detailsDialogOpen}
        onClose={() => setDetailsDialogOpen(false)}
        maxWidth="md"
        fullWidth
      >
        <DialogTitle>Template Details</DialogTitle>
        <DialogContent>
          {selectedTemplate && (
            <Box>
              <Typography variant="h6" gutterBottom>
                {selectedTemplate.name}
              </Typography>
              <Typography variant="body1" paragraph>
                {selectedTemplate.description}
              </Typography>

              <Box display="grid" gridTemplateColumns="1fr 1fr" gap={2} mb={3}>
                <Box>
                  <Typography variant="subtitle2" color="text.secondary">
                    Cloud Provider
                  </Typography>
                  <Typography>{selectedTemplate.cloudProvider}</Typography>
                </Box>
                
                <Box>
                  <Typography variant="subtitle2" color="text.secondary">
                    Category
                  </Typography>
                  <Typography>{selectedTemplate.category}</Typography>
                </Box>
                
                <Box>
                  <Typography variant="subtitle2" color="text.secondary">
                    Complexity
                  </Typography>
                  <Chip
                    label={selectedTemplate.complexity}
                    size="small"
                    color={getComplexityColor(selectedTemplate.complexity) as any}
                  />
                </Box>
                
                <Box>
                  <Typography variant="subtitle2" color="text.secondary">
                    Estimated Cost
                  </Typography>
                  <Typography>
                    ${selectedTemplate.estimatedCost.min}-${selectedTemplate.estimatedCost.max}/month
                  </Typography>
                </Box>
              </Box>

              <Accordion>
                <AccordionSummary expandIcon={<ExpandMoreIcon />}>
                  <Typography variant="subtitle2">Resources ({selectedTemplate.resources.length})</Typography>
                </AccordionSummary>
                <AccordionDetails>
                  <Box display="flex" flexWrap="wrap" gap={1}>
                    {selectedTemplate.resources.map((resource, index) => (
                      <Chip key={index} label={resource} size="small" variant="outlined" />
                    ))}
                  </Box>
                </AccordionDetails>
              </Accordion>
            </Box>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDetailsDialogOpen(false)}>
            Close
          </Button>
          <Button
            variant="contained"
            onClick={() => {
              if (selectedTemplate) {
                handleUseTemplate(selectedTemplate);
                setDetailsDialogOpen(false);
              }
            }}
          >
            Use Template
          </Button>
        </DialogActions>
      </Dialog>
    </Container>
  );
};

export default Templates;