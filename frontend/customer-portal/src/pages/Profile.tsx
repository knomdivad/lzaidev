import React, { useState, useEffect } from 'react';
import {
  Container,
  Typography,
  Box,
  Card,
  CardContent,
  TextField,
  Button,
  Avatar,
  Divider,
  Alert,
  Snackbar,
  Paper,
  List,
  ListItem,
  ListItemText,
  ListItemIcon,
  Chip,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
} from '@mui/material';
import {
  Person as PersonIcon,
  Email as EmailIcon,
  Business as BusinessIcon,
  Phone as PhoneIcon,
  LocationOn as LocationIcon,
  CalendarToday as CalendarIcon,
  Security as SecurityIcon,
  Logout as LogoutIcon,
  Edit as EditIcon,
} from '@mui/icons-material';

interface UserProfile {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  company: string;
  phone: string;
  location: string;
  timezone: string;
  joinedDate: string;
  lastLogin: string;
  avatar?: string;
}

interface SecuritySettings {
  twoFactorEnabled: boolean;
  lastPasswordChange: string;
  loginSessions: LoginSession[];
}

interface LoginSession {
  id: string;
  device: string;
  location: string;
  ipAddress: string;
  lastActive: string;
  current: boolean;
}

const Profile: React.FC = () => {
  const [profile, setProfile] = useState<UserProfile>({
    id: '1',
    firstName: '',
    lastName: '',
    email: '',
    company: '',
    phone: '',
    location: '',
    timezone: '',
    joinedDate: '',
    lastLogin: '',
  });

  const [security, setSecurity] = useState<SecuritySettings>({
    twoFactorEnabled: false,
    lastPasswordChange: '',
    loginSessions: [],
  });

  const [loading, setLoading] = useState(false);
  const [snackbarOpen, setSnackbarOpen] = useState(false);
  const [snackbarMessage, setSnackbarMessage] = useState('');
  const [snackbarSeverity, setSnackbarSeverity] = useState<'success' | 'error'>('success');
  const [passwordDialogOpen, setPasswordDialogOpen] = useState(false);
  const [editMode, setEditMode] = useState(false);

  const [passwordForm, setPasswordForm] = useState({
    currentPassword: '',
    newPassword: '',
    confirmPassword: '',
  });

  useEffect(() => {
    fetchProfile();
    fetchSecuritySettings();
  }, []);

  const fetchProfile = async () => {
    try {
      // Mock data - replace with actual API call
      setProfile({
        id: '1',
        firstName: 'John',
        lastName: 'Doe',
        email: 'john.doe@company.com',
        company: 'TechCorp Inc.',
        phone: '+1 (555) 123-4567',
        location: 'San Francisco, CA',
        timezone: 'Pacific Time (PT)',
        joinedDate: '2023-06-15',
        lastLogin: '2024-01-21 14:30:00',
      });
    } catch (error) {
      console.error('Error fetching profile:', error);
    }
  };

  const fetchSecuritySettings = async () => {
    try {
      // Mock data - replace with actual API call
      setSecurity({
        twoFactorEnabled: true,
        lastPasswordChange: '2023-12-15',
        loginSessions: [
          {
            id: '1',
            device: 'Chrome on macOS',
            location: 'San Francisco, CA',
            ipAddress: '192.168.1.100',
            lastActive: '2024-01-21 14:30:00',
            current: true,
          },
          {
            id: '2',
            device: 'Safari on iPhone',
            location: 'San Francisco, CA',
            ipAddress: '192.168.1.101',
            lastActive: '2024-01-20 09:15:00',
            current: false,
          },
        ],
      });
    } catch (error) {
      console.error('Error fetching security settings:', error);
    }
  };

  const handleSaveProfile = async () => {
    setLoading(true);
    try {
      // TODO: API call to save profile
      setEditMode(false);
      showSnackbar('Profile updated successfully', 'success');
    } catch (error) {
      showSnackbar('Error updating profile', 'error');
    } finally {
      setLoading(false);
    }
  };

  const handleChangePassword = async () => {
    if (passwordForm.newPassword !== passwordForm.confirmPassword) {
      showSnackbar('Passwords do not match', 'error');
      return;
    }

    if (passwordForm.newPassword.length < 8) {
      showSnackbar('Password must be at least 8 characters long', 'error');
      return;
    }

    setLoading(true);
    try {
      // TODO: API call to change password
      setPasswordDialogOpen(false);
      setPasswordForm({
        currentPassword: '',
        newPassword: '',
        confirmPassword: '',
      });
      showSnackbar('Password changed successfully', 'success');
    } catch (error) {
      showSnackbar('Error changing password', 'error');
    } finally {
      setLoading(false);
    }
  };

  const handleRevokeSession = async (sessionId: string) => {
    setLoading(true);
    try {
      // TODO: API call to revoke session
      setSecurity(prev => ({
        ...prev,
        loginSessions: prev.loginSessions.filter(session => session.id !== sessionId),
      }));
      showSnackbar('Session revoked successfully', 'success');
    } catch (error) {
      showSnackbar('Error revoking session', 'error');
    } finally {
      setLoading(false);
    }
  };

  const showSnackbar = (message: string, severity: 'success' | 'error') => {
    setSnackbarMessage(message);
    setSnackbarSeverity(severity);
    setSnackbarOpen(true);
  };

  const handleLogout = () => {
    // TODO: Implement logout logic
    console.log('Logging out...');
  };

  return (
    <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
      <Typography variant="h4" component="h1" gutterBottom>
        Profile
      </Typography>
      <Typography variant="body1" color="text.secondary" paragraph>
        Manage your personal information and account settings
      </Typography>

      <Box display="grid" gridTemplateColumns={{ xs: '1fr', md: '1fr 1fr' }} gap={3}>
        {/* Profile Information */}
        <Card>
          <CardContent>
            <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
              <Typography variant="h6">Personal Information</Typography>
              <Button
                variant={editMode ? 'contained' : 'outlined'}
                startIcon={<EditIcon />}
                onClick={() => setEditMode(!editMode)}
              >
                {editMode ? 'Cancel' : 'Edit'}
              </Button>
            </Box>

            <Box display="flex" flexDirection="column" alignItems="center" mb={3}>
              <Avatar
                sx={{
                  width: 100,
                  height: 100,
                  mb: 2,
                  bgcolor: 'primary.main',
                  fontSize: '2rem',
                }}
              >
                {profile.firstName.charAt(0)}{profile.lastName.charAt(0)}
              </Avatar>
              <Typography variant="h6">
                {profile.firstName} {profile.lastName}
              </Typography>
              <Typography variant="body2" color="text.secondary">
                {profile.email}
              </Typography>
            </Box>

            <Box display="grid" gridTemplateColumns="1fr" gap={2}>
              <TextField
                label="First Name"
                value={profile.firstName}
                onChange={(e) => setProfile(prev => ({ ...prev, firstName: e.target.value }))}
                disabled={!editMode}
                fullWidth
              />
              
              <TextField
                label="Last Name"
                value={profile.lastName}
                onChange={(e) => setProfile(prev => ({ ...prev, lastName: e.target.value }))}
                disabled={!editMode}
                fullWidth
              />
              
              <TextField
                label="Email"
                value={profile.email}
                onChange={(e) => setProfile(prev => ({ ...prev, email: e.target.value }))}
                disabled={!editMode}
                fullWidth
              />
              
              <TextField
                label="Company"
                value={profile.company}
                onChange={(e) => setProfile(prev => ({ ...prev, company: e.target.value }))}
                disabled={!editMode}
                fullWidth
              />
              
              <TextField
                label="Phone"
                value={profile.phone}
                onChange={(e) => setProfile(prev => ({ ...prev, phone: e.target.value }))}
                disabled={!editMode}
                fullWidth
              />
              
              <TextField
                label="Location"
                value={profile.location}
                onChange={(e) => setProfile(prev => ({ ...prev, location: e.target.value }))}
                disabled={!editMode}
                fullWidth
              />
            </Box>

            {editMode && (
              <Box mt={3} display="flex" gap={2}>
                <Button
                  variant="contained"
                  onClick={handleSaveProfile}
                  disabled={loading}
                  fullWidth
                >
                  Save Changes
                </Button>
              </Box>
            )}

            <Divider sx={{ my: 3 }} />

            <List dense>
              <ListItem>
                <ListItemIcon>
                  <CalendarIcon />
                </ListItemIcon>
                <ListItemText
                  primary="Member Since"
                  secondary={new Date(profile.joinedDate).toLocaleDateString()}
                />
              </ListItem>
              
              <ListItem>
                <ListItemIcon>
                  <CalendarIcon />
                </ListItemIcon>
                <ListItemText
                  primary="Last Login"
                  secondary={new Date(profile.lastLogin).toLocaleString()}
                />
              </ListItem>
            </List>
          </CardContent>
        </Card>

        {/* Security Settings */}
        <Card>
          <CardContent>
            <Typography variant="h6" gutterBottom>
              Security & Privacy
            </Typography>

            <Box mb={3}>
              <Typography variant="subtitle2" gutterBottom>
                Password
              </Typography>
              <Typography variant="body2" color="text.secondary" paragraph>
                Last changed: {new Date(security.lastPasswordChange).toLocaleDateString()}
              </Typography>
              <Button
                variant="outlined"
                onClick={() => setPasswordDialogOpen(true)}
              >
                Change Password
              </Button>
            </Box>

            <Divider sx={{ my: 2 }} />

            <Box mb={3}>
              <Typography variant="subtitle2" gutterBottom>
                Two-Factor Authentication
              </Typography>
              <Chip
                label={security.twoFactorEnabled ? 'Enabled' : 'Disabled'}
                color={security.twoFactorEnabled ? 'success' : 'warning'}
                size="small"
              />
            </Box>

            <Divider sx={{ my: 2 }} />

            <Box mb={3}>
              <Typography variant="subtitle2" gutterBottom>
                Active Sessions
              </Typography>
              <List dense>
                {security.loginSessions.map((session) => (
                  <ListItem key={session.id}>
                    <ListItemText
                      primary={
                        <Box display="flex" alignItems="center" gap={1}>
                          <Typography variant="body2">
                            {session.device}
                          </Typography>
                          {session.current && (
                            <Chip label="Current" size="small" color="primary" />
                          )}
                        </Box>
                      }
                      secondary={
                        <Box>
                          <Typography variant="caption" display="block">
                            {session.location} • {session.ipAddress}
                          </Typography>
                          <Typography variant="caption" display="block">
                            Last active: {new Date(session.lastActive).toLocaleString()}
                          </Typography>
                        </Box>
                      }
                    />
                    {!session.current && (
                      <Button
                        size="small"
                        onClick={() => handleRevokeSession(session.id)}
                      >
                        Revoke
                      </Button>
                    )}
                  </ListItem>
                ))}
              </List>
            </Box>

            <Divider sx={{ my: 2 }} />

            <Button
              variant="outlined"
              color="error"
              startIcon={<LogoutIcon />}
              onClick={handleLogout}
              fullWidth
            >
              Sign Out of All Devices
            </Button>
          </CardContent>
        </Card>
      </Box>

      {/* Change Password Dialog */}
      <Dialog
        open={passwordDialogOpen}
        onClose={() => setPasswordDialogOpen(false)}
        maxWidth="sm"
        fullWidth
      >
        <DialogTitle>Change Password</DialogTitle>
        <DialogContent>
          <TextField
            fullWidth
            type="password"
            label="Current Password"
            value={passwordForm.currentPassword}
            onChange={(e) =>
              setPasswordForm(prev => ({ ...prev, currentPassword: e.target.value }))
            }
            margin="normal"
          />
          
          <TextField
            fullWidth
            type="password"
            label="New Password"
            value={passwordForm.newPassword}
            onChange={(e) =>
              setPasswordForm(prev => ({ ...prev, newPassword: e.target.value }))
            }
            margin="normal"
            helperText="Password must be at least 8 characters long"
          />
          
          <TextField
            fullWidth
            type="password"
            label="Confirm New Password"
            value={passwordForm.confirmPassword}
            onChange={(e) =>
              setPasswordForm(prev => ({ ...prev, confirmPassword: e.target.value }))
            }
            margin="normal"
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setPasswordDialogOpen(false)}>
            Cancel
          </Button>
          <Button
            onClick={handleChangePassword}
            variant="contained"
            disabled={loading}
          >
            Change Password
          </Button>
        </DialogActions>
      </Dialog>

      <Snackbar
        open={snackbarOpen}
        autoHideDuration={6000}
        onClose={() => setSnackbarOpen(false)}
      >
        <Alert
          severity={snackbarSeverity}
          onClose={() => setSnackbarOpen(false)}
        >
          {snackbarMessage}
        </Alert>
      </Snackbar>
    </Container>
  );
};

export default Profile;