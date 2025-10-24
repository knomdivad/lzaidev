import React, { useState, useRef, useEffect } from 'react';
import {
  Container,
  Paper,
  Typography,
  Box,
  TextField,
  IconButton,
  List,
  ListItem,
  Avatar,
  Chip,
  Button,
  Card,
  CardContent,
  LinearProgress,
} from '@mui/material';
import {
  Send as SendIcon,
  SmartToy as AIIcon,
  Person as PersonIcon,
  Refresh as RefreshIcon,
} from '@mui/icons-material';

interface ChatMessage {
  id: string;
  content: string;
  sender: 'user' | 'ai';
  timestamp: Date;
  type?: 'text' | 'recommendation' | 'template' | 'cost-estimate';
  data?: any;
}

interface TemplateRecommendation {
  id: string;
  name: string;
  description: string;
  cloudProvider: string;
  estimatedCost: number;
  confidence: number;
}

const AIChat: React.FC = () => {
  const [messages, setMessages] = useState<ChatMessage[]>([]);
  const [currentMessage, setCurrentMessage] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [conversationId, setConversationId] = useState<string | null>(null);
  const messagesEndRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    // Initialize conversation with welcome message
    if (messages.length === 0) {
      startNewConversation();
    }
    scrollToBottom();
  }, [messages]);

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  };

  const startNewConversation = () => {
    const welcomeMessage: ChatMessage = {
      id: Date.now().toString(),
      content: "Hello! I'm your AI Assistant for creating landing zones. I'll help you gather requirements and recommend the best templates for your needs. What type of infrastructure are you looking to deploy?",
      sender: 'ai',
      timestamp: new Date(),
      type: 'text',
    };
    setMessages([welcomeMessage]);
    setConversationId(Date.now().toString());
  };

  const sendMessage = async () => {
    if (!currentMessage.trim() || isLoading) return;

    const userMessage: ChatMessage = {
      id: Date.now().toString(),
      content: currentMessage,
      sender: 'user',
      timestamp: new Date(),
      type: 'text',
    };

    setMessages(prev => [...prev, userMessage]);
    setCurrentMessage('');
    setIsLoading(true);

    try {
      // TODO: Replace with actual API call
      const response = await simulateAIResponse(currentMessage, messages);
      
      setTimeout(() => {
        setMessages(prev => [...prev, response]);
        setIsLoading(false);
      }, 1500);
    } catch (error) {
      console.error('Error sending message:', error);
      setIsLoading(false);
    }
  };

  const simulateAIResponse = async (userInput: string, conversationHistory: ChatMessage[]): Promise<ChatMessage> => {
    // Mock AI response logic
    const input = userInput.toLowerCase();
    
    if (input.includes('web app') || input.includes('website')) {
      return {
        id: (Date.now() + 1).toString(),
        content: "Great! I understand you're looking to deploy a web application. Let me ask a few questions to better understand your requirements:",
        sender: 'ai',
        timestamp: new Date(),
        type: 'text',
      };
    }
    
    if (input.includes('azure') || input.includes('aws') || input.includes('gcp')) {
      const cloudProvider = input.includes('azure') ? 'Azure' : input.includes('aws') ? 'AWS' : 'GCP';
      return {
        id: (Date.now() + 1).toString(),
        content: `Perfect! I see you're interested in ${cloudProvider}. Based on your requirements, here are my recommended templates:`,
        sender: 'ai',
        timestamp: new Date(),
        type: 'recommendation',
        data: {
          recommendations: [
            {
              id: '1',
              name: `${cloudProvider} Web App with Database`,
              description: 'Complete web application setup with managed database and auto-scaling',
              cloudProvider,
              estimatedCost: 125.50,
              confidence: 85,
            },
            {
              id: '2',
              name: `${cloudProvider} Serverless Solution`,
              description: 'Cost-effective serverless architecture for web applications',
              cloudProvider,
              estimatedCost: 45.75,
              confidence: 72,
            },
          ],
        },
      };
    }
    
    if (input.includes('cost') || input.includes('budget')) {
      return {
        id: (Date.now() + 1).toString(),
        content: "I can help you estimate costs. What's your monthly budget range, and do you have any specific performance requirements?",
        sender: 'ai',
        timestamp: new Date(),
        type: 'text',
      };
    }

    // Default response
    return {
      id: (Date.now() + 1).toString(),
      content: "I understand. Could you tell me more about your specific requirements? For example:\n• What type of application are you deploying?\n• Which cloud provider do you prefer?\n• Do you have any budget constraints?\n• What are your performance and scaling needs?",
      sender: 'ai',
      timestamp: new Date(),
      type: 'text',
    };
  };

  const handleKeyPress = (event: React.KeyboardEvent) => {
    if (event.key === 'Enter' && !event.shiftKey) {
      event.preventDefault();
      sendMessage();
    }
  };

  const createLandingZoneFromTemplate = (templateId: string) => {
    // TODO: Navigate to template configuration or deployment page
    console.log('Creating landing zone from template:', templateId);
  };

  const renderMessage = (message: ChatMessage) => {
    const isUser = message.sender === 'user';
    
    return (
      <ListItem
        key={message.id}
        sx={{
          flexDirection: 'column',
          alignItems: isUser ? 'flex-end' : 'flex-start',
          px: 2,
          py: 1,
        }}
      >
        <Box
          display="flex"
          alignItems="flex-start"
          gap={1}
          sx={{
            flexDirection: isUser ? 'row-reverse' : 'row',
            maxWidth: '70%',
          }}
        >
          <Avatar sx={{ bgcolor: isUser ? 'primary.main' : 'secondary.main' }}>
            {isUser ? <PersonIcon /> : <AIIcon />}
          </Avatar>
          
          <Paper
            elevation={1}
            sx={{
              p: 2,
              bgcolor: isUser ? 'primary.light' : 'grey.100',
              color: isUser ? 'primary.contrastText' : 'text.primary',
            }}
          >
            <Typography variant="body1" sx={{ whiteSpace: 'pre-wrap' }}>
              {message.content}
            </Typography>
            
            {/* Render template recommendations */}
            {message.type === 'recommendation' && message.data?.recommendations && (
              <Box mt={2}>
                {message.data.recommendations.map((rec: TemplateRecommendation) => (
                  <Card key={rec.id} sx={{ mt: 1 }}>
                    <CardContent sx={{ p: 2 }}>
                      <Box display="flex" justifyContent="space-between" alignItems="flex-start" mb={1}>
                        <Typography variant="h6" component="div">
                          {rec.name}
                        </Typography>
                        <Chip
                          label={`${rec.confidence}% match`}
                          size="small"
                          color={rec.confidence > 80 ? 'success' : 'warning'}
                        />
                      </Box>
                      <Typography variant="body2" color="text.secondary" mb={1}>
                        {rec.description}
                      </Typography>
                      <Box display="flex" justifyContent="space-between" alignItems="center">
                        <Typography variant="body2" color="text.secondary">
                          Estimated cost: ${rec.estimatedCost}/month
                        </Typography>
                        <Button
                          variant="contained"
                          size="small"
                          onClick={() => createLandingZoneFromTemplate(rec.id)}
                        >
                          Use Template
                        </Button>
                      </Box>
                    </CardContent>
                  </Card>
                ))}
              </Box>
            )}
            
            <Typography
              variant="caption"
              color={isUser ? 'primary.contrastText' : 'text.secondary'}
              sx={{ opacity: 0.7, mt: 1, display: 'block' }}
            >
              {message.timestamp.toLocaleTimeString()}
            </Typography>
          </Paper>
        </Box>
      </ListItem>
    );
  };

  return (
    <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
        <Box>
          <Typography variant="h4" component="h1" gutterBottom>
            AI Assistant
          </Typography>
          <Typography variant="body1" color="text.secondary">
            Get personalized recommendations for your landing zone requirements
          </Typography>
        </Box>
        <Button
          variant="outlined"
          startIcon={<RefreshIcon />}
          onClick={startNewConversation}
        >
          New Conversation
        </Button>
      </Box>

      <Paper elevation={3} sx={{ height: '600px', display: 'flex', flexDirection: 'column' }}>
        {/* Chat Messages */}
        <Box
          sx={{
            flex: 1,
            overflowY: 'auto',
            maxHeight: '500px',
          }}
        >
          <List sx={{ p: 0 }}>
            {messages.map(renderMessage)}
            {isLoading && (
              <ListItem sx={{ justifyContent: 'flex-start', px: 2 }}>
                <Box display="flex" alignItems="center" gap={1}>
                  <Avatar sx={{ bgcolor: 'secondary.main' }}>
                    <AIIcon />
                  </Avatar>
                  <Paper elevation={1} sx={{ p: 2, bgcolor: 'grey.100' }}>
                    <Typography variant="body2" color="text.secondary">
                      AI is thinking...
                    </Typography>
                    <LinearProgress sx={{ mt: 1, width: 200 }} />
                  </Paper>
                </Box>
              </ListItem>
            )}
          </List>
          <div ref={messagesEndRef} />
        </Box>

        {/* Message Input */}
        <Box
          sx={{
            borderTop: 1,
            borderColor: 'divider',
            p: 2,
            backgroundColor: 'background.paper',
          }}
        >
          <Box display="flex" gap={1}>
            <TextField
              fullWidth
              multiline
              maxRows={3}
              placeholder="Describe your infrastructure needs..."
              value={currentMessage}
              onChange={(e) => setCurrentMessage(e.target.value)}
              onKeyPress={handleKeyPress}
              disabled={isLoading}
              variant="outlined"
              size="small"
            />
            <IconButton
              color="primary"
              onClick={sendMessage}
              disabled={!currentMessage.trim() || isLoading}
              sx={{ alignSelf: 'flex-end' }}
            >
              <SendIcon />
            </IconButton>
          </Box>
        </Box>
      </Paper>
    </Container>
  );
};

export default AIChat;