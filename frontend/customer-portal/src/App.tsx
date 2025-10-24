import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import CssBaseline from '@mui/material/CssBaseline';
import { Box } from '@mui/material';

// Components
import Header from './components/Header';
import Sidebar from './components/Sidebar';
import Dashboard from './pages/Dashboard';
import AIChat from './pages/AIChat';
import LandingZones from './pages/LandingZones';
import Templates from './pages/Templates';
import Configuration from './pages/Configuration';
import Profile from './pages/Profile';
import AdminDashboard from './pages/AdminDashboard';

// Theme
const theme = createTheme({
  palette: {
    mode: 'light',
    primary: {
      main: '#1976d2',
    },
    secondary: {
      main: '#dc004e',
    },
    background: {
      default: '#f5f5f5',
    },
  },
  typography: {
    fontFamily: '"Roboto", "Helvetica", "Arial", sans-serif',
    h4: {
      fontWeight: 600,
    },
    h5: {
      fontWeight: 600,
    },
  },
});

function App() {
  const [sidebarOpen, setSidebarOpen] = React.useState(false);

  const handleSidebarToggle = () => {
    setSidebarOpen(!sidebarOpen);
  };

  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <Router>
        <Box sx={{ display: 'flex', minHeight: '100vh' }}>
          <Header onMenuClick={handleSidebarToggle} />
          <Sidebar open={sidebarOpen} onClose={() => setSidebarOpen(false)} />
          
          <Box
            component="main"
            sx={{
              flexGrow: 1,
              pt: 8, // Account for fixed header
              pl: { sm: '240px' }, // Account for sidebar on larger screens
              bgcolor: 'background.default',
              minHeight: '100vh',
            }}
          >
            <Routes>
              <Route path="/" element={<Navigate to="/dashboard" replace />} />
              <Route path="/dashboard" element={<Dashboard />} />
              <Route path="/ai-chat" element={<AIChat />} />
              <Route path="/landing-zones" element={<LandingZones />} />
              <Route path="/templates" element={<Templates />} />
              <Route path="/configuration" element={<Configuration />} />
              <Route path="/profile" element={<Profile />} />
              <Route path="/admin" element={<AdminDashboard />} />
            </Routes>
          </Box>
        </Box>
      </Router>
    </ThemeProvider>
  );
}

export default App;
