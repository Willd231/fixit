import React from 'react';
import { Outlet, NavLink, useNavigate } from 'react-router-dom';
import Sidebar from '../components/Sidebar';
import Header from '../components/Header';
import './layout.css';

const MainLayout = () => {
  return (
    <div className="app-shell">
      <Sidebar />
      <div className="main-area">
        <Header />
        <div className="page-content">
          <Outlet />
        </div>
      </div>
    </div>
  );
};

export default MainLayout;
