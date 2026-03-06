import React, { useState, useEffect, useCallback } from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import AppNavbar from './components/AppNavbar';
import DashboardPage from './pages/DashboardPage';
import CiclosPage from './pages/CiclosPage';
import TimesPage from './pages/TimesPage';
import ConfigPage from './pages/ConfigPage';
import { obterStatusDatabase } from './services/api';

function App() {
  const [dbStatus, setDbStatus] = useState({ configurado: false, caminho: null });
  const [loading, setLoading] = useState(true);

  const verificarStatus = useCallback(async () => {
    try {
      const result = await obterStatusDatabase();
      setDbStatus({
        configurado: result.configurado || false,
        caminho: result.caminho || null,
      });
    } catch {
      setDbStatus({ configurado: false, caminho: null });
    }
    setLoading(false);
  }, []);

  useEffect(() => {
    verificarStatus();
  }, [verificarStatus]);

  if (loading) return null;

  return (
    <Router>
      <AppNavbar dbConfigurado={dbStatus.configurado} dbCaminho={dbStatus.caminho} />
      <Routes>
        <Route
          path="/"
          element={dbStatus.configurado ? <DashboardPage /> : <Navigate to="/config" replace />}
        />
        <Route
          path="/ciclos"
          element={dbStatus.configurado ? <CiclosPage /> : <Navigate to="/config" replace />}
        />
        <Route
          path="/times"
          element={dbStatus.configurado ? <TimesPage /> : <Navigate to="/config" replace />}
        />
        <Route
          path="/config"
          element={<ConfigPage onConfigured={verificarStatus} />}
        />
      </Routes>
    </Router>
  );
}

export default App;
