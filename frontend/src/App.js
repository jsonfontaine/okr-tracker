import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import AppNavbar from './components/AppNavbar';
import DashboardPage from './pages/DashboardPage';
import CiclosPage from './pages/CiclosPage';
import TimesPage from './pages/TimesPage';
import ConfigPage from './pages/ConfigPage';

function App() {
  return (
    <Router>
      <AppNavbar />
      <Routes>
        <Route path="/" element={<DashboardPage />} />
        <Route path="/ciclos" element={<CiclosPage />} />
        <Route path="/times" element={<TimesPage />} />
        <Route path="/config" element={<ConfigPage />} />
      </Routes>
    </Router>
  );
}

export default App;
