import React, { useState, useEffect, useCallback } from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import AppNavbar from './components/AppNavbar';
import DashboardPage from './pages/DashboardPage';
import CiclosPage from './pages/CiclosPage';
import TimesPage from './pages/TimesPage';
import ConfigPage from './pages/ConfigPage';
import { obterStatusDatabase, desconectarDatabase, configurarDatabase } from './services/api';

function App() {
  const [dbStatus, setDbStatus] = useState({ configurado: false, caminho: null });
  const [loading, setLoading] = useState(true);

  const verificarStatus = useCallback(async () => {
    try {
      const result = await obterStatusDatabase();
      if (result.configurado) {
        setDbStatus({ configurado: true, caminho: result.caminho });
        setLoading(false);
        return;
      }

      const cachedPath = localStorage.getItem('okr-tracker-db-path');
      if (cachedPath) {
        const autoResult = await configurarDatabase(cachedPath);
        if (autoResult.success) {
          setDbStatus({ configurado: true, caminho: cachedPath });
          setLoading(false);
          return;
        }
      }

      setDbStatus({ configurado: false, caminho: null });
    } catch {
      setDbStatus({ configurado: false, caminho: null });
    }
    setLoading(false);
  }, []);

  const handleDisconnect = useCallback(async () => {
    await desconectarDatabase();
    setDbStatus({ configurado: false, caminho: null });
  }, []);

  useEffect(() => {
    verificarStatus();
  }, [verificarStatus]);

  if (loading) return null;

  return (
    <Router>
      <AppNavbar dbConfigurado={dbStatus.configurado} dbCaminho={dbStatus.caminho} onDisconnect={handleDisconnect} />
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
