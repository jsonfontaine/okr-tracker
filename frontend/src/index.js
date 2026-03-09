import React from 'react';
import ReactDOM from 'react-dom/client';
import 'bootstrap/dist/css/bootstrap.min.css';
import '@genial/design-system/styles';
import './index.css';
import { ThemeProvider } from '@genial/design-system';
import App from './App';

const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
  <React.StrictMode>
    <ThemeProvider defaultMode="light">
      <App />
    </ThemeProvider>
  </React.StrictMode>
);
